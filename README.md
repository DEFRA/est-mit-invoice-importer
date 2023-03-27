# Introduction 
This Repository contains the code for the function app which parses bulk upload data from blob storage when triggered from the message queue. 
It reads the bulk invoice spreadsheet data to be imported from blob storage, checks it for validation errors and sends the validated data onto the invoice service.

## Entry Queue
The function app requires:
- Queue name: `invoice-importer`

## Storage
The function app uses Azure Storage for Table and Queue storage. 

### Storage Name

'invoices/import'

## Destination Endpoint

'POST /invoice'

## Build and Test 
To run the function:

'cd EST.MIT.InvoiceImporter.Function'

'func start'

## Useful links

- [Use dependency injection in .NET Azure Functions](https://learn.microsoft.com/en-us/azure/azure-functions/functions-dotnet-dependency-injection)
