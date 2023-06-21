using Azure.Storage.Blobs;

namespace EST.MIT.InvoiceImporter.Function.Interfaces;

public interface IAzureBlobService
{
    BlobServiceClient? BlobServiceClient { get; set; }
}