using EST.MIT.InvoiceImporter.Function.Models;
using System.Threading.Tasks;

namespace EST.MIT.InvoiceImporter.Function.Interfaces;

public interface IEventQueueService
{
    Task CreateMessage(string id, string status, string action, string message, Invoice? invoice = null);
}