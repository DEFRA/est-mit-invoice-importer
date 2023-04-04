using System.Collections.Generic;
using System.IO;
using EST.MIT.InvoiceImporter.Function.Models;
using Microsoft.Extensions.Logging;

namespace EST.MIT.InvoiceImporter.Function.Services;

public interface IInvoiceParser
{
    List<T> Parse<T>(Stream blobStream, string invoiceAccountType, string invoiceItemType, ILogger log) where T : new();
}
