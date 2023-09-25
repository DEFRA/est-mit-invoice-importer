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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EST.MIT.InvoiceImporter.Function.Functions;
public class UploadFunctions : IUploadFunctions
{
    private readonly IBlobService _blobService;
    private readonly IConfiguration _configuration;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly IEventQueueService _eventQueueService;
    private readonly IAzureTableService _azureTableService;

    public UploadFunctions(IBlobService blobService, IConfiguration configuration, IAzureBlobService azureBlobService, IEventQueueService eventQueueService, IAzureTableService azureTableService)
    {
        _blobService = blobService;
        _configuration = configuration;
        _eventQueueService = eventQueueService;
        _blobServiceClient = azureBlobService.BlobServiceClient ?? new BlobServiceClient(_configuration.GetConnectionString("PrimaryConnection"));
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