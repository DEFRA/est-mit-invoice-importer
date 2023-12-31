using EST.MIT.InvoiceImporter.Function.Models;

namespace EST.MIT.InvoiceImporter.Function.Test.Models;

public class ImportRequestTests
{
    [Fact]
    public void TestExcelLineProperties()
    {
        var importRequest = new ImportRequest
        {
            ImportRequestId = Guid.Parse("f3939c6a-3527-4c0a-a649-f662f116d296"),
            FileName = "test.xlsx",
            FileSize = 1024,
            FileType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            Timestamp = DateTimeOffset.Now,
            PaymentType = "AR",
            Organisation = "RDT",
            SchemeType = "CP",
            AccountType = "First Payment",
            Email = "email@defra.gov.uk",
            CreatedBy = "test@example.com",
            Status = UploadStatus.Uploaded,
            BlobFileName = "https://defrastorageaccount.blob.core.windows.net/invoices/import/test.xlsx"
        };

        Assert.Equal(Guid.Parse("f3939c6a-3527-4c0a-a649-f662f116d296"), importRequest.ImportRequestId);
        Assert.Equal("test.xlsx", importRequest.FileName);
        Assert.Equal(1024, importRequest.FileSize);
        Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", importRequest.FileType);
        Assert.NotNull(importRequest.Timestamp);
        Assert.Equal("AR", importRequest.PaymentType);
        Assert.Equal("RDT", importRequest.Organisation);
        Assert.Equal("CP", importRequest.SchemeType);
        Assert.Equal("First Payment", importRequest.AccountType);
        Assert.Equal("email@defra.gov.uk", importRequest.Email);
        Assert.Equal("test@example.com", importRequest.CreatedBy);
        Assert.Equal(UploadStatus.Uploaded, importRequest.Status);
        Assert.Equal("https://defrastorageaccount.blob.core.windows.net/invoices/import/test.xlsx", importRequest.BlobFileName);
    }
}