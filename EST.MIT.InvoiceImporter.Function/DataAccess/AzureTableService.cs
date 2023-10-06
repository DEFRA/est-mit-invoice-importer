using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Azure.Data.Tables;
using EST.MIT.InvoiceImporter.Function.Interfaces;
using EST.MIT.InvoiceImporter.Function.Models;
using EST.MIT.InvoiceImporter.Function.TableEntities;

namespace EST.MIT.InvoiceImporter.Function.DataAccess;

public class AzureTableService : IAzureTableService
{
    private readonly TableClient _client;
    private readonly IMapper _mapper;

    public AzureTableService(TableClient client, IMapper mapper)
    {
        _client = client;
        _mapper = mapper;

        _client.CreateIfNotExists();
    }

    public async Task UpsertImportRequestAsync(ImportRequest importRequest)
    {
        var entity = new ImportRequestEntity(importRequest);

        await _client.UpsertEntityAsync(entity);
    }

    public Task<IEnumerable<ImportRequest>> GetAllImportRequestsAsync()
    {
        var query = _client.Query<ImportRequestEntity>()
                     .OrderByDescending(e => e.Timestamp);

        var entities = query.AsEnumerable();

        var dataset = _mapper.Map<IEnumerable<ImportRequest>>(entities);

        return Task.FromResult(dataset);
    }

    public Task<IEnumerable<ImportRequest>> GetUserImportRequestsAsync(string createdBy)
    {
        var query = _client.Query<ImportRequestEntity>()
                .Where(i => i.CreatedBy == createdBy)
                .OrderByDescending(e => e.Timestamp);

        var entities = query.AsEnumerable();

        var dataset = _mapper.Map<IEnumerable<ImportRequest>>(entities);

        return Task.FromResult(dataset);
    }
}
