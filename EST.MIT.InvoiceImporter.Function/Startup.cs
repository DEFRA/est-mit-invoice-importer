using EST.MIT.Importer.Function.Services;
using EST.MIT.InvoiceImporter.Function.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;

[assembly: FunctionsStartup(typeof(Startup.Function.Startup))]

namespace Startup.Function
{
    public class Startup : FunctionsStartup
    {
        [ExcludeFromCodeCoverage]
        public override void Configure(IFunctionsHostBuilder builder)
        {
            Console.WriteLine("Configuring service...");

            builder.Services.AddSingleton<IAzureBlobService, AzureBlobService>();

            builder.Services.AddSingleton<IImporter, Importer>();

            builder.Services.AddSingleton<IBlobService, BlobService>();
        }
    }
}
