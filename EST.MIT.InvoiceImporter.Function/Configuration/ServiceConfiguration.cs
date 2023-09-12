using Azure.Storage.Queues;
using EST.MIT.InvoiceImporter.Function.Interfaces;
using EST.MIT.InvoiceImporter.Function.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EST.MIT.InvoiceImporter.Function.Configuration;
public static class ServiceConfiguration
{
    public static IServiceCollection RegisterServices(this IServiceCollection services, string storageConnection, string eventQueueName)
    {
        services.AddSingleton<IEventQueueService>(_ =>
        {
            var eventQueueClient = new QueueClient(storageConnection, eventQueueName);
            return new EventQueueService(eventQueueClient);
        });

        return services;
    }
}
