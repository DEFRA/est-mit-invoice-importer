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
    private readonly IAzureBlobService _blobService;
    private readonly IAzureTableService _azureTableService;


    public ImporterFunctions(IAzureBlobService azureBlobService, IAzureTableService azureTableService)
    {
        _blobService = azureBlobService;
        _azureTableService = azureTableService;
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

                if (isMoved)
                {
                    var importRequest = JsonConvert.DeserializeObject<ImportRequest>(importMessage);
                    importRequest.FileName = $"archive/{importRequest.FileName}";
                    var newImportMessage = JsonConvert.SerializeObject(importRequest);
                    await _azureTableService.UpsertImportRequestAsync(importRequest);
                    log.LogInformation($"[MainTrigger] Successfully moved and processed file: {importRequest.FileName}");
                }
                else
                {
                    log.LogWarning($"[MainTrigger] Failed to move the file to archive.");
                }
            }
        }
        catch (Exception ex)
        {
            log.LogError(ex, $"[MainTrigger] An error occurred while processing the message: {importMessage}");
            throw;
        }
    }
}