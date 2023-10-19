using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using EST.MIT.InvoiceImporter.Function.Interfaces;
using EST.MIT.InvoiceImporter.Function.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace EST.MIT.InvoiceImporter.Function.DataAccess;
public class AzureBlobService : IAzureBlobService
{
    private readonly BlobServiceClient _blobServiceClient;
    private string _fileName;
    private readonly ILogger<AzureBlobService> _logger;

    public AzureBlobService(BlobServiceClient blobServiceClient, ILogger<AzureBlobService> logger)
    {
        _blobServiceClient = blobServiceClient;
        _logger = logger;
    }

    public BlobServiceClient? GetBlobServiceClient()
    {
        return _blobServiceClient;
    }

    public async Task<Stream> ReadBLOBIntoStream(string importMsg, IBinder blobBinder)
    {
        Stream blobStream = null;
        if (importMsg == null)
        {
            _logger.LogError("No import request received.");
            return blobStream;
        }

        ImportRequest importRequest;
        try
        {
            importRequest = JsonConvert.DeserializeObject<ImportRequest>(importMsg);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Invalid import request received.");
            return blobStream;
        }

        var blobAttr = new BlobAttribute($"invoices/import/{importRequest.FileName}", FileAccess.Read)
        {
            Connection = "BlobConnectionString"
        };

        _fileName = importRequest.FileName;
        return await blobBinder.BindAsync<Stream>(blobAttr);
    }

    [ExcludeFromCodeCoverage]
    public async Task<bool> MoveFileToArchive(string fileName, BlobServiceClient blobServiceClient)
    {
        try
        {
            var containerClient = blobServiceClient.GetBlobContainerClient("rpa-mit-invoices");
            var sourceBlobClient = containerClient.GetBlobClient($"import/{fileName}");
            var destBlobClient = containerClient.GetBlobClient($"archive/{fileName}");
            CopyFromUriOperation copyOperation = await destBlobClient.StartCopyFromUriAsync(sourceBlobClient.Uri);
            await copyOperation.WaitForCompletionAsync();
            await sourceBlobClient.DeleteIfExistsAsync();
            _logger.LogInformation($"File {fileName} moved to archive folder.");
            return true;
        }
        catch (RequestFailedException ex)
        {
            _logger.LogError($"An error occured when moving the file [{fileName}] to the archive folder.");
            _logger.LogError(ex.ErrorCode);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occured when moving the file [{fileName}] to the archive folder.");
            _logger.LogError(ex.Message);
            return false;
        }
    }

    public virtual string GetFileName()
    {
        return _fileName;
    }

    public async Task<Stream> GetFileByFileNameAsync(string fileName)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient("rpa-mit-invoices");
        var blobClient = blobContainerClient.GetBlobClient($"import/{fileName}");

        if (!await blobClient.ExistsAsync())
        {
            return null;
        }

        return await blobClient.OpenReadAsync();
    }
}
