using EST.MIT.InvoiceImporter.Function.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using System.Reflection;
using EST.MIT.InvoiceImporter.Function.DataAccess;
using Azure.Storage.Blobs;

namespace EST.MIT.InvoiceImporter.Function.Services.Tests;
public class AzureBlobServiceTests
{
    [Fact]
    public async Task ReadBLOBIntoStream_WithValidImportRequest_ReturnsStream()
    {
        var importRequest = new ImportRequest { FileName = "test-file.txt" };
        var importRequestJson = JsonConvert.SerializeObject(importRequest);
        var blobBinder = new Mock<IBinder>();
        var blobStream = new MemoryStream();
        blobBinder.Setup(b => b.BindAsync<Stream>(It.IsAny<BlobAttribute>(), CancellationToken.None)).ReturnsAsync(blobStream);
        var logger = new Mock<ILogger<AzureBlobService>>();
        var blobServiceClientMock = new Mock<BlobServiceClient>();
        var blobService = new AzureBlobService(blobServiceClientMock.Object, logger.Object, AzureBlobService.default_BlobContainerName);

        var result = await blobService.ReadBLOBIntoStream(importRequestJson, blobBinder.Object);

        Assert.NotNull(result);
        Assert.IsType<MemoryStream>(result);
    }

    [Fact]
    public async Task ReadBLOBIntoStream_WithInvalidImportRequest_ReturnsNull()
    {
        var importRequestJson = "invalid-json";
        var blobBinder = new Mock<IBinder>();
        var logger = new Mock<ILogger<AzureBlobService>>();
        var blobServiceClientMock = new Mock<BlobServiceClient>();
        var blobService = new AzureBlobService(blobServiceClientMock.Object, logger.Object, AzureBlobService.default_BlobContainerName);
        var result = await blobService.ReadBLOBIntoStream(importRequestJson, blobBinder.Object);

        Assert.Null(result);
    }

    [Fact]
    public async Task ReadBLOBIntoStream_WithNullImportRequest_ReturnsNull()
    {
        string? importRequestJson = null;
        var blobBinder = new Mock<IBinder>();
        var logger = new Mock<ILogger<AzureBlobService>>();
        var blobServiceClientMock = new Mock<BlobServiceClient>();
        var blobService = new AzureBlobService(blobServiceClientMock.Object, logger.Object, AzureBlobService.default_BlobContainerName);

        var result = await blobService.ReadBLOBIntoStream(importRequestJson, blobBinder.Object);

        Assert.Null(result);
    }

    [Fact]
    public void GetFileName_ReturnsFileName()
    {
        var logger = new Mock<ILogger<AzureBlobService>>();
        var expectedFileName = "test-file.xlsx";
        var blobServiceClientMock = new Mock<BlobServiceClient>();
        var blobService = new AzureBlobService(blobServiceClientMock.Object, logger.Object, AzureBlobService.default_BlobContainerName);

        blobService.GetType().GetField("_fileName", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(blobService, expectedFileName);

        var result = blobService.GetFileName();

        Assert.Equal(expectedFileName, result);
    }
}