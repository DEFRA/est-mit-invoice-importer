using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Data.Tables;
using EST.MIT.InvoiceImporter.Function.Interfaces;
using EST.MIT.InvoiceImporter.Function.Models;
using EST.MIT.InvoiceImporter.Function.TableEntities;

namespace EST.MIT.InvoiceImporter.Function.Services;

public class AzureTableService : IAzureTableService
{
    private readonly TableClient _client;

    public AzureTableService(TableClient client)
    {
        _client = client;

        _client.CreateIfNotExists();
    }

    public async Task AddImportRequestAsync(ImportRequest importRequest)
    {
        var entity = new ImportRequestEntity(importRequest);

        await _client.AddEntityAsync(entity);
    }

    public Task<IEnumerable<ImportRequest>> GetAllImportRequestsAsync()
    {
        var query = _client.Query<ImportRequestEntity>()
                     .OrderByDescending(e => e.Timestamp);

        //TODO: move the mapping to an implement cast within ImportRequestEntity or (preferably) Automapper
        var datasets = query.Select(entity => new ImportRequest
        {
            FileName = entity.FileName,
            FileSize = entity.FileSize,
            FileType = entity.FileType,
            Timestamp = entity.Timestamp ?? DateTimeOffset.MinValue,
            InvoiceType = entity.InvoiceType,
            Organisation = entity.Organisation,
            SchemeType = entity.SchemeType,
            AccountType = entity.AccountType
        }).AsEnumerable();

        return Task.FromResult(datasets);
    }
}
