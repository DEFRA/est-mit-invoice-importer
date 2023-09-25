using AutoMapper;
using EST.MIT.Importer.Function.Interfaces;
using EST.MIT.InvoiceImporter.Function.AutoMapperProfiles;
using EST.MIT.InvoiceImporter.Function.Configuration;
using EST.MIT.InvoiceImporter.Function.Functions;
using EST.MIT.InvoiceImporter.Function.Interfaces;
using EST.MIT.InvoiceImporter.Function.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;

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

        builder.Services.AddSingleton<IAzureBlobService, AzureBlobService>();

        builder.Services.AddSingleton<IImporter, Importer>();
        builder.Services.AddSingleton<IUploadFunctions, UploadFunctions>();

        builder.Services.AddSingleton<IBlobService, BlobService>();
        builder.Services.AddSingleton<IEventQueueService, EventQueueService>();

        var mapperConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new ImportRequestMapper());
        });

        var storageConnection = Configuration["Storage:ConnectionString"];
        var eventQueueName = Configuration["Storage:EventQueueName"];
        builder.Services.RegisterServices(storageConnection, eventQueueName);
    }
}