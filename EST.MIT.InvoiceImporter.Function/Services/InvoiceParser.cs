using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using InvoiceImporter.Function.Models;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace InvoiceImporter.Function.Service;

public class InvoiceParser : IInvoiceParser
{
    public InvoiceParser()
    {

    }

    public async Task<List<Invoice>> GetInvoicesAsync(Stream stream, ILogger log)
    {
        var inputData = await GetInputDataAsync(stream, log);
        List<Invoice> invoiceList = new (); //Empty top level class
        try
        {
            foreach (var inputRecord in inputData) //Loop through raw input data records
            {
                var invoice = new Invoice() //Create a new invoice record (top level)
                {
                    Id = Guid.NewGuid(),
                    AccountType = inputRecord.AccountType,
                    InvoiceType = inputRecord.InvoiceType,
                    PaymentRequests = new List<PaymentRequest>()
                    {
                        new PaymentRequest
                        {
                            PaymentRequestId = Guid.NewGuid().ToString(),
                            //TODO: Add these to the input CSV and the unit test csv mockup
                            //Currency = inputRecord.Currency,
                            SourceSystem = "Manual",
                            InvoiceLines = new List<InvoiceLine>()
                            {
                                new InvoiceLine()
                                {
                                    //TODO: Add these to the input CSV and the unit test csv mockup
                                    //Value = inputRecord.Value,
                                    //Description = inputRecord.Description,
                                    //SchemeCode = inputRecord.SchemeCode,
                                    //DeliveryBody = inputRecord.DeliveryBody                                    
                                }
                            }

                        }
                    }
                };
                //Check if the invoice is valid
                //First check for null values
                if (invoice.AccountType == null || invoice.InvoiceType == null)
                {
                    throw new InvalidOperationException("Invalid invoice record");
                }
                else
                {
                    invoiceList.Add(invoice);
                }
            }
            return await Task.FromResult(invoiceList);
        }
        catch (Exception exc)
        {
            throw new AggregateException("Error during Invoice import - " + exc.Message);
        }
    }

    private static async Task<List<InputData>> GetInputDataAsync(Stream stream, ILogger log)
    {
        List<InputData> inputData = new ();
        int count = 0;
        try
        {
            log.LogInformation("Starting to import data from data stream...");
            using var streamReader = new StreamReader(stream);
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord= true,
                Delimiter = ",",
                Quote = '"',
            };
            using (var csv = new CsvReader(streamReader, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<InputDataMap>();
                while (csv.Read())
                {
                    try
                    {
                        inputData.Add(csv.GetRecord<InputData>());
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException("Unknown record type. - " + ex.Message);
                    }
                    count++;
                }
            }
            log.LogInformation("Finished reading {0} records from data stream", count);
            return await Task.FromResult(inputData);
        }
        catch (Exception exc)
        {
            throw new AggregateException("Error during Invoice import - " + exc.Message);
        }
    }
}