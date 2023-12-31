﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EST.MIT.InvoiceImporter.Function.Models;

public class PaymentRequestsBatch
{
    [Required]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    [Required(ErrorMessage = "Account Type is required")]
    public string AccountType { get; set; } = default!;
    [Required(ErrorMessage = "Organisation is required")]
    public string Organisation { get; set; } = default!;
    [Required(ErrorMessage = "Scheme Type is required")]
    public string SchemeType { get; set; } = default!;
    public string PaymentType { get; set; } = default!;
    public List<PaymentRequest> PaymentRequests { get; set; } = new List<PaymentRequest>();
    public string Status { get; set; } = "new";
    public string Reference { get; set; } = default!;
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset Updated { get; set; }
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(this, new ValidationContext(this), results, validateAllProperties: true);
        return results;
    }
}