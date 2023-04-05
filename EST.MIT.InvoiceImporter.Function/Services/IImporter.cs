using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace EST.MIT.Importer.Function.Services
{
    public interface IImporter
    {
        Task QueueTrigger([QueueTrigger("invoice-importer", Connection = "QueueConnectionString")] string importMessage, IBinder blobBinder, ILogger log);
    }
}