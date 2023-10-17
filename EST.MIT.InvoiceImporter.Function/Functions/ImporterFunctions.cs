using System;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using EST.MIT.Importer.Function.Interfaces;
using EST.MIT.InvoiceImporter.Function.Interfaces;
using EST.MIT.InvoiceImporter.Function.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EST.MIT.InvoiceImporter.Function.Functions;
public class ImporterFunctions : IImporterFunctions
{
    private readonly IBlobService _blobService;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly IAzureTableService _azureTableService;

    private readonly IEventQueueService _eventQueueService;
    private readonly INotificationService _notificationService;

    public ImporterFunctions(IBlobService blobService, IAzureBlobService azureBlobService, IAzureTableService azureTableService, IEventQueueService eventQueueService, INotificationService notificationService)
    {
        _blobService = blobService;
        _blobServiceClient = azureBlobService.GetBlobServiceClient();
        _azureTableService = azureTableService;
        _eventQueueService = eventQueueService;
        _notificationService = notificationService;
    }

    [FunctionName("MainTrigger")]
    public async Task QueueTrigger(
        [QueueTrigger("rpa-mit-invoice-importer", Connection = "QueueConnectionString")] string importMessage,
        IBinder blobBinder,
        ILogger log)
    {
        log.LogInformation($"[MainTrigger] Recieved message: {importMessage} at {DateTime.UtcNow.ToLongTimeString()}");
        using (await _blobService.ReadBLOBIntoStream(importMessage, blobBinder))
        {
            var isMoved = await _blobService.MoveFileToArchive(_blobService.GetFileName(), _blobServiceClient);

            if (isMoved)
            {
                var importRequest = JsonConvert.DeserializeObject<ImportRequest>(importMessage);
                importRequest.FileName = $"archive/{importRequest.FileName}";
                var newImportMessage = JsonConvert.SerializeObject(importRequest);
                await _azureTableService.UpsertImportRequestAsync(importRequest);
            }
        }
    }
}