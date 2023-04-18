using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using InvoiceImporter.Function.Models;
using Microsoft.Extensions.Logging;

namespace InvoiceImporter.Function.Service;

public interface IInvoiceParser
{
    Task<List<Invoice>> TryParse(Stream reader, ILogger log);
}
