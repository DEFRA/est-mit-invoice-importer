# Introduction 
This Repository contains the code for consuming the invoices to be imported from Web end points to the Payment Generator. 

# Getting Started 

## CosmosDb ??

- [Install and use the Azure Cosmos DB Emulator for local development and testing](https://learn.microsoft.com/en-us/azure/cosmos-db/local-emulator?tabs=ssl-netstd21)

## Azurite

Follow the following guide to setup Azurite:

- [Azurite emulator for local Azure Storage development](https://dev.azure.com/defragovuk/DEFRA-EST/_wiki/wikis/DEFRA-EST/7722/Azurite-emulator-for-local-Azure-Storage-development)

- [Docker](https://dev.azure.com/defragovuk/DEFRA-EST/_wiki/wikis/DEFRA-EST/9601/Azurite-with-Docker)

## Storage ??

The function app uses Azure Storage for Table and Queue.

The function app requires:

- Table name: `invoices`
- Queue name: `Invoice Importer`

## local.settings

'''
{
    "IsEncrypted": false,
    "Values": {
        "AzureWebJobsStorage": "UseDevelopmentStorage=true",
        "FUNCTIONS_WORKER_RUNTIME": "dotnet",
        "QueueConnectionString": "UseDevelopmentStorage=true",
        "TableConnectionString": "UseDevelopmentStorage=true"
    }
}



## HTTP

## Endpoint ??

'GET /invoice/invoiceimporter/{invoiceId}'

### Response 200

'''
{
	"name": "Invoice Importer"
	"properties":{
	"id":"1234567890",
	}
	 
}

# Build and Test 
To run the function:

'cd EST.MIT.InvoiceImporter.Function'

'func start'

## Useful links

- [Use dependency injection in .NET Azure Functions](https://learn.microsoft.com/en-us/azure/azure-functions/functions-dotnet-dependency-injection)
