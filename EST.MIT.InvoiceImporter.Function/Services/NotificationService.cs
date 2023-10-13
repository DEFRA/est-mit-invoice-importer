using System;
using System.Collections.Generic;
using System.Linq;
using EST.MIT.InvoiceImporter.Function.Interfaces;
using EST.MIT.InvoiceImporter.Function.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace EST.MIT.InvoiceImporter.Function.Services;

public class NotificationService : INotificationService
{
    private readonly IConfiguration _configuration;

    public NotificationService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string CreateNotificationMessage(string userId, IEnumerable<ImportRequest> importRequests)
    {
        var baseUrl = _configuration.GetValue<string>("WebUIBaseUrl");

        var messageObject = new
        {
            UserId = userId,
            Uploads = importRequests.Select(ir => new
            {
                ReferenceNumber = Guid.NewGuid().ToString(),
                Filename = ir.FileName,
                UploadStatus = ir.Status.ToString(),
                WebUILink = $"{baseUrl}{ir.FileName}"
            })
        };

        return JsonConvert.SerializeObject(messageObject);
    }
}
