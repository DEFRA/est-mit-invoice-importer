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
    private readonly TableClient _tableClient;
    private readonly IMapper _mapper;

    public AzureTableService(TableClient client, IMapper mapper)
    {
        _tableClient = client;
        _mapper = mapper;

        _tableClient.CreateIfNotExists();
    }

    public async Task UpsertImportRequestAsync(ImportRequest importRequest)
    {
        if (importRequest.ImportRequestId == Guid.Empty)
        {
            importRequest.ImportRequestId = Guid.NewGuid();
        }

        var entity = new ImportRequestEntity(importRequest);
        await _tableClient.UpsertEntityAsync(entity);
    }

    public Task<IEnumerable<ImportRequest>> GetAllImportRequestsAsync()
    {
        var query = _tableClient.Query<ImportRequestEntity>()
                     .OrderByDescending(e => e.Timestamp);

        var entities = query.AsEnumerable();

        var dataset = _mapper.Map<IEnumerable<ImportRequest>>(entities);

        return Task.FromResult(dataset);
    }

    public Task<IEnumerable<ImportRequest>> GetUserImportRequestsAsync(string createdBy)
    {
        var query = _tableClient.Query<ImportRequestEntity>()
                //.Where(i => i.CreatedBy == createdBy)     // ## Return for all users for now
                .OrderByDescending(e => e.Timestamp);

        var entities = query.AsEnumerable();

        var dataset = _mapper.Map<IEnumerable<ImportRequest>>(entities);

        return Task.FromResult(dataset);
    }

    public async Task<ImportRequest> GetUserImportRequestsByImportRequestIdAsync(string ImportRequestId)
    {
        try
        {
            var entity = await _tableClient.GetEntityAsync<ImportRequestEntity>(ImportRequestEntity.DefaultPartitionKey, ImportRequestId);
            return _mapper.Map<ImportRequest>(entity.Value);
        } catch (Exception ex)
        {
            return null;
        }
    }
}
