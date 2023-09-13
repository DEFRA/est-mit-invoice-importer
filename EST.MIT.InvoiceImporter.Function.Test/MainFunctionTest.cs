using Azure.Storage.Blobs;
using EST.MIT.InvoiceImporter.Function.Interfaces;
using EST.MIT.InvoiceImporter.Function.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace InvoiceImporter.Function.Tests;
public class ImporterTests
{
    private readonly Mock<ILogger> _mockLogger;
    private readonly Mock<IBinder> _mockBinder;
    private readonly IConfiguration _configuration;
    private readonly Importer _importer;
    private readonly Mock<IBlobService> _mockBlobService;

    public ImporterTests()
    {
        _mockLogger = new Mock<ILogger>();
        _mockBinder = new Mock<IBinder>();
        _mockBlobService = new Mock<IBlobService>();

        var mockConfig = new Mock<IConfiguration>();
        var mockConfigSection = new Mock<IConfigurationSection>();
        mockConfigSection.Setup(x => x.Value).Returns("some_text");
        mockConfig.Setup(x => x.GetSection(It.Is<string>(y => y == "ConnectionStrings:PrimaryConnection"))).Returns(mockConfigSection.Object);
        _configuration = mockConfig.Object;

        var mockBlobServiceClient = new Mock<BlobServiceClient>();
        var mockAzureBlobService = new Mock<IAzureBlobService>();
        mockAzureBlobService.Setup(x => x.BlobServiceClient).Returns(mockBlobServiceClient.Object);

        var mockBlobService = new Mock<IBlobService>();

        _importer = new Importer(Mock.Of<IBlobService>(), _configuration, mockAzureBlobService.Object);
    }

    [Fact]
    public async void QueueTrigger_Valid_Request()
    {

        // Arrange
        _mockBlobService.Setup(x => x.ReadBLOBIntoStream(It.IsAny<string>(), It.IsAny<IBinder>())).ReturnsAsync(new Mock<Stream>().Object);
        _mockBlobService.Setup(x => x.GetFileName()).Returns("testfile.csv");
        _mockBlobService.Setup(x => x.MoveFileToArchive(It.IsAny<string>(), It.IsAny<BlobServiceClient>())).ReturnsAsync(true);

        // Act 
        await _importer.QueueTrigger("some text", _mockBinder.Object, _mockLogger.Object);

        // Assert
        _mockBinder.Verify(b => b.BindAsync<string>(It.IsAny<BlobAttribute>(), CancellationToken.None), Times.Never);
    }
}