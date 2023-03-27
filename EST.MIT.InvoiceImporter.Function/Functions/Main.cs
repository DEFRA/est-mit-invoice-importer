using InvoiceImporter.Function.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace InvoiceImporter.Function;

public static class Importer
{
    [FunctionName("MainTrigger")]
    public static async Task QueueTrigger(
        [QueueTrigger("invoice-importer",
        Connection = "QueueConnectionString")] string importMsg,
        IBinder blobBinder,
        ILogger log)
    {
        log.LogInformation($"[MainTrigger] Received message: {importMsg} at {DateTime.UtcNow.ToLongTimeString()}");
        ImportRequest request;

        if (importMsg == null)
        {
            log.LogError("No import request received.");
            return;
        }

        try
        {
            request = JsonConvert.DeserializeObject<ImportRequest>(importMsg);
        }
        catch (JsonException ex)
        {
            log.LogError(ex, "Invalid import request received.");
            return;
        }

        var blobAttr = new BlobAttribute($"invoices/import/{request.FileName}", FileAccess.Read)
        {
            Connection = "StorageConnectionString"
        };

        using var blobStream = await blobBinder.BindAsync<Stream>(blobAttr);
        log.LogInformation("File read into blobstream.");
    }
}