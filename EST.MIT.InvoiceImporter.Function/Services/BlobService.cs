using Azure.Storage.Blobs;
using InvoiceImporter.Function.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace EST.MIT.InvoiceImporter.Function.Services
{
    public class BlobService : IBlobService
    {
        public async Task<Stream> ReadBLOBIntoStream(string importMsg, ILogger log, IBinder blobBinder)
        {
            Stream blobStream = null;
            ImportRequest importRequest = null;

            if (importMsg == null)
            {
                log.LogError("No import request received.");
                return blobStream;
            }

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

            return await blobBinder.BindAsync<Stream>(blobAttr);
        }

        //Write a stream to a blob
        public async Task<bool> WriteStreamToBLOB(string blobName, ILogger log, IBinder blobBinder)
        {
            try
            {
                var blobAttr = new BlobAttribute($"invoices/import/archive/{blobName}", FileAccess.Write)
                {
                    Connection = "StorageConnectionString"
                };
                var blobBinding = await blobBinder.BindAsync<Stream>(blobAttr);
                return true;
            }
            catch (JsonException ex) { log.LogError($"An error occured when creating an archive file: [{0}]", ex); }
        }

        public async Task<bool> MoveFileIntoArchive(string fileName, ILogger log, IBinder blobBinder)
        {
            //Copy a file matching the fileName to the archive folder in the blob storage
            var sourceFile = new BlobAttribute($"invoices/import/{fileName}", FileAccess.Read)
            {
                Connection = "StorageConnectionString"
            };

            var destFile = new BlobAttribute($"invoices/archive/{fileName}", FileAccess.Write)
            {
                Connection = "StorageConnectionString"
            };

            var sourceBinding = await blobBinder.BindAsync<Stream>(sourceFile);

            var destBinding = await blobBinder.BindAsync<Stream>(destFile);
            
        return true;
        }
    }
}
