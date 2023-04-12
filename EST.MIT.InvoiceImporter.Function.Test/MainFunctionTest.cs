using Azure.Storage.Blobs;
using EST.MIT.Importer.Function.Services;
using EST.MIT.InvoiceImporter.Function.Services;
using InvoiceImporter.Function.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace InvoiceImporter.Function.Tests
{
    public class ImporterTests
    {
        private readonly Mock<ILogger> _mockLogger;
        private readonly Mock<IBinder> _mockBinder;
        private readonly ImportRequest _importRequest;
        private IConfiguration _configuration;

        public ImporterTests()
        {
            _mockLogger = new Mock<ILogger>();
            _mockBinder = new Mock<IBinder>();
            _importRequest = new ImportRequest { FileName = "12345", FileSize = 12345, FileType = "xlsx", Timestamp = DateTimeOffset.UtcNow };



            var mockConfig = new Mock<IConfiguration>();

            var mockConfigSection = new Mock<IConfigurationSection>();
            mockConfigSection.Setup(x => x.Value).Returns("some_text");
            mockConfig.Setup(x => x.GetSection(It.Is<string>(y => y == "StorageConnectionString"))).Returns(mockConfigSection.Object);


            _configuration = mockConfig.Object;
        }

        //[Fact]
        //public async Task QueueTrigger_No_Import_Request_Error()
        //{
        //    // Arrange
        //    const string expectedErrorMsg = "No import request received.";
        //    Environment.SetEnvironmentVariable("StorageConnectionString", "UseDevelopmentStorage=true");
        //    Importer importer = new();

        //    // Act
        //    await importer.QueueTrigger(null, _mockBinder.Object, _mockLogger.Object);

        //    // Assert
        //    _mockBinder.Verify(b => b.BindAsync<string>(It.IsAny<BlobAttribute>(), CancellationToken.None), Times.Never);
        //    _mockLogger.Verify(l => l.Log(LogLevel.Error, It.IsAny<EventId>(), It.Is<It.IsAnyType>((o, t) => string.Equals(expectedErrorMsg, o.ToString(), StringComparison.InvariantCulture)), It.IsAny<Exception>(), (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);
        //}

        [Fact]
        public async void QueueTrigger_Valid_Request()
        {
            

            // Arrange
            var mockBlobService = new Mock<IBlobService>();
            mockBlobService.Setup(x => x.ReadBLOBIntoStream(It.IsAny<string>(), new Mock<ILogger>().Object, It.IsAny<IBinder>())).ReturnsAsync(new Mock<Stream>().Object);
            mockBlobService.Setup(x => x.GetFileName()).Returns("testfile.csv");
            mockBlobService.Setup(x => x.MoveFileToArchive(It.IsAny<string>(), new Mock<ILogger>().Object, It.IsAny<BlobServiceClient>())).ReturnsAsync(true);
            Environment.SetEnvironmentVariable("StorageConnectionString", "UseDevelopmentStorage=true");
            var importer = new Importer(mockBlobService.Object, _configuration);
            
            // Act
            await importer.QueueTrigger("some text", _mockBinder.Object, _mockLogger.Object);

            // Assert
            _mockBinder.Verify(b => b.BindAsync<string>(It.IsAny<BlobAttribute>(), CancellationToken.None), Times.Never);
        }


        //[Fact]
        //public async Task QueueTrigger_Invalid_Import_Request_Error()
        //{
        //    // Arrange
        //    const string expectedErrorMsg = "Invalid import request received.";
        //    const string queueMessage = "Test Message";
        //    Environment.SetEnvironmentVariable("StorageConnectionString", "UseDevelopmentStorage=true");
        //    Importer importer = new();

        //    // Act
        //    await importer.QueueTrigger(queueMessage, _mockBinder.Object, _mockLogger.Object);

        //    // Assert
        //    _mockBinder.Verify(b => b.BindAsync<string>(It.IsAny<BlobAttribute>(), CancellationToken.None), Times.Never);
        //    _mockLogger.Verify(l => l.Log(LogLevel.Error, It.IsAny<EventId>(), It.Is<It.IsAnyType>((o, t) => string.Equals(expectedErrorMsg, o.ToString(), StringComparison.InvariantCulture)), It.IsAny<Exception>(), (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);
        //}
    }
}