using System;
using System.ComponentModel.DataAnnotations;
using EST.MIT.InvoiceImporter.Function.Models;
using Xunit;

namespace EST.MIT.InvoiceImporter.Function.Test.Models;

public class UploadStatusTests
{
    [Theory]
    [InlineData(UploadStatus.Required, "REQUIRED")]
    [InlineData(UploadStatus.Uploaded, "UPLOADED")]
    [InlineData(UploadStatus.Uploading, "UPLOADING")]
    [InlineData(UploadStatus.Validating, "VALIDATING")]
    [InlineData(UploadStatus.Rejected, "REJECTED")]
    public void DisplayAttributeIsCorrect(UploadStatus status, string expectedName)
    {
        var memberInfo = status.GetType().GetMember(status.ToString());
        var attributes = memberInfo[0].GetCustomAttributes(typeof(DisplayAttribute), false);

        var displayAttribute = (DisplayAttribute)attributes[0];

        Assert.Equal(expectedName, displayAttribute.Name);
    }
}
