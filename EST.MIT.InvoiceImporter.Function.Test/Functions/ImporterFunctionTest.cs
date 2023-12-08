using Azure.Storage.Blobs;
using EST.MIT.InvoiceImporter.Function.Functions;
using EST.MIT.InvoiceImporter.Function.Interfaces;
using EST.MIT.InvoiceImporter.Function.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace InvoiceImporter.Function.Functions.Tests;
public class ImporterTests
{
    private readonly Mock<ILogger> _mockLogger;
    private readonly Mock<IBinder> _mockBinder;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly ImporterFunctions _importer;
    private readonly Mock<IAzureBlobService> _mockBlobService;
    private readonly Mock<IAzureTableService> _mockTableService;
    private readonly Mock<INotificationQueueService> _mockNotificationQueueService;

    public ImporterTests()
    {
        _mockLogger = new Mock<ILogger>();
        _mockBinder = new Mock<IBinder>();
        _mockConfiguration = new Mock<IConfiguration>();
        _mockBlobService = new Mock<IAzureBlobService>();
        _mockTableService = new Mock<IAzureTableService>();
        _mockNotificationQueueService = new Mock<INotificationQueueService>();

        var fakeBaseUrl = "http://mit.defra.gov.uk/";
        var mockWebUIBaseUrlSection = new Mock<IConfigurationSection>();
        mockWebUIBaseUrlSection.SetupGet(m => m.Value).Returns(fakeBaseUrl);
        _mockConfiguration.Setup(m => m.GetSection("WebUIBaseUrl")).Returns(mockWebUIBaseUrlSection.Object);
        _mockConfiguration.Setup(m => m.GetSection("WebUIUserUploadsUrl")).Returns(mockWebUIBaseUrlSection.Object);

        var mockBlobServiceClient = new Mock<BlobServiceClient>();
        _mockBlobService.Setup(x => x.GetBlobServiceClient()).Returns(mockBlobServiceClient.Object);

        var mockBlobService = new Mock<IAzureBlobService>();

        _importer = new ImporterFunctions(_mockBlobService.Object, _mockTableService.Object, _mockNotificationQueueService.Object, _mockConfiguration.Object);

    }

    [Fact]
    public async void QueueTrigger_Valid_Request()
    {
        _mockBlobService.Setup(x => x.ReadBLOBIntoStream(It.IsAny<string>(), It.IsAny<IBinder>())).ReturnsAsync(new Mock<Stream>().Object);
        _mockBlobService.Setup(x => x.GetFileName()).Returns("testfile.csv");

        _mockBlobService.Setup(x => x.MoveFileToArchive(It.IsAny<string>(), It.IsAny<BlobServiceClient>())).ReturnsAsync(true);
        _mockNotificationQueueService.Setup(x => x.AddMessageToQueueAsync(It.IsAny<Notification>())).ReturnsAsync(true);

        await _importer.QueueTrigger("{\"importRequestId\":\"578ecf14-8ccf-4639-ace0-8f5905f8049f\",\"fileName\":\"TestFile.txt\",\"SchemeType\":\"AZ\",\"Email\":\"email@defra.gov.uk\",\"Status\":\"Uploaded\"}", _mockBinder.Object, _mockLogger.Object);

        _mockBinder.Verify(b => b.BindAsync<string>(It.IsAny<BlobAttribute>(), CancellationToken.None), Times.Never);
    }

    [Fact]
    public async Task QueueTrigger_InvalidJson_LogsError()
    {
        await _importer.QueueTrigger("{\"importRequestId\":\"578ecf14-8ccf-4639-ace0-8f5905f8049f\",\"fileName\":\"TestFile.txt\",\"SchemeType\":\"AZ\",\"Email\":\"email@defra.gov.uk\",\"Status\":\"Uploaded\"}", _mockBinder.Object, _mockLogger.Object);

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

        await _importer.QueueTrigger("{\"importRequestId\":\"578ecf14-8ccf-4639-ace0-8f5905f8049f\",\"fileName\":\"TestFile.txt\",\"SchemeType\":\"AZ\",\"Email\":\"email@defra.gov.uk\",\"Status\":\"Uploaded\"}", _mockBinder.Object, _mockLogger.Object);

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
    public async Task QueueTrigger_FailedFileMove_LogsWarning()
    {
        _mockBlobService.Setup(x => x.ReadBLOBIntoStream(It.IsAny<string>(), It.IsAny<IBinder>()))
            .ReturnsAsync(new MemoryStream());
        _mockBlobService.Setup(x => x.GetFileName())
            .Returns("testfile.csv");
        _mockBlobService.Setup(x => x.MoveFileToArchive(It.IsAny<string>(), It.IsAny<BlobServiceClient>()))
            .ReturnsAsync(false);

        await _importer.QueueTrigger("{\"importRequestId\":\"578ecf14-8ccf-4639-ace0-8f5905f8049f\",\"fileName\":\"TestFile.txt\",\"SchemeType\":\"AZ\",\"Email\":\"email@defra.gov.uk\",\"Status\":\"Uploaded\"}", _mockBinder.Object, _mockLogger.Object);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                null,
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
            Times.AtLeastOnce);
    }
}