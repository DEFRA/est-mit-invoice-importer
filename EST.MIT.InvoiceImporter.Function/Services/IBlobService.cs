using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace EST.MIT.InvoiceImporter.Function.Services
{
    public interface IBlobService
    {
        Task<Stream> ReadBLOBIntoStream(string importMsg, ILogger log, IBinder blobBinder);
    }
}