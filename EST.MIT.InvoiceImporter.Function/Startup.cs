using AutoMapper;
using EST.MIT.Importer.Function.Interfaces;
using EST.MIT.InvoiceImporter.Function.AutoMapperProfiles;
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


        var mapperConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new ImportRequestMapper());
        });

        builder.Services.AddTableBlobQueueServices(Configuration, mapperConfig);

        builder.Services.AddSingleton<IImporterFunctions, ImporterFunctions>();
        builder.Services.AddSingleton<IUploadFunctions, UploadFunctions>();
        builder.Services.AddSingleton<INotificationService>(_ => new NotificationService(Configuration));
    }
}
