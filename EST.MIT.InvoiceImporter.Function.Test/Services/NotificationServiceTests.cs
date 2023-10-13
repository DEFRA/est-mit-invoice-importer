using EST.MIT.InvoiceImporter.Function.Interfaces;
using Moq;
using EST.MIT.InvoiceImporter.Function.Models;
using EST.MIT.InvoiceImporter.Function.Services;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace EST.MIT.InvoiceImporter.Function.Test.Services;

public class NotificationServiceTests
{
    private readonly Mock<IConfiguration> _mockConfiguration;
    private INotificationService _notificationService;

    public NotificationServiceTests()
    {
        _mockConfiguration = new Mock<IConfiguration>();
    }

    [Fact]
    public void Given_ImportRequests_CreateNotificationMessage_Returns_ExpectedJson()
    {
        var userId = "test-user-id";
        var fakeBaseUrl = "http://mit.defra.gov.uk/";
        var importRequests = new List<ImportRequest>
            {
                new ImportRequest { FileName = "file1.xlsx", Status = UploadStatus.Uploaded },
                new ImportRequest { FileName = "file2.xlsx", Status = UploadStatus.Rejected }
            };

        var mockWebUIBaseUrlSection = new Mock<IConfigurationSection>();
        mockWebUIBaseUrlSection.SetupGet(m => m.Value).Returns(fakeBaseUrl);
        _mockConfiguration.Setup(m => m.GetSection("WebUIBaseUrl")).Returns(mockWebUIBaseUrlSection.Object);
        _notificationService = new NotificationService(_mockConfiguration.Object);

        var notificationMessage = _notificationService.CreateNotificationMessage(userId, importRequests);

        Assert.NotNull(notificationMessage);

        dynamic jsonMessage = JsonConvert.DeserializeObject(notificationMessage);
        Assert.Equal(userId, jsonMessage.UserId.ToString());

        var uploads = jsonMessage.Uploads.ToObject<List<dynamic>>();
        Assert.Equal(importRequests.Count, uploads.Count);

        for (int i = 0; i < uploads.Count; i++)
        {
            Assert.Equal($"{fakeBaseUrl}{importRequests[i].FileName}", uploads[i].WebUILink.ToString());
            Assert.Equal(importRequests[i].Status.ToString(), uploads[i].UploadStatus.ToString());
            Assert.Equal(importRequests[i].FileName, uploads[i].Filename.ToString());
        }
    }
}