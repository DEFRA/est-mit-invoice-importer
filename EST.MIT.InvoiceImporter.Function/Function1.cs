using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace EST.MIT.InvoiceImporter.Function
{
    public static class Importer
    {
        [FunctionName("QueueTrigger")]
        public static void QueueTrigger(
            [QueueTrigger("invoice-importer")] string myQueueItem,
            ILogger log)
        {
            log.LogInformation($"C# function processed: {myQueueItem}");
        }
    }
}