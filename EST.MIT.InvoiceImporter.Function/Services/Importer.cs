using Azure.Storage.Blobs;
using EST.MIT.InvoiceImporter.Function.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace EST.MIT.Importer.Function.Services
{
    public class Importer : IImporter
    {
        private readonly IBlobService _blobService;
        private readonly IConfiguration _configuration;

        public Importer(IBlobService blobService, IConfiguration configuration) 
        {
            _blobService= blobService;
            _configuration = configuration; 
        }

        [FunctionName("MainTrigger")]
        public async Task QueueTrigger(
            [QueueTrigger("invoice-importer", Connection = "QueueConnectionString")] string importMessage,
            IBinder blobBinder,
            ILogger log)
        {
            log.LogInformation($"[MainTrigger] Recieved message: {importMessage} at {DateTime.UtcNow.ToLongTimeString()}");
            var blobServiceClient = new BlobServiceClient(_configuration["StorageConnectionString"]);

            using (await _blobService.ReadBLOBIntoStream(importMessage, log, blobBinder))
            {
                await _blobService.MoveFileToArchive(_blobService.GetFileName(), log, blobServiceClient);
            }
        }
    }
}