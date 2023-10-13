using Azure.Storage.Blobs;
using EST.MIT.InvoiceImporter.Function.Functions;
using EST.MIT.InvoiceImporter.Function.Interfaces;
using EST.MIT.InvoiceImporter.Function.Models;
using EST.MIT.InvoiceImporter.Function.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace EST.MIT.InvoiceImporter.Function.Test.Functions;

public class UploadFunctionsTests
{
    private readonly Mock<ILogger> _mockLogger;
    private readonly Mock<IBinder> _mockBinder;
    private readonly IConfiguration _configuration;
    private readonly UploadFunctions _uploadFunctions;
    private readonly Mock<IBlobService> _mockBlobService;
    private readonly Mock<IAzureTableService> _mockTableService;

    public UploadFunctionsTests()
    {
        _mockLogger = new Mock<ILogger>();
        _mockBinder = new Mock<IBinder>();
        _mockBlobService = new Mock<IBlobService>();
        _mockTableService = new Mock<IAzureTableService>();

        var mockConfig = new Mock<IConfiguration>();
        var mockConfigSection = new Mock<IConfigurationSection>();
        mockConfigSection.Setup(x => x.Value).Returns("some_text");
        mockConfig.Setup(x => x.GetSection(It.Is<string>(y => y == "ConnectionStrings:PrimaryConnection"))).Returns(mockConfigSection.Object);
        _configuration = mockConfig.Object;

        var mockBlobServiceClient = new Mock<BlobServiceClient>();
        var mockAzureBlobService = new Mock<IAzureBlobService>();

        var mockAzureTableService = new Mock<IAzureTableService>();
        mockAzureBlobService.Setup(x => x.GetBlobServiceClient()).Returns(mockBlobServiceClient.Object);

        var mockBlobService = new Mock<IBlobService>();

        _uploadFunctions = new UploadFunctions(_mockTableService.Object);
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
}
