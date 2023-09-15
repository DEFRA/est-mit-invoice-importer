using EST.MIT.InvoiceImporter.Function.Models;
using EST.MIT.InvoiceImporter.Function.TableEntities;

namespace EST.MIT.InvoiceImporter.Function.Test.TableEntities;

public class ImportRequestEntityTests
{
    [Fact]
    public void TestImportRequestEntityProperties()
    {
        var request = new ImportRequest
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

        var importRequestEntity = new ImportRequestEntity(request);

        Assert.Equal("test.xlsx", importRequestEntity.FileName);
        Assert.Equal(1024, importRequestEntity.FileSize);
        Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", importRequestEntity.FileType);
        Assert.NotNull(importRequestEntity.Timestamp);
        Assert.Equal("AR", importRequestEntity.InvoiceType);
        Assert.Equal("RDT", importRequestEntity.Organisation);
        Assert.Equal("CP", importRequestEntity.SchemeType);
        Assert.Equal("First Payment", importRequestEntity.AccountType);
    }
}
