using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace AzureServices;

public interface IAzureBlobService
{
    BlobServiceClient? BlobServiceClient { get; set; }
}


[ExcludeFromCodeCoverage]
public class AzureBlobService : IAzureBlobService
{ 
    private readonly IConfiguration? _configuration;
    private readonly BlobServiceClient _blobServiceClient;


    public AzureBlobService(IConfiguration configuration)
    {
        _configuration = configuration;
        _blobServiceClient = new BlobServiceClient(_configuration.GetConnectionString("PrimaryConnection"));
        BlobServiceClient = _blobServiceClient;
    }

    public BlobServiceClient? BlobServiceClient { get; set; }

}