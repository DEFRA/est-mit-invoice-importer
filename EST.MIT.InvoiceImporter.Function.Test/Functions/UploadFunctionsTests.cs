using Azure.Storage.Blobs;
using EST.MIT.InvoiceImporter.Function.Functions;
using EST.MIT.InvoiceImporter.Function.Interfaces;
using EST.MIT.InvoiceImporter.Function.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace EST.MIT.InvoiceImporter.Function.Test.Functions;

public class UploadFunctionsTests
{
    private readonly Mock<ILogger> _mockLogger;
    private readonly UploadFunctions _uploadFunctions;
    private readonly Mock<IAzureBlobService> _mockBlobService;
    private readonly Mock<IAzureTableService> _mockTableService;

    public UploadFunctionsTests()
    {
        _mockLogger = new Mock<ILogger>();
        _mockBlobService = new Mock<IAzureBlobService>();
        _mockTableService = new Mock<IAzureTableService>();

        var mockBlobServiceClient = new Mock<BlobServiceClient>();
        var mockAzureBlobService = new Mock<IAzureBlobService>();

        var mockAzureTableService = new Mock<IAzureTableService>();
        mockAzureBlobService.Setup(x => x.GetBlobServiceClient()).Returns(mockBlobServiceClient.Object);

        var mockBlobService = new Mock<IAzureBlobService>();

        _uploadFunctions = new UploadFunctions(_mockTableService.Object, _mockBlobService.Object);
    }

    [Fact]
    public async Task GetUploadsByUser_ReturnsOkObjectResult_WhenCalledWithValidUserId()
    {
        string userId = "testUser";
        var importRequests = new List<ImportRequest>
        {
            new ImportRequest(),
            new ImportRequest()
        };
        _mockTableService.Setup(s => s.GetUserImportRequestsAsync(userId))
            .ReturnsAsync(importRequests);

        var result = await _uploadFunctions.GetUploadsByUser(null, userId, _mockLogger.Object);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetUploadsByUser_ReturnsInternalServerError_WhenExceptionIsThrown()
    {
        string userId = "testUser";
        _mockTableService.Setup(s => s.GetUserImportRequestsAsync(userId))
            .ThrowsAsync(new Exception());

        var result = await _uploadFunctions.GetUploadsByUser(null, userId, _mockLogger.Object);

        Assert.IsType<StatusCodeResult>(result);
        var statusCodeResult = result as StatusCodeResult;
        Assert.Equal((int)HttpStatusCode.InternalServerError, statusCodeResult.StatusCode);
    }

    [Fact]
    public async Task GetUploadedFile_ReturnsFileStreamResult_WhenFileExists()
    {
        string refId = "testRef";
        var mockImportRequest = new ImportRequest { FileName = "test-file.txt", BlobFileName="BlobFileName", BlobFolder="BlobFolder" };
        var mockStream = new MemoryStream();

        _mockTableService.Setup(s => s.GetUserImportRequestsByImportRequestIdAsync(refId))
            .ReturnsAsync(mockImportRequest);
        _mockBlobService.Setup(s => s.GetFileByFileNameAsync($"{ mockImportRequest.BlobFolder}/{mockImportRequest.BlobFileName}"))
            .ReturnsAsync(mockStream);

        var result = await _uploadFunctions.GetUploadedFile(null, refId, _mockLogger.Object);

        Assert.IsType<FileStreamResult>(result);
        var fileResult = result as FileStreamResult;
        Assert.Equal("application/octet-stream", fileResult.ContentType);
        Assert.Equal(mockImportRequest.FileName, fileResult.FileDownloadName);
    }

    [Fact]
    public async Task GetUploadedFile_ReturnsNotFoundResult_WhenFileDoesNotExist()
    {
        string refId = "testRef";

        _mockTableService.Setup(s => s.GetUserImportRequestsByImportRequestIdAsync(refId))
            .ReturnsAsync((ImportRequest?)null);

        var result = await _uploadFunctions.GetUploadedFile(null, refId, _mockLogger.Object);

        Assert.IsType<NotFoundResult>(result);
    }
}
