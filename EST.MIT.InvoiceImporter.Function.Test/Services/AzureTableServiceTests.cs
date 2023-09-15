using EST.MIT.InvoiceImporter.Function.Models;
using Moq;
using Azure.Data.Tables;
using EST.MIT.InvoiceImporter.Function.Services;
using EST.MIT.InvoiceImporter.Function.TableEntities;
using Azure;

namespace EST.MIT.InvoiceImporter.Function.Test.Services;

public class AzureTableServiceTests
{

    private readonly Mock<TableClient> _tableClient = new();
    private readonly AzureTableService _datasetService;

    public AzureTableServiceTests()
    {
        _datasetService = new AzureTableService(_tableClient.Object);
    }

    [Fact]
    public async Task AddDatasetShouldAddTableEntity()
    {
        var mockDataset = new ImportRequest
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

        await _datasetService.AddImportRequestAsync(mockDataset);

        _tableClient.Verify(x => x.AddEntityAsync(
                It.Is<ImportRequestEntity>(
                    e => e.FileName == "test.xlsx" && e.FileSize == 1024),
                CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public async Task GetAllDatasetsShouldReturnMostRecentEntityInEachPartition()
    {
        var page = Page<ImportRequestEntity>.FromValues(new[]
        {
            new ImportRequestEntity
            {
                PartitionKey = "9bb3ce76-c7bc-40ba-9330-d7143663e228",
                RowKey = "9bb3ce76-c7bc-40ba-9330-d7143663e228_2023-03-29T16:47:55.5134136+01:00",
                FileName = "test.xlsx",
                InvoiceType = "AR",
                Timestamp = DateTimeOffset.Parse("2023-03-15T17:00:00.0000000+00:00")
            },
            new ImportRequestEntity
            {
                PartitionKey = "9bb3ce76-c7bc-40ba-9330-d7143663e228",
                RowKey = "9bb3ce76-c7bc-40ba-9330-d7143663e228_2023-03-29T16:48:55.5134136+01:00",
                FileName = "test2.xlsx",
                InvoiceType = "AP",
                Timestamp = DateTimeOffset.Parse("2023-03-15T17:00:01.0000000+00:00")
            }
        }, null, Mock.Of<Response>());

        var pageable = Pageable<ImportRequestEntity>.FromPages(new[] { page });

        _tableClient.Setup(x => x.Query<ImportRequestEntity>(It.IsAny<string>(), null, null, CancellationToken.None)).Returns(pageable);

        var result = await _datasetService.GetAllImportRequestsAsync();

        var list = result.ToList();

        Assert.Equal("test2.xlsx", list[0].FileName);
        Assert.Equal("AP", list[0].InvoiceType);

        Assert.Equal("test.xlsx", list[1].FileName);
        Assert.Equal("AR", list[1].InvoiceType);
    }
}