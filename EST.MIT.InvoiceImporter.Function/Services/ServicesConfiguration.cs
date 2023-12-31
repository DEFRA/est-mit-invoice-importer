using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using Azure.Storage.Queues;
using Azure.Identity;
using Azure.Data.Tables;
using EST.MIT.InvoiceImporter.Function.Interfaces;
using EST.MIT.InvoiceImporter.Function.DataAccess;
using Azure.Storage.Blobs;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace EST.MIT.InvoiceImporter.Function.Services
{
    /// <summary>
    /// Register service-tier services.
    /// </summary>
    public static class ServicesConfiguration
    {
        /// <summary>
        /// Method to register service-tier services.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="mapperConfig">Automapper config</param>
        public static void AddTableBlobQueueServices(this IServiceCollection services, IConfiguration configuration, MapperConfiguration mapperConfig)
        {
            services.AddSingleton<IAzureTableService>(_ =>
            {
                var tableStorageAccountCredential = configuration.GetSection("TableConnectionString:Credential").Value;
                var ImporterTableName = configuration.GetSection("ImporterTableName").Value;
                if (IsManagedIdentity(tableStorageAccountCredential))
                {
                    var tableServiceUri = new Uri(configuration.GetSection("TableConnectionString:TableServiceUri").Value);
                    Console.WriteLine($"Startup.TableClient using Managed Identity with url {tableServiceUri}");
                    return new AzureTableService(new TableClient(tableServiceUri, ImporterTableName, new DefaultAzureCredential()), mapperConfig.CreateMapper());
                }
                else
                {
                    return new AzureTableService(new TableClient(configuration.GetSection("TableConnectionString").Value, ImporterTableName), mapperConfig.CreateMapper());
                }
            });

            services.AddSingleton<IAzureBlobService>(_ =>
            {
                var blobStorageAccountCredential = configuration.GetSection("BlobConnectionString:Credential").Value;
                var blobContainerName = configuration.GetSection("BlobContainerName").Value;
                blobContainerName = string.IsNullOrWhiteSpace(blobContainerName) ? AzureBlobService.default_BlobContainerName : blobContainerName;
                var logger = _.GetService<ILogger<AzureBlobService>>();
                if (IsManagedIdentity(blobStorageAccountCredential))
                {
                    var blobServiceUri = new Uri(configuration.GetSection("BlobConnectionString:BlobServiceUri").Value);
                    Console.WriteLine($"Startup.BlobClient using Managed Identity with url {blobServiceUri}");
                    return new AzureBlobService(new BlobServiceClient(blobServiceUri, new DefaultAzureCredential()), logger, blobContainerName);
                }
                else
                {
                    return new AzureBlobService(new BlobServiceClient(configuration.GetSection("BlobConnectionString").Value), logger, blobContainerName);
                }
            });


            services.AddSingleton<IEventQueueService>(_ =>
            {
                var eventQueueName = configuration.GetSection("EventQueueName").Value;
                var queueStorageAccountCredential = configuration.GetSection("QueueConnectionString:Credential").Value;

                if (IsManagedIdentity(queueStorageAccountCredential))
                {
                    var queueServiceUri = configuration.GetSection("QueueConnectionString:QueueServiceUri").Value;
                    var queueUrl = new Uri($"{queueServiceUri}{eventQueueName}");
                    return new EventQueueService(new QueueClient(queueUrl, new DefaultAzureCredential()));
                }
                else
                {
                    return new EventQueueService(new QueueClient(configuration.GetSection("QueueConnectionString").Value, eventQueueName));
                }
            });

            services.AddSingleton<INotificationQueueService>(_ =>
            {
                var notificationQueueName = configuration.GetSection("NotificationQueueName").Value;
                var queueStorageAccountCredential = configuration.GetSection("QueueConnectionString:Credential").Value;
                var logger = _.GetService<ILogger<INotificationQueueService>>();
                if (logger is null)
                {
                    return null;
                }
                if (IsManagedIdentity(queueStorageAccountCredential))
                {
                    var queueServiceUri = configuration.GetSection("QueueConnectionString:QueueServiceUri").Value;
                    var queueUrl = new Uri($"{queueServiceUri}{notificationQueueName}");
                    return new NotificationQueueService(new QueueClient(queueUrl, new DefaultAzureCredential()), logger);
                }
                else
                {
                    return new NotificationQueueService(new QueueClient(configuration.GetSection("QueueConnectionString").Value, notificationQueueName), logger);
                }
            });
        }

        private static bool IsManagedIdentity(string credentialName)
        {
            return credentialName != null && credentialName.ToLower() == "managedidentity";
        }
    }
}