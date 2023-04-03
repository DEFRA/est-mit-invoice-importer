using EST.MIT.Importer.Function.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

[assembly: FunctionsStartup(typeof(Startup.Function.Startup))]

namespace Startup.Function
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            Console.WriteLine("Configuring service...");
            builder.Services.AddSingleton<IImporter>((s) =>
            {
                return new Importer();
            });
        }
    }
}
