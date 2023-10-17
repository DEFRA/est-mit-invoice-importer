using System.Collections.Generic;
using EST.MIT.InvoiceImporter.Function.Models;

namespace EST.MIT.InvoiceImporter.Function.Interfaces;

public interface INotificationService
{
    string CreateNotificationMessage(string userId, IEnumerable<ImportRequest> importRequests);
}
