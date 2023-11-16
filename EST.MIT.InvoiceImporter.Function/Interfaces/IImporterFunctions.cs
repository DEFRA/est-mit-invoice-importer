using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace EST.MIT.InvoiceImporter.Function.Interfaces;
public interface IImporterFunctions
{
    Task QueueTrigger([QueueTrigger("invoice-importer", Connection = "QueueConnectionString")] string importMessage, IBinder blobBinder, ILogger log);
}
