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
    }
}
