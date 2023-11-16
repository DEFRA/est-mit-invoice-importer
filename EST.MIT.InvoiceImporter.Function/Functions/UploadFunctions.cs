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
    private readonly IAzureBlobService _blobService;

    public UploadFunctions(IAzureTableService azureTableService, IAzureBlobService azureBlobService)
    {
        _azureTableService = azureTableService;
        _blobService = azureBlobService;
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

    [FunctionName("GetUploadedFile")]
    public async Task<IActionResult> GetUploadedFile(
      [HttpTrigger(AuthorizationLevel.Function, "get", Route = "UploadedFile/{Ref}")] HttpRequest req,
      string Ref,
      ILogger log)
    {
        log.LogInformation($"[GetUploadedFile] Received request for Ref: {Ref}");

        var importRequest = await _azureTableService.GetUserImportRequestsByImportRequestIdAsync(Ref);
        if (importRequest == null)
        {
            return new NotFoundResult();
        }

        var fileStream = await _blobService.GetFileByFileNameAsync($"{importRequest.BlobFolder}/{importRequest.BlobFileName}");
        if (fileStream == null)
        {
            return new NotFoundResult();
        }

        return new FileStreamResult(fileStream, "application/octet-stream") { FileDownloadName = importRequest.FileName };
    }
}