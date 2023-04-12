using Azure.Storage.Blobs;
using EST.MIT.InvoiceImporter.Function.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Services;
using System;
using System.Threading.Tasks;

namespace EST.MIT.Importer.Function.Services
{
    public class Importer : IImporter
    {
        private readonly IBlobService _blobService;
        private readonly IConfiguration _configuration;
        private readonly BlobServiceClient _blobServiceClient;

        public Importer(IBlobService blobService, IConfiguration configuration, IAzureBlobService azureBlobService) 
        {
            _blobService= blobService;
            _configuration = configuration;
            _blobServiceClient = azureBlobService.blobServiceClient == null? new BlobServiceClient(_configuration.GetConnectionString("PrimaryConnection")) : azureBlobService.blobServiceClient;
        }

        [FunctionName("MainTrigger")]
        public async Task QueueTrigger(
            [QueueTrigger("invoice-importer", Connection = "QueueConnectionString")] string importMessage,
            IBinder blobBinder,
            ILogger log)
        {
            log.LogInformation($"[MainTrigger] Recieved message: {importMessage} at {DateTime.UtcNow.ToLongTimeString()}");



            var somthingElse = _configuration["StorageConnectionString"];

            //var blobServiceClient = new BlobServiceClient(somthingElse);

            using (await _blobService.ReadBLOBIntoStream(importMessage, log, blobBinder))
            {
                await _blobService.MoveFileToArchive(_blobService.GetFileName(), log, _blobServiceClient);
            }
        }
    }
}