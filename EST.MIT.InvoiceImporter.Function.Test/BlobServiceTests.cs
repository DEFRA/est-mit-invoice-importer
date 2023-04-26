using InvoiceImporter.Function.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using System.Reflection;

namespace EST.MIT.InvoiceImporter.Function.Services.Tests
{
    public class BlobServiceTests
    {
        [Fact]
        public async Task ReadBLOBIntoStream_WithValidImportRequest_ReturnsStream()
        {
            // Arrange
            var importRequest = new ImportRequest { FileName = "test-file.txt" };
            var importRequestJson = JsonConvert.SerializeObject(importRequest);
            var blobBinder = new Mock<IBinder>();
            var blobStream = new MemoryStream();
            blobBinder.Setup(b => b.BindAsync<Stream>(It.IsAny<BlobAttribute>(), CancellationToken.None)).ReturnsAsync(blobStream);
            var logger = new Mock<ILogger<BlobService>>();
            var blobService = new BlobService(logger.Object);

            // Act
            var result = await blobService.ReadBLOBIntoStream(importRequestJson, blobBinder.Object);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<MemoryStream>(result);
        }

        [Fact]
        public async Task ReadBLOBIntoStream_WithInvalidImportRequest_ReturnsNull()
        {
            // Arrange
            var importRequestJson = "invalid-json";
            var blobBinder = new Mock<IBinder>();
            var logger = new Mock<ILogger<BlobService>>();
            var blobService = new BlobService(logger.Object);

            // Act
            var result = await blobService.ReadBLOBIntoStream(importRequestJson, blobBinder.Object);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task ReadBLOBIntoStream_WithNullImportRequest_ReturnsNull()
        {
            // Arrange
            string? importRequestJson = null;
            var blobBinder = new Mock<IBinder>();
            var logger = new Mock<ILogger<BlobService>>();
            var blobService = new BlobService(logger.Object);

            // Act
            var result = await blobService.ReadBLOBIntoStream(importRequestJson, blobBinder.Object);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetFileName_ReturnsFileName()
        {
            // Arrange
            var logger = new Mock<ILogger<BlobService>>();
            var expectedFileName = "test-file.txt";
            var blobService = new BlobService(logger.Object);

            blobService.GetType().GetField("_fileName", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(blobService, expectedFileName);

            // Act
            var result = blobService.GetFileName();

            // Assert
            Assert.Equal(expectedFileName, result);
        }
    }
}