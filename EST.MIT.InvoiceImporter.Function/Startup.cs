using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using InvoiceImporter.Function.Models;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.Xml.Linq;
using EST.MIT.InvoiceImporter.Function.Services;
using EST.MIT.InvoiceImporter.Function;
using EST.MIT.Importer.Function.Services;

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
