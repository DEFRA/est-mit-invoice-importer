using EST.MIT.InvoiceImporter.Function.Models;
using System.Threading.Tasks;

namespace EST.MIT.InvoiceImporter.Function.Interfaces;
public interface INotificationQueueService
{
    Task<bool> AddMessageToQueueAsync(Notification request);
}