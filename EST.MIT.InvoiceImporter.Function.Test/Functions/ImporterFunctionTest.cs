using Azure.Storage.Blobs;
using EST.MIT.InvoiceImporter.Function.Functions;
using EST.MIT.InvoiceImporter.Function.Interfaces;
using EST.MIT.InvoiceImporter.Function.Models;
using EST.MIT.InvoiceImporter.Function.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace InvoiceImporter.Function.Functions.Tests;
public class ImporterTests
{
    private readonly Mock<ILogger> _mockLogger;
    private readonly Mock<IBinder> _mockBinder;
    private readonly IConfiguration _configuration;
    private readonly ImporterFunctions _importer;
    private readonly Mock<IBlobService> _mockBlobService;
    private readonly Mock<IAzureTableService> _mockTableService;

    public ImporterTests()
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

        _importer = new ImporterFunctions(Mock.Of<IBlobService>(), mockAzureBlobService.Object, _mockTableService.Object);

    }

    [Fact]
    public async void QueueTrigger_Valid_Request()
    {
        _mockBlobService.Setup(x => x.ReadBLOBIntoStream(It.IsAny<string>(), It.IsAny<IBinder>())).ReturnsAsync(new Mock<Stream>().Object);
        _mockBlobService.Setup(x => x.GetFileName()).Returns("testfile.csv");
        _mockBlobService.Setup(x => x.MoveFileToArchive(It.IsAny<string>(), It.IsAny<BlobServiceClient>())).ReturnsAsync(true);

        await _importer.QueueTrigger("some text", _mockBinder.Object, _mockLogger.Object);

        _mockBinder.Verify(b => b.BindAsync<string>(It.IsAny<BlobAttribute>(), CancellationToken.None), Times.Never);
    }

    [Fact]
    public async Task QueueTrigger_InvalidJson_LogsError()
    {
        await _importer.QueueTrigger("invalid json", _mockBinder.Object, _mockLogger.Object);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                null,
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task QueueTrigger_SuccessfulFileMove_LogsInformation()
    {
        _mockBlobService.Setup(x => x.ReadBLOBIntoStream(It.IsAny<string>(), It.IsAny<IBinder>()))
            .ReturnsAsync(new MemoryStream());
        _mockBlobService.Setup(x => x.GetFileName())
            .Returns("testfile.csv");
        _mockBlobService.Setup(x => x.MoveFileToArchive(It.IsAny<string>(), It.IsAny<BlobServiceClient>()))
            .ReturnsAsync(true);

        await _importer.QueueTrigger("some valid json", _mockBinder.Object, _mockLogger.Object);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                null,
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
            Times.AtLeastOnce);
    }

}