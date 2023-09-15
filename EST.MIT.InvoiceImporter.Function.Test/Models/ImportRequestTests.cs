using EST.MIT.InvoiceImporter.Function.Models;

namespace EST.MIT.InvoiceImporter.Function.Test.Models;

public class ImportRequestTests
{
    [Fact]
    public void TestExcelLineProperties()
    {
        var importRequest = new ImportRequest
        {
            FileName = "test.xlsx",
            FileSize = 1024,
            FileType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            Timestamp = DateTimeOffset.Now,
            InvoiceType = "AR",
            Organisation = "RDT",
            SchemeType = "CP",
            AccountType = "First Payment"
        };

        Assert.Equal("test.xlsx", importRequest.FileName);
        Assert.Equal(1024, importRequest.FileSize);
        Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", importRequest.FileType);
        Assert.NotNull(importRequest.Timestamp);
        Assert.Equal("AR", importRequest.InvoiceType);
        Assert.Equal("RDT", importRequest.Organisation);
        Assert.Equal("CP", importRequest.SchemeType);
        Assert.Equal("First Payment", importRequest.AccountType);
    }
}