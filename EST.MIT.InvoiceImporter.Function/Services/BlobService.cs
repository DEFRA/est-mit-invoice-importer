using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using InvoiceImporter.Function.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace EST.MIT.InvoiceImporter.Function.Services
{
    public class BlobService : IBlobService
    {
        private string _fileName;

        public async Task<Stream> ReadBLOBIntoStream(string importMsg, ILogger log, IBinder blobBinder)
        {
            Stream blobStream = null;
            if (importMsg == null)
            {
                log.LogError("No import request received.");
                return blobStream;
            }

            ImportRequest importRequest;
            try
            {
                importRequest = JsonConvert.DeserializeObject<ImportRequest>(importMsg);
            }
            catch (JsonException ex)
            {
                log.LogError(ex, "Invalid import request received.");
                return blobStream;
            }

            var blobAttr = new BlobAttribute($"invoices/import/{importRequest.FileName}", FileAccess.Read)
            {
                Connection = "StorageConnectionString"
            };

            _fileName = importRequest.FileName;
            return await blobBinder.BindAsync<Stream>(blobAttr);
        }

        [ExcludeFromCodeCoverage]
        public async Task<bool> MoveFileToArchive(string fileName, ILogger log, BlobServiceClient blobServiceClient)
        {
            try
            {
                var containerClient = blobServiceClient.GetBlobContainerClient("invoices");
                var sourceBlobClient = containerClient.GetBlobClient($"import/{fileName}");
                var destBlobClient = containerClient.GetBlobClient($"archive/{fileName}");
                CopyFromUriOperation copyOperation = await destBlobClient.StartCopyFromUriAsync(sourceBlobClient.Uri);
                await copyOperation.WaitForCompletionAsync();
                await sourceBlobClient.DeleteIfExistsAsync();
                return true;
            }
            catch (Exception ex)
            {
                log.LogError($"An error occured when moving a file to the archive folder: [{0}]", ex);
                return false;
            }
        }

        public virtual string GetFileName()
        {
            return _fileName;
        }
    }
}
