using Azure.Storage.Blobs;
using EST.MIT.InvoiceImporter.Function.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace EST.MIT.InvoiceImporter.Function.Services;

[ExcludeFromCodeCoverage]
public class AzureBlobService : IAzureBlobService
{
    private readonly IConfiguration? _configuration;
    private readonly BlobServiceClient _blobServiceClient;


    public AzureBlobService(IConfiguration configuration)
    {
        _configuration = configuration;
        //_blobServiceClient = new BlobServiceClient(_configuration.GetConnectionString("PrimaryConnection"));
        BlobServiceClient = _blobServiceClient;
    }

    public BlobServiceClient? BlobServiceClient { get; set; }
}
