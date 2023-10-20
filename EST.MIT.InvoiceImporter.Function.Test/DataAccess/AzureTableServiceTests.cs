using EST.MIT.InvoiceImporter.Function.Models;
using Moq;
using Azure.Data.Tables;
using EST.MIT.InvoiceImporter.Function.DataAccess;
using EST.MIT.InvoiceImporter.Function.TableEntities;
using Azure;
using AutoMapper;
using EST.MIT.InvoiceImporter.Function.AutoMapperProfiles;

namespace EST.MIT.InvoiceImporter.Function.Test.Services;

public class AzureTableServiceTests
{
    private readonly Mock<TableClient> _tableClient = new();
    private readonly AzureTableService _datasetService;

    public AzureTableServiceTests()
    {
        var mapperConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new ImportRequestMapper());
        });
        mapperConfig.AssertConfigurationIsValid();
        IMapper mapper = mapperConfig.CreateMapper();

        _datasetService = new AzureTableService(_tableClient.Object, mapper);
    }

    [Fact]
    public async Task UpsertImportRequestAsyncAddTableEntity()
    {
        var mockDataset = new ImportRequest
        {
            FileName = "test.xlsx",
            FileSize = 1024,
            FileType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            Timestamp = DateTimeOffset.Now,
            PaymentType = "AR",
            Organisation = "RDT",
            SchemeType = "CP",
            AccountType = "First Payment"
        };

        await _datasetService.UpsertImportRequestAsync(mockDataset);

        _tableClient.Verify(x => x.UpsertEntityAsync(
                It.Is<ImportRequestEntity>(
                    e => e.FileName == "test.xlsx" && e.FileSize == 1024),
                    TableUpdateMode.Merge,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpsertImportRequestAsyncUpdateTableEntity()
    {
        var mockDataset = new ImportRequest
        {
            FileName = "test.xlsx",
            FileSize = 1024,
            FileType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            Timestamp = DateTimeOffset.Now,
            PaymentType = "AR",
            Organisation = "RDT",
            SchemeType = "CP",
            AccountType = "First Payment"
        };

        await _datasetService.UpsertImportRequestAsync(mockDataset);

        mockDataset.FileSize = 2048;

        await _datasetService.UpsertImportRequestAsync(mockDataset);

        _tableClient.Verify(x => x.UpsertEntityAsync(
                It.Is<ImportRequestEntity>(
                    e => e.FileName == "test.xlsx" && e.FileSize == 2048),
                    TableUpdateMode.Merge,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpsertImportRequestAsyncAssignsGuidIfEmpty()
    {
        var mockDataset = new ImportRequest
        {
            FileName = "test.xlsx",
            FileSize = 1024,
            FileType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            Timestamp = DateTimeOffset.Now,
            PaymentType = "AR",
            Organisation = "RDT",
            SchemeType = "CP",
            AccountType = "First Payment"
        };

        Assert.Equal(Guid.Empty, mockDataset.ImportRequestId);

        await _datasetService.UpsertImportRequestAsync(mockDataset);

        Assert.NotEqual(Guid.Empty, mockDataset.ImportRequestId);

        _tableClient.Verify(x => x.UpsertEntityAsync(
                It.IsAny<ImportRequestEntity>(),
                It.IsAny<TableUpdateMode>(),
                It.IsAny<CancellationToken>()),
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
                PaymentType = "AR",
                Timestamp = DateTimeOffset.Parse("2023-03-15T17:00:00.0000000+00:00"),
                CreatedBy = "test@example.com"
            },
            new ImportRequestEntity
            {
                PartitionKey = "9bb3ce76-c7bc-40ba-9330-d7143663e228",
                RowKey = "9bb3ce76-c7bc-40ba-9330-d7143663e228_2023-03-29T16:48:55.5134136+01:00",
                FileName = "test2.xlsx",
                PaymentType = "AP",
                Timestamp = DateTimeOffset.Parse("2023-03-15T17:00:01.0000000+00:00"),
                CreatedBy = "test@example.com"
            }
        }, null, Mock.Of<Response>());

        var pageable = Pageable<ImportRequestEntity>.FromPages(new[] { page });

        _tableClient.Setup(x => x.Query<ImportRequestEntity>(It.IsAny<string>(), null, null, CancellationToken.None)).Returns(pageable);

        var result = await _datasetService.GetAllImportRequestsAsync();

        var list = result.ToList();

        Assert.Equal("test2.xlsx", list[0].FileName);
        Assert.Equal("AP", list[0].PaymentType);

        Assert.Equal("test.xlsx", list[1].FileName);
        Assert.Equal("AR", list[1].PaymentType);
    }

    [Fact]
    public async Task GetUserDatasetsShouldReturnMostRecentEntityInEachPartition()
    {
        var page = Page<ImportRequestEntity>.FromValues(new[]
        {
            new ImportRequestEntity
            {
                PartitionKey = "9bb3ce76-c7bc-40ba-9330-d7143663e228",
                RowKey = "9bb3ce76-c7bc-40ba-9330-d7143663e228_2023-03-29T16:47:55.5134136+01:00",
                FileName = "test.xlsx",
                PaymentType = "AR",
                Timestamp = DateTimeOffset.Parse("2023-03-15T17:00:00.0000000+00:00"),
                CreatedBy = "test@example.com"
            },
            new ImportRequestEntity
            {
                PartitionKey = "9bb3ce76-c7bc-40ba-9330-d7143663e228",
                RowKey = "9bb3ce76-c7bc-40ba-9330-d7143663e228_2023-03-29T16:48:55.5134136+01:00",
                FileName = "test1.xlsx",
                PaymentType = "AP",
                Timestamp = DateTimeOffset.Parse("2023-03-15T17:00:01.0000000+00:00"),
                CreatedBy = "test@example.com"
            },
            new ImportRequestEntity
            {
                PartitionKey = "77c91e93-6dd6-4644-af64-7da6f27677f9",
                RowKey = "56066040-be37-402a-a9f8-9483910e84ec_2023-03-29T16:48:55.5134136+01:00",
                FileName = "test2.xlsx",
                PaymentType = "AP",
                Timestamp = DateTimeOffset.Parse("2023-03-15T17:00:01.0000000+00:00"),
                CreatedBy = "test2@example.com"
            }
        }, null, Mock.Of<Response>());

        var pageable = Pageable<ImportRequestEntity>.FromPages(new[] { page });

        _tableClient.Setup(x => x.Query<ImportRequestEntity>(It.IsAny<string>(), null, null, CancellationToken.None)).Returns(pageable);

        var result = await _datasetService.GetUserImportRequestsAsync("test@example.com");

        var list = result.ToList();
        Assert.Equal(2, result.Count());

        Assert.Equal("test1.xlsx", list[0].FileName);
        Assert.Equal("AP", list[0].PaymentType);

        Assert.Equal("test.xlsx", list[1].FileName);
        Assert.Equal("AR", list[1].PaymentType);
    }

    [Fact]
    public async Task GetUserImportRequestsByImportRequestIdAsync_ReturnsEntity_WhenEntityExists()
    {
        var mockDataset = new ImportRequest
        {
            ImportRequestId = Guid.Parse("f3939c6a-3527-4c0a-a649-f662f116d296"),
            FileName = "test.xlsx",
            FileSize = 1024,
            FileType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            Timestamp = DateTimeOffset.Now,
            PaymentType = "AR",
            Organisation = "RDT",
            SchemeType = "CP",
            AccountType = "First Payment"
        };

        var mockResponse = new Mock<Response<ImportRequestEntity>>();
        mockResponse.Setup(r => r.Value).Returns(new ImportRequestEntity(mockDataset));

        _tableClient.Setup(x => x.GetEntityAsync<ImportRequestEntity>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(mockResponse.Object));

        var result = await _datasetService.GetUserImportRequestsByImportRequestIdAsync("f3939c6a-3527-4c0a-a649-f662f116d296");

        Assert.NotNull(result);
        Assert.Equal(mockDataset.ImportRequestId, result.ImportRequestId);
        Assert.Equal(mockDataset.FileName, result.FileName);
        Assert.Equal(mockDataset.FileSize, result.FileSize);
        Assert.Equal(mockDataset.FileType, result.FileType);
        Assert.Equal(mockDataset.Timestamp, result.Timestamp);
        Assert.Equal(mockDataset.PaymentType, result.PaymentType);
        Assert.Equal(mockDataset.Organisation, result.Organisation);
        Assert.Equal(mockDataset.SchemeType, result.SchemeType);
        Assert.Equal(mockDataset.AccountType, result.AccountType);
    }
}