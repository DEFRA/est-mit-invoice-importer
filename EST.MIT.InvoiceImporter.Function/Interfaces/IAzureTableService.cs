using System.Collections.Generic;
using System.Threading.Tasks;
using EST.MIT.InvoiceImporter.Function.Models;

namespace EST.MIT.InvoiceImporter.Function.Interfaces;

public interface IAzureTableService
{
    Task UpsertImportRequestAsync(ImportRequest importRequest);

    Task<IEnumerable<ImportRequest>> GetAllImportRequestsAsync();
    Task<IEnumerable<ImportRequest>> GetUserImportRequestsAsync(string createdBy);
}