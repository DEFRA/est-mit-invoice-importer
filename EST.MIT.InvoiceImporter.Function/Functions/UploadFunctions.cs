using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using EST.MIT.Importer.Function.Interfaces;
using EST.MIT.InvoiceImporter.Function.Interfaces;
using EST.MIT.InvoiceImporter.Function.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace EST.MIT.InvoiceImporter.Function.Functions;
public class UploadFunctions : IUploadFunctions
{
    private readonly IAzureTableService _azureTableService;

    public UploadFunctions(IAzureTableService azureTableService)
    {
        _azureTableService = azureTableService;
    }

    [FunctionName("GetUploadsByUser")]
    public async Task<IActionResult> GetUploadsByUser(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "Uploads/{UserId}")] HttpRequest req, string UserId, ILogger log)
    {
        log.LogInformation($"Fetching import requests for user {UserId}");

        try
        {
            IEnumerable<ImportRequest> importRequests = await _azureTableService.GetUserImportRequestsAsync(UserId);
            return new OkObjectResult(importRequests);
        }
        catch (Exception ex)
        {
            log.LogError($"An error occurred while fetching import requests for user {UserId}: {ex.Message}");
            return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
        }
    }
}