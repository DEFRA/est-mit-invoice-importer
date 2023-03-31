using InvoiceImporter.Function.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using System.Text;
using EST.MIT.Importer.Function.Services;

namespace InvoiceImporter.Function.Tests
{
    public class ImporterTests
    {
        private readonly Mock<ILogger> _mockLogger;
        private readonly Mock<IBinder> _mockBinder;
        private readonly ImportRequest _importRequest;

        public ImporterTests()
        {
            _mockLogger = new Mock<ILogger>();
            _mockBinder = new Mock<IBinder>();
            _importRequest = new ImportRequest { FileName = "12345", FileSize = 12345, FileType = "xlsx", Timestamp = DateTimeOffset.UtcNow };
        }

        [Fact]
        public async Task QueueTrigger_No_Import_Request_Error()
        {
            // Arrange
            const string expectedErrorMsg = "No import request received.";
            Importer importer = new();

            // Act
            await importer.QueueTrigger(null, _mockBinder.Object, _mockLogger.Object);

            // Assert
            _mockBinder.Verify(b => b.BindAsync<string>(It.IsAny<BlobAttribute>(), CancellationToken.None), Times.Never);
            _mockLogger.Verify(l => l.Log(LogLevel.Error, It.IsAny<EventId>(), It.Is<It.IsAnyType>((o, t) => string.Equals(expectedErrorMsg, o.ToString(), StringComparison.InvariantCulture)), It.IsAny<Exception>(), (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public async Task QueueTrigger_Invalid_Import_Request_Error()
        {
            // Arrange
            const string expectedErrorMsg = "Invalid import request received.";
            const string queueMessage = "Test Message";
            Importer importer = new();

            // Act
            await importer.QueueTrigger(queueMessage, _mockBinder.Object, _mockLogger.Object);

            // Assert
            _mockBinder.Verify(b => b.BindAsync<string>(It.IsAny<BlobAttribute>(), CancellationToken.None), Times.Never);
            _mockLogger.Verify(l => l.Log(LogLevel.Error, It.IsAny<EventId>(), It.Is<It.IsAnyType>((o, t) => string.Equals(expectedErrorMsg, o.ToString(), StringComparison.InvariantCulture)), It.IsAny<Exception>(), (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public async Task QueueTrigger_ValidMessage_ParsesInvoices()
        {
            // Arrange
            string msg = JsonConvert.SerializeObject(_importRequest);
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes("This is my excel file"));
            Importer importer = new();
            _mockBinder.Setup(b => b.BindAsync<Stream>(It.IsAny<BlobAttribute>(), It.IsAny<CancellationToken>())).ReturnsAsync(memoryStream);

            // Act
            await importer.QueueTrigger(msg, _mockBinder.Object, _mockLogger.Object);

            // Asserts
            _mockLogger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.AtLeastOnce());
        }
    }
}