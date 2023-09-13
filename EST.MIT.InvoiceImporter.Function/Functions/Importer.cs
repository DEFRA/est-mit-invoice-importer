using System;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using EST.MIT.Importer.Function.Interfaces;
using EST.MIT.InvoiceImporter.Function.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EST.MIT.InvoiceImporter.Function.Services;
public class Importer : IImporter
{
    private readonly IBlobService _blobService;
    private readonly IConfiguration _configuration;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly IEventQueueService _eventQueueService;

    public Importer(IBlobService blobService, IConfiguration configuration, IAzureBlobService azureBlobService, IEventQueueService eventQueueService)
    {
        _blobService = blobService;
        _configuration = configuration;
        _eventQueueService = eventQueueService;
        _blobServiceClient = azureBlobService.BlobServiceClient ?? new BlobServiceClient(_configuration.GetConnectionString("PrimaryConnection"));
    }

    [FunctionName("MainTrigger")]
    public async Task QueueTrigger(
        [QueueTrigger("invoice-importer", Connection = "QueueConnectionString")] string importMessage,
        IBinder blobBinder,
        ILogger log)
    {
        log.LogInformation($"[MainTrigger] Recieved message: {importMessage} at {DateTime.UtcNow.ToLongTimeString()}");
        using (await _blobService.ReadBLOBIntoStream(importMessage, blobBinder))
        {
            await _blobService.MoveFileToArchive(_blobService.GetFileName(), _blobServiceClient);
        }
    }
}