using System.Threading.Tasks;

namespace EST.MIT.InvoiceImporter.Function.Interfaces;

public interface IEventQueueService
{
    Task CreateMessage(string status, string action, string message, string data);
}
