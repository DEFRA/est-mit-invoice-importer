using AutoMapper;
using EST.MIT.Importer.Function.Interfaces;
using EST.MIT.InvoiceImporter.Function.AutoMapperProfiles;
using EST.MIT.InvoiceImporter.Function.Functions;
using EST.MIT.InvoiceImporter.Function.Interfaces;
using EST.MIT.InvoiceImporter.Function.Services;
using EST.MIT.InvoiceImporter.Function.DataAccess;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;
using Azure.Data.Tables;
using Azure.Identity;
using Azure.Storage.Blobs;

[assembly: FunctionsStartup(typeof(Startup.Function.Startup))]

namespace Startup.Function;
public class Startup : FunctionsStartup
{
    private static IConfiguration Configuration { get; set; }

    [ExcludeFromCodeCoverage]
    public override void Configure(IFunctionsHostBuilder builder)
    {
        Configuration = builder.GetContext().Configuration;

        Console.WriteLine("Configuring services...");

        builder.Services.AddSingleton<IAzureTableService, AzureTableService>();

        var mapperConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new ImportRequestMapper());
        });

        builder.Services.AddSingleton<IAzureTableService>(_ =>
        {
            var storageAccountCredential = Configuration.GetSection("TableConnectionString:Credential").Value;
            if (IsManagedIdentity(storageAccountCredential))
            {
                var tableServiceUri = new Uri(Configuration.GetSection("TableConnectionString:TableServiceUri").Value);
                Console.WriteLine($"Startup.TableClient using Managed Identity with url {tableServiceUri}");
                return new AzureTableService(new TableClient(tableServiceUri, "importrequests", new DefaultAzureCredential()), mapperConfig.CreateMapper());
            }
            else
            {
                return new AzureTableService(new TableClient(Configuration.GetSection("TableConnectionString").Value, "importrequests"), mapperConfig.CreateMapper());
            }
        });

        builder.Services.AddSingleton<IAzureBlobService>(_ =>
        {
            var storageAccountCredential = Configuration.GetSection("BlobConnectionString:Credential").Value;
            if (IsManagedIdentity(storageAccountCredential))
            {
                var blobServiceUri = new Uri(Configuration.GetSection("BlobConnectionString:BlobServiceUri").Value);
                Console.WriteLine($"Startup.BlobClient using Managed Identity with url {blobServiceUri}");
                return new AzureBlobService(new BlobServiceClient(blobServiceUri, new DefaultAzureCredential()));
            }
            else
            {
                return new AzureBlobService(new BlobServiceClient(Configuration.GetSection("BlobConnectionString").Value));
            }
        });

        builder.Services.AddSingleton<IImporterFunctions, ImporterFunctions>();
        builder.Services.AddSingleton<IUploadFunctions, UploadFunctions>();

        builder.Services.AddSingleton<IBlobService, BlobService>();
    }

    private static bool IsManagedIdentity(string credentialName)
    {
        return credentialName != null && credentialName.ToLower() == "managedidentity";
    }
}
