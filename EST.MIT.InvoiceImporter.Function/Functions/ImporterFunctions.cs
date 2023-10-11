using System;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using EST.MIT.Importer.Function.Interfaces;
using EST.MIT.InvoiceImporter.Function.Interfaces;
using EST.MIT.InvoiceImporter.Function.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EST.MIT.InvoiceImporter.Function.Functions;
public class ImporterFunctions : IImporterFunctions
{
    private readonly IBlobService _blobService;
    private readonly IConfiguration _configuration;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly IEventQueueService _eventQueueService;
    private readonly IAzureTableService _azureTableService;

    public ImporterFunctions(IBlobService blobService, IConfiguration configuration, IAzureBlobService azureBlobService, IEventQueueService eventQueueService, IAzureTableService azureTableService)
    {
        _blobService = blobService;
        _configuration = configuration;
        _eventQueueService = eventQueueService;
        _blobServiceClient = azureBlobService.BlobServiceClient ?? new BlobServiceClient(_configuration.GetConnectionString("PrimaryConnection"));
        _azureTableService = azureTableService;
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