using InvoiceImporter.Function.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace InvoiceImporter.Function.Service;

public interface IInvoiceParser
{
    Task<List<Invoice>> GetInvoicesAsync(Stream stream, ILogger log);
}
