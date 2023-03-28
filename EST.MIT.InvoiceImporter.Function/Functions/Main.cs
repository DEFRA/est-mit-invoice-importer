using InvoiceImporter.Function.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using EST.MIT.InvoiceImporter.Function.Services;

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
        BlobService blobService = new ();
        Stream memoryStream = await blobService.ReadBLOBIntoStream(importMsg, log, blobBinder);
        log.LogInformation("Memory stream contains {0} bytes", memoryStream?.Length.ToString() ?? "0");
        log.LogInformation("File read into blobstream.");
    }
}