using Azure.Storage.Blobs;
using AzureServices;
using EST.MIT.InvoiceImporter.Function.Services;
using InvoiceImporter.Function.Service;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace EST.MIT.Importer.Function.Services
{
    public class Importer : IImporter
    {
        private readonly IBlobService _blobService;
        private readonly IConfiguration _configuration;
        private readonly BlobServiceClient _blobServiceClient;
        public static IInvoiceParser InvoiceParser = new InvoiceParser();

        public Importer(IBlobService blobService, IConfiguration configuration, IAzureBlobService azureBlobService)
        {
            _blobService = blobService;
            _configuration = configuration;
            _blobServiceClient = azureBlobService.BlobServiceClient ?? new BlobServiceClient(_configuration.GetConnectionString("PrimaryConnection"));
        }

        [FunctionName("MainTrigger")]
        public async Task QueueTrigger(
            [QueueTrigger("invoice-importer", Connection = "QueueConnectionString")] string importMessage,
            IBinder blobBinder,
            ILogger log)
        {
            log.LogInformation($"[MainTrigger] Recieved message: {importMessage} at {DateTime.UtcNow.ToLongTimeString()}");
            using Stream dataStream = await _blobService.ReadBLOBIntoStream(importMessage, log, blobBinder);
            var invoices = await InvoiceParser.GetInvoicesAsync(dataStream, log);

            await _blobService.MoveFileToArchive(_blobService.GetFileName(), log, _blobServiceClient);
        }
    }
}