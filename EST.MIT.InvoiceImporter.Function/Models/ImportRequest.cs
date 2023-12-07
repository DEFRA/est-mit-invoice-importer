using System;

namespace EST.MIT.InvoiceImporter.Function.Models;

public class ImportRequest
{
    public Guid ImportRequestId { get; set; }
    public string FileName { get; set; }
    public int FileSize { get; set; }
    public string FileType { get; set; }
    public DateTimeOffset? Timestamp { get; set; }

    public string PaymentType { get; set; }
    public string Organisation { get; set; }
    public string SchemeType { get; set; }
    public string AccountType { get; set; }
    public string Email { get; set; }
    public string CreatedBy { get; init; }
    public UploadStatus Status { get; set; }
    public string BlobFileName { get; set; }
    public string BlobFolder { get; set; }
}