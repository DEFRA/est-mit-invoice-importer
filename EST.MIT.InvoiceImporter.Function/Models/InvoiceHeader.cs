using System.Collections.Generic;

namespace EST.MIT.InvoiceImporter.Function.Models;

public class InvoiceHeader
{
    public string InvoiceId { get; set; }
    public string ClaimReferenceNumber { get; set; }
    public string ClaimReference { get; set; }
    public string CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
    public string PreferredCurrency { get; set; }
    public string Description { get; set; }

    public List<InvoiceDetail> InvoiceDetails { get; set; }
}
