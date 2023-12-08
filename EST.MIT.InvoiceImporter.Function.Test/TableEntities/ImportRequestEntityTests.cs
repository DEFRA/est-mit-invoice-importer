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
            ImportRequestId = Guid.Parse("f3939c6a-3527-4c0a-a649-f662f116d296"),
            FileName = "test.xlsx",
            FileSize = 1024,
            FileType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            Timestamp = DateTimeOffset.Now,
            PaymentType = "AR",
            Organisation = "RDT",
            SchemeType = "CP",
            AccountType = "First Payment",
            CreatedBy = "test@example.com",
            Status = UploadStatus.Upload_success,
            BlobFileName="BlobFileName",
            BlobFolder="BlobFolder"
        };

        var importRequestEntity = new ImportRequestEntity(request);

        Assert.Equal("test.xlsx", importRequestEntity.FileName);
        Assert.Equal(1024, importRequestEntity.FileSize);
        Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", importRequestEntity.FileType);
        Assert.NotNull(importRequestEntity.Timestamp);
        Assert.Equal("AR", importRequestEntity.PaymentType);
        Assert.Equal("RDT", importRequestEntity.Organisation);
        Assert.Equal("CP", importRequestEntity.SchemeType);
        Assert.Equal("First Payment", importRequestEntity.AccountType);
        Assert.Equal("test@example.com", importRequestEntity.CreatedBy);
        Assert.Equal(UploadStatus.Upload_success, importRequestEntity.Status);
        Assert.Equal("BlobFileName", importRequestEntity.BlobFileName);
        Assert.Equal("BlobFolder", importRequestEntity.BlobFolder);

        Assert.Equal(ImportRequestEntity.DefaultPartitionKey, importRequestEntity.PartitionKey);
        Assert.StartsWith(request.ImportRequestId.ToString(), importRequestEntity.RowKey);
        Assert.Equal(default, importRequestEntity.ETag);
    }
}
