using Azure;
using Azure.Storage.Queues;
using EST.MIT.InvoiceImporter.Function.Services;
using Moq;

namespace EST.MIT.InvoiceImporter.Function.Test;

public class EventQueueServiceTests
{
    [Fact]
    public async Task CreateMessage_ValidArguments_CallsSendMessageAsync()
    {
        var queueClientMock = new Mock<QueueClient>();
        var eventQueueService = new EventQueueService(queueClientMock.Object);
        var expectedMessageContent = "Expected error content";

        queueClientMock
            .Setup(qc => qc.SendMessageAsync(It.IsAny<string>()))
            .Callback(() => throw new RequestFailedException(expectedMessageContent));

        Exception exception = null;

        try
        {
            await eventQueueService.CreateMessage("status", "action", "message", "data");
        }
        catch (Exception ex)
        {
            exception = ex;
        }

        Assert.NotNull(exception);
        Assert.Contains(expectedMessageContent, exception.Message);
    }
}
