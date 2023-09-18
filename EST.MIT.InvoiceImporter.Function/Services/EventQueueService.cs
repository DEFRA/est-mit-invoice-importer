using Azure.Storage.Queues;
using EST.MIT.InvoiceImporter.Function.Interfaces;
using EST.MIT.InvoiceImporter.Function.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EST.MIT.InvoiceImporter.Function.Services;

public class EventQueueService : IEventQueueService
{
    private readonly QueueClient _queueClient;
    private readonly ILogger<EventQueueService> _logger;

    public EventQueueService(QueueClient queueClient)
    {
        _queueClient = queueClient;
    }

    public EventQueueService(QueueClient queueClient, ILogger<EventQueueService> logger)
    {
        _queueClient = queueClient;
        _logger = logger;
    }

    public async Task CreateMessage(string id, string status, string action, string message, PaymentRequestsBatch? paymentRequestsBatch = null)
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
                    Data = JsonSerializer.Serialize(paymentRequestsBatch)
                }
            }
        };

        var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(eventRequest));
        try
        {
            await _queueClient.SendMessageAsync(Convert.ToBase64String(bytes));
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occured when sending the message with id {id} to the queue.");
            _logger.LogError(ex.Message);
        }
    }
}