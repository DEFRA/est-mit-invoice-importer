using Azure.Storage.Queues;
using EST.MIT.InvoiceImporter.Function.Interfaces;
using EST.MIT.InvoiceImporter.Function.Models;
using InvoiceImporter.Function.Models;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EST.MIT.InvoiceImporter.Function.Services;

public class EventQueueService : IEventQueueService
{
    private readonly QueueClient _queueClient;

    public EventQueueService(QueueClient queueClient)
    {
        _queueClient = queueClient;
    }

    public async Task CreateMessage(string id, string status, string action, string message, Invoice? invoice = null)
    {
        var eventRequest = new Event()
        {
            Name = "Invoice",
            Properties = new EventProperties()
            {
                Id = id,
                Status = status,
                Checkpoint = "Invoice Api",
                Action = new EventAction()
                {
                    Type = action,
                    Message = message,
                    Timestamp = DateTime.UtcNow,
                    Data = invoice == null ? null : JsonSerializer.Serialize(invoice)
                }
            }
        };

        var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(eventRequest));
        await _queueClient.SendMessageAsync(Convert.ToBase64String(bytes));
    }
}
