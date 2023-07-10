namespace EST.MIT.InvoiceImporter.Function.Models;

public class ExcelLine
{
    public string Value { get; set; }
    public string Currency { get; set; } = null!;
    public string FundCode { get; set; } = null!;
    public string MainAccount { get; set; } = null!;
    public string SchemeCode { get; set; } = null!;
    public string MarketingYear { get; init; }
    public string Description { get; set; } = null!;
}
