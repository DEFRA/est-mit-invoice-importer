﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EST.MIT.InvoiceImporter.Function.Models;

public class PaymentRequest
{
    public string PaymentRequestId { get; set; } = Guid.NewGuid().ToString();
    [Required]
    public string SourceSystem { get; set; } = "Manual";
    [Required]
    [RegularExpression("(\\d{10})", ErrorMessage = "The FRN must be 10 digits")]
    public string ClaimReferenceNumber { get; set; }
    public string ClaimReference { get; set; }
    public long FRN { get; set; }
    [Range(2014, int.MaxValue, ErrorMessage = "The Marketing Year must be after than 2014")]
    public int MarketingYear { get; set; }
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "The Payment Request Number must be greater than 0")]
    public int PaymentRequestNumber { get; set; }
    [Required(ErrorMessage = "The Agreement Number is required")]
    public string AgreementNumber { get; set; } = string.Empty;
    [Required]
    [RegularExpression("GBP|EUR", ErrorMessage = "The Currency must be either GBP or EUR")]
    public string Currency { get; set; } = "GBP";
    public string Description { get; set; }
    public string OriginalInvoiceNumber { get; set; }
    public DateTime OriginalSettlementDate { get; set; }
    public DateTime RecoveryDate { get; set; }
    public string InvoiceCorrectionReference { get; set; }
    public string DueDate { get; set; } = string.Empty;
    [Required]
    [RegularExpression("(^(0|\\d+\\.\\d{2})$)", ErrorMessage = "The Value must be in the format 0.00")]
    public decimal Value { get; set; } = 0.00M;
    public List<InvoiceLine> InvoiceLines { get; set; } = new List<InvoiceLine>();
    public int SBI { get; set; }
    public string Vendor { get; set; }

    public PaymentRequest()
    {
        InvoiceLines = new List<InvoiceLine>();
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(this, new ValidationContext(this), results, validateAllProperties: true);
        return results;
    }
}