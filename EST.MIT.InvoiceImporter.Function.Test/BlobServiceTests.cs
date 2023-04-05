using Azure.Storage.Blobs;
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
            var blobService = new BlobService();

            // Act
            var result = await blobService.ReadBLOBIntoStream(importRequestJson, logger.Object, blobBinder.Object);

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
            var blobService = new BlobService();

            // Act
            var result = await blobService.ReadBLOBIntoStream(importRequestJson, logger.Object, blobBinder.Object);

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
            var blobService = new BlobService();

            // Act
            var result = await blobService.ReadBLOBIntoStream(importRequestJson, logger.Object, blobBinder.Object);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task MoveFileToArchive_WithInvalidFileName_ReturnsFalse()
        {
            // Arrange
            var fileName = "non-existent-file.txt";
            var blobServiceClient = new BlobServiceClient("UseDevelopmentStorage=true");
            var containerClient = blobServiceClient.GetBlobContainerClient("invoices");
            var sourceBlobClient = containerClient.GetBlobClient($"import/{fileName}");
            var destBlobClient = containerClient.GetBlobClient($"archive/{fileName}");
            var log = new Mock<ILogger<BlobService>>().Object;

            // Act
            var result = await BlobService.MoveFileToArchive(fileName, log, blobServiceClient);

            // Assert
            Assert.False(result);
            Assert.False(await sourceBlobClient.ExistsAsync()); // source blob should not exist
            Assert.False(await destBlobClient.ExistsAsync()); // destination blob should not exist
        }

        [Fact]
        public void GetFileName_ReturnsFileName()
        {
            // Arrange
            var expectedFileName = "test-file.txt";
            var blobService = new BlobService();

            blobService.GetType().GetField("_fileName", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(blobService, expectedFileName);

            // Act
            var result = blobService.GetFileName();

            // Assert
            Assert.Equal(expectedFileName, result);
        }
    }
}