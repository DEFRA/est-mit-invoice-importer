using System;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using EST.MIT.InvoiceImporter.Function.Builders;
using EST.MIT.InvoiceImporter.Function.DataAccess;
using EST.MIT.InvoiceImporter.Function.Interfaces;
using EST.MIT.InvoiceImporter.Function.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EST.MIT.InvoiceImporter.Function.Functions;
public class ImporterFunctions : IImporterFunctions
{
    private readonly IAzureBlobService _blobService;
    private readonly IAzureTableService _azureTableService;
    private readonly INotificationQueueService _notificationQueueService;
    private readonly IConfiguration _configuration;


    public ImporterFunctions(IAzureBlobService azureBlobService,
        IAzureTableService azureTableService,
        INotificationQueueService notificationQueueService,
        IConfiguration configuration)
    {
        _blobService = azureBlobService;
        _azureTableService = azureTableService;
        _notificationQueueService = notificationQueueService;
        _configuration = configuration;
    }

    [FunctionName("MainTrigger")]
    public async Task QueueTrigger(
        [QueueTrigger("rpa-mit-invoice-importer", Connection = "QueueConnectionString")] string importMessage,
            IBinder blobBinder,
            ILogger log)
    {
        log.LogInformation($"[MainTrigger] Received message: {importMessage} at {DateTime.UtcNow.ToLongTimeString()}");

        try
        {
            using (await _blobService.ReadBLOBIntoStream(importMessage, blobBinder))
            {
                BlobServiceClient blobServiceClient = _blobService.GetBlobServiceClient();
                var isMoved = await _blobService.MoveFileToArchive(_blobService.GetFileName(), blobServiceClient);
                var importRequest = JsonConvert.DeserializeObject<ImportRequest>(importMessage);

                if (isMoved)
                {
                    importRequest.BlobFolder = AzureBlobService.folder_archive;
                    importRequest.Status = UploadStatus.Upload_success;
                    var newImportMessage = JsonConvert.SerializeObject(importRequest);
                    await _azureTableService.UpsertImportRequestAsync(importRequest);
                    log.LogInformation($"[MainTrigger] Successfully moved and processed file: {importRequest.FileName}");
                }
                else
                {
                    importRequest.Status = UploadStatus.Upload_failed;
                    log.LogWarning($"[MainTrigger] Failed to move the file to archive.");
                }

                var notification = CreateNotificationRequest(importRequest);
                await _notificationQueueService.AddMessageToQueueAsync(notification);
            }
        }
        catch (Exception ex)
        {
            log.LogError(ex, $"[MainTrigger] An error occurred while processing the message: {importMessage}");
            throw;
        }
    }

    private Notification CreateNotificationRequest(ImportRequest importMessage)
    {
        var baseUrl = _configuration.GetValue<string>("WebUIBaseUrl");

        return new NotificationBuilder()
                                        .WithId(importMessage.ImportRequestId.ToString())
                                        .WithScheme(importMessage.SchemeType)
                                        .WithAction(importMessage.Status)
                                        .WithEmailRecipient(importMessage.Email)
                                        .WithData(new NotificationOutstandingApproval
                                        {
                                            Name = importMessage.Email,
                                            Link = $"{baseUrl}/user-uploads",
                                            ImportRequestId = importMessage.ImportRequestId.ToString(),
                                            SchemeType = importMessage.SchemeType
                                        })
                                    .Build();
    }
}