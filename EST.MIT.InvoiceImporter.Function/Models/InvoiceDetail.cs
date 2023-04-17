namespace EST.MIT.InvoiceImporter.Function.Models;

public class InvoiceDetail
{
    public string InvoiceId { get; set; }
    //public string ClaimReferenceNumber { get; set; }
    //public string ClaimReference { get; set; }
    public decimal Amount { get; set; }
    public string PreferredCurrency { get; set; }
    public string Fund { get; set; }
    public string MainAccount { get; set; }
    public string Scheme { get; set; }
    public string MarketingYear { get; set; }
    public string DeliveryBodyCode { get; set; }
    public string LineDescription { get; set; }
}
