using System;
using Azure;
using Azure.Data.Tables;
using EST.MIT.InvoiceImporter.Function.Models;

namespace EST.MIT.InvoiceImporter.Function.TableEntities;

public class ImportRequestEntity : ITableEntity
{
    public const string DefaultPartitionKey = "mit-import-request";

    public string PartitionKey { get; set; } = DefaultPartitionKey;
    public string RowKey { get; set; } = null!;
    public string FileName { get; set; }
    public int FileSize { get; set; }
    public string FileType { get; set; }
    public DateTimeOffset? Timestamp { get; set; } = DateTimeOffset.Now;
    public string PaymentType { get; set; }
    public string Organisation { get; set; }
    public string SchemeType { get; set; }
    public string AccountType { get; set; }
    public string CreatedBy { get; init; }
    public UploadStatus Status { get; init; }
    public string BlobFileName { get; set; }
    public string BlobFolder { get; set; }

    public ETag ETag { get; set; }

    public ImportRequestEntity()
    {
    }

    public ImportRequestEntity(ImportRequest importRequest) : this()
    {
        FileName = importRequest.FileName;
        FileSize = importRequest.FileSize;
        FileType = importRequest.FileType;
        Timestamp = importRequest.Timestamp;
        PaymentType = importRequest.PaymentType;
        Organisation = importRequest.Organisation;
        SchemeType = importRequest.SchemeType;
        AccountType = importRequest.AccountType;
        CreatedBy = importRequest.CreatedBy;
        Status = importRequest.Status;
        BlobFileName = importRequest.BlobFileName;
        BlobFolder = importRequest.BlobFolder;

        PartitionKey = DefaultPartitionKey;
        RowKey = importRequest.ImportRequestId.ToString();
    }
}
