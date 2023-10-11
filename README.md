# Introduction 
This Repository contains the code for the function app which parses bulk upload data from blob storage when triggered from the message queue. 
It reads the bulk invoice csv file to be imported from blob storage, checks it for validation errors, parses it into invoice objects and sends the validated invoice
data onto the invoice service.

## Local dev
For local dev, use the storage emeulator `Azurite` for queues

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

## Build and Test 
To run the function:

'cd EST.MIT.InvoiceImporter.Function'

'func start'

## Useful links

- [Use dependency injection in .NET Azure Functions](https://learn.microsoft.com/en-us/azure/azure-functions/functions-dotnet-dependency-injection)
