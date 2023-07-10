using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InvoiceImporter.Function.Models;

public class InputData
{
    [Required]
    public string InvoiceType { get; set; } = default!;
    [Required(ErrorMessage = "Account Type is required")]
    public string AccountType { get; set; } = default!;
    [Required(ErrorMessage = "Organisation is required")]
    public string Organisation { get; set; } = default!;
    [Required(ErrorMessage = "Scheme Type is required")]
    public string SchemeType { get; set; } = default!;
    public string Status { get; set; } = "new";
    public string Reference { get; set; } = default!;
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset Updated { get; set; }
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }
    public string SourceSystem { get; set; }
    public int FRN { get; set; }
    public int MarketingYear { get; set; }
    public int PaymentRequestNumber { get; set; }
    public string AgreementNumber { get; set; }
    public string Currency { get; set; }
    public string DueDate { get; set; }
    public double PaymentRequestValue { get; set; }
    public double LineValue { get; set; }
    public string Description { get; set; }
    public string SchemeCode { get; set; }

    public string DeliveryBody { get; set; }

    public InputData()
    {

    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(this, new ValidationContext(this), results, validateAllProperties: true);
        return results;
    }
}

public sealed class InputDataMap : ClassMap<InputData>
{
    public InputDataMap()
    {
        Map(m => m.InvoiceType).Name("InvoiceType");
        Map(m => m.AccountType).Name("AccountType");
        Map(m => m.Organisation).Name("Organisation");
        Map(m => m.SchemeType).Name("SchemeType");
        Map(m => m.Reference).Name("Reference");
        Map(m => m.Created).Name("Created").TypeConverterOption.Format("dd/MM/yyyy HH:mm");
        Map(m => m.Updated).Name("Updated").TypeConverterOption.Format("dd/MM/yyyy HH:mm");
        Map(m => m.CreatedBy).Name("CreatedBy");
        Map(m => m.UpdatedBy).Name("UpdatedBy");
        Map(m => m.SourceSystem).Name("SourceSystem");
        Map(m => m.FRN).Name("FRN");
        Map(m => m.MarketingYear).Name("MarketingYear");
        Map(m => m.PaymentRequestNumber).Name("PaymentRequestNumber");
        Map(m => m.AgreementNumber).Name("AgreementNumber");
        Map(m => m.Currency).Name("Currency");
        Map(m => m.DueDate).Name("DueDate");
        Map(m => m.PaymentRequestValue).Name("PaymentRequestValue");
        Map(m => m.LineValue).Name("LineValue");
        Map(m => m.Description).Name("Description");
        Map(m => m.SchemeCode).Name("SchemeCode");
        Map(m => m.DeliveryBody).Name("DeliveryBody");
    }
}
