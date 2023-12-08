using Azure.Storage.Queues.Models;
using Azure.Storage.Queues;
using Azure;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Threading.Tasks;
using System;
using EST.MIT.InvoiceImporter.Function.Interfaces;
using EST.MIT.InvoiceImporter.Function.Models;
using System.Text.Json;

namespace EST.MIT.InvoiceImporter.Function.Services;
public class NotificationQueueService : INotificationQueueService
{
    private readonly ILogger<INotificationQueueService> _logger;
    private readonly QueueClient _queueClient;

    public NotificationQueueService(QueueClient queueClient, ILogger<INotificationQueueService> logger)
    {
        _queueClient = queueClient;
        _logger = logger;
    }

    public async Task<bool> AddMessageToQueueAsync(Notification request)
    {
        try
        {
            var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request));
            await _queueClient.SendMessageAsync(Convert.ToBase64String(bytes));

            return true;

        }
        catch (RequestFailedException ex)
                when (ex.ErrorCode == QueueErrorCode.QueueAlreadyExists)
        {
            // Ignore any errors if the queue already exists

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error {ex.Message} sending \"{request}\" message to Notification Queue.");
            return false;
        }
    }
}