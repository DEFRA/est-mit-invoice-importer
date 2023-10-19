using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using System.IO;
using System.Threading.Tasks;

namespace EST.MIT.InvoiceImporter.Function.Interfaces;

public interface IAzureBlobService
{
    BlobServiceClient? GetBlobServiceClient();
    Task<Stream> ReadBLOBIntoStream(string importMsg, IBinder blobBinder);
    string GetFileName();
    Task<bool> MoveFileToArchive(string fileName, BlobServiceClient blobServiceClient);
    Task<Stream> GetFileByFileNameAsync(string fileName);
}