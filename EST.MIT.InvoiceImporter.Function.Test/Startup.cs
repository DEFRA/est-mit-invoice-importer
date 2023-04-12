using Azure.Storage.Blobs;
using EST.MIT.Importer.Function.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

[assembly: FunctionsStartup(typeof(Startup.Function.TestStartup))]

namespace Startup.Function
{
    public class TestStartup : FunctionsStartup
    {
        [ExcludeFromCodeCoverage]
        public override void Configure(IFunctionsHostBuilder builder)
        {
            Console.WriteLine("Configuring service...");
            builder.Services.AddSingleton<IImporter, Importer>();

            builder.Services.AddSingleton<BlobServiceClient>((bsc) =>
            {
                return new BlobServiceClient(Environment.GetEnvironmentVariable("StorageConnectionString"));
            });
        }
    }
}
