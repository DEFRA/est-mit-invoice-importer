//using Azure.Storage.Blobs;
//using EST.MIT.Importer.Function.Services;
//using EST.MIT.InvoiceImporter.Function.Services;
//using Microsoft.Extensions.Logging;
//using Moq;

//namespace InvoiceImporter.Function.Test
//{
//    public class ImporterTests
//    {
//        [Fact]
//        public async Task ProcessStream_MovesFileToArchive_WhenStreamIsNotNull()
//        {
//            // Arrange
//            var importMessage = "test-import-message";
//            var mockBlobServiceClient = new Mock<BlobServiceClient>().Object;
//            var mockBlobService = new Mock<BlobService>();
//            var memoryStream = new MemoryStream(new byte[] { 1, 2, 3 });
//            var mockLogger = new Mock<ILogger>();
//            var expectedLogMessage1 = $"[MainTrigger] file read into memory stream, filelength: 0 KB";
//            var expectedLogMessage2 = $"Moving file to archive: test-import-message";
//            var expectedLogMessage3 = $"An error occured when moving a file to the archive folder: [0]";
//            mockBlobService.Setup(x => x.GetFileName()).Returns("testfile.csv");

//            // Act
//            await Importer.ProcessStream(importMessage, mockLogger.Object, mockBlobServiceClient, mockBlobService.Object, memoryStream);

//            // Assert
//            mockLogger.Verify(l => l.Log(LogLevel.Information, It.IsAny<EventId>(), It.Is<It.IsAnyType>((o, t) => string.Equals(expectedLogMessage1, o.ToString(), StringComparison.InvariantCulture)), It.IsAny<Exception>(), (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);
//            mockLogger.Verify(l => l.Log(LogLevel.Error, It.IsAny<EventId>(), It.Is<It.IsAnyType>((o, t) => string.Equals(expectedLogMessage3, o.ToString(), StringComparison.InvariantCulture)), It.IsAny<Exception>(), (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);
//        }
//    }
//}
