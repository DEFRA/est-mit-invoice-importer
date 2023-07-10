using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EST.MIT.InvoiceImporter.Function.Models;

public class ExcelInvoice
{
    public string Id { get; set; } = default!;
    public string InvoiceType { get; set; } = default!;
    public string AccountType { get; set; } = default!;
    public string Organisation { get; set; } = default!;
    public string SchemeType { get; set; } = default!;
    public List<ExcelHeader> Headers { get; set; }
}
