using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EST.MIT.InvoiceImporter.Function.Models;

public class ExcelHeader
{
    public string PaymentRequestId { get; init; } = default!;
    public string AgreementNumber { get; init; } = default!;
    public string ClaimRef { get; init; } = default!;
    public string FRN { get; init; }
    public string PaymentRequestNumber { get; init; }
    public string ContractNumber { get; init; } = default!;
    public string Value { get; init; }
    public string DeliveryBody { get; init; } = default!;
    public string DueDate { get; init; } = default!;
    public string RecoveryDate { get; init; } = default!;
    public string Description { get; init; } = default!;
    public List<ExcelLine> Lines { get; set; }
}
