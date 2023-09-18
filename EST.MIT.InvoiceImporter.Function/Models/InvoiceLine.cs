using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EST.MIT.InvoiceImporter.Function.Models;

public class InvoiceLine
{
    [Required]
    [RegularExpression("(^\\d+\\.\\d{2}$)", ErrorMessage = "The Value must be in the format 0.00")]
    public decimal Value { get; set; }
    public string Description { get; set; } = default!;
    public string FundCode { get; set; }
    public string MainAccount { get; set; }
    public string SchemeCode { get; set; } = default!;
    public int MarketingYear { get; set; }
    public string DeliveryBody { get; set; } = "RP00";

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(this, new ValidationContext(this), results, validateAllProperties: true);
        return results;
    }
}
