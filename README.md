# Introduction 
This Repository contains the code for the function app which parses bulk upload data from blob storage when triggered from the message queue. 
It reads the bulk invoice csv file to be imported from blob storage, checks it for validation errors, parses it into invoice objects and sends the validated invoice
data onto the invoice service.

## Entry Queue
The function app requires:
- Queue name: `rpa-mit-invoice-importer`

## Storage
The function app uses Azure Storage for Table and Queue storage. 

### Storage Name

'invoices/import'
This is the location where it picks up the input data from the UI. 

'invoices/archive'
This is the location where it puts the file once processed. 

## Destination Endpoint

'POST /invoice'

## Build and Test locally

Use the storage emeulator `Azurite` for queues/tables/blobs

Create a local.settings.json file with the following content:
```
{
    "IsEncrypted": false,
    "Values": {
      "FUNCTIONS_WORKER_RUNTIME": "dotnet",
      "AzureWebJobsStorage": "UseDevelopmentStorage=true",
      "QueueConnectionString": "UseDevelopmentStorage=true",
      "BlobConnectionString": "UseDevelopmentStorage=true",
      "TableConnectionString": "UseDevelopmentStorage=true"
    }
}
```
To run the function:

'cd EST.MIT.InvoiceImporter.Function'

'func start'

## Example payload

1. Ensure you have file 'test1.xlsx' in blob storage under folder 'import'
2. Paste this payload into the rpa-mit-invoice-importer queue
```
{
 "fileName": "test1.xlsx",
 "fileSize": 1024,
 "fileType": "xlsx",
 "timestamp": "2023-10-01 09:07:10",
 "paymentType": "",
 "organisation": "myOrg",
 "schemeType": "pbs",
 "accountType": "",
 "createdBy": "Steve Dickinson"
}
```
3. The file 'test1.xlsx' should get processed and moved from /import to /archive in blob storage

## Useful links

- [Use dependency injection in .NET Azure Functions](https://learn.microsoft.com/en-us/azure/azure-functions/functions-dotnet-dependency-injection)
