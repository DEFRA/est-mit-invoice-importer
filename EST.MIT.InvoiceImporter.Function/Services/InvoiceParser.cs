﻿using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Delegates;
using CsvHelper.Expressions;
using CsvHelper.TypeConversion;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Text;
using System.Threading.Tasks;
using InvoiceImporter.Function.Models;
using Microsoft.Extensions.Logging;

namespace InvoiceImporter.Function.Service;

public class InvoiceParser : IInvoiceParser
{
    private readonly byte[] _invoice;
    private readonly byte _separator = (byte)',';

    public InvoiceParser()
    {
        _invoice = Encoding.UTF8.GetBytes("InvoiceType, AccountType, Organisation, SchemeType");//, Reference, Created, Updated, CreatedBy, UpdatedBy");
    }

    public async Task<List<Invoice>> GetInvoicesAsync(Stream stream, ILogger log)
    {
        var result = new List<Invoice>();
        try
        {
            log.LogInformation("Starting to import invoice");
            int currentCount = 1;
            using (var reader = new StreamReader(stream))
            using (var csv = new CsvReader(reader))
            {
                csv.Configuration // RegisterClassMap<InvoiceMap>();
            }
        }
        catch (Exception)
        {

            throw;
        }
    }
    public async Task<List<Invoice>> TryParse(Stream reader, ILogger log)
    {
        using var streamReader = new StreamReader(reader);

        var invoiceLines = await Parse(streamReader);
        return new List<Invoice>(invoiceLines);
    }

    private async Task<Invoice[]> Parse(StreamReader streamReader)
    {
        var invoicePool = ArrayPool<Invoice>.Shared;
        var invoices = invoicePool.Rent(1000);
        var position = 0;
        var reader = PipeReader.Create(streamReader.BaseStream);
        while (true)
        {
            var data = await reader.ReadAsync();
            var dataBuffer = data.Buffer;
            var actualPosition = ParseLine(dataBuffer, position, invoices);
            reader.AdvanceTo(actualPosition, dataBuffer.End);

            if (data.IsCompleted)
                break;
        }

        await reader.CompleteAsync();

        invoicePool.Return(invoices);

        return invoices;
    }

    private SequencePosition ParseLine(ReadOnlySequence<byte> dataBuffer, int position, Invoice[] invoices)
    {
        var reader = new SequenceReader<byte>(dataBuffer);
        while (reader.TryReadTo(out ReadOnlySpan<byte> line, (byte)'\n'))
        {
            var invoice = GetInvoice(line);
            if (invoice != null)
            {
                invoices[position] = invoice;
                position++;
            }
        }

        return reader.Position;
    }

    private Invoice GetInvoice(ReadOnlySpan<byte> line)
    {
        if (line.IndexOf(_invoice) >= 0)
            return null;

        var record = new Invoice();

        for (int i = 0; i < 4; i++)
        {
            var index = line.IndexOf(_separator);
            if (index < 0)
            {
                index = line.Length;
            }

            switch (i)
            {
                case 0:
                    record.InvoiceType = Encoding.UTF8.GetString(line[..index]);
                    break;
                case 1:
                    record.AccountType = Encoding.UTF8.GetString(line[..index]);
                    break;
                case 2:
                    record.Organisation = Encoding.UTF8.GetString(line[..index]);
                    break;
                case 3:
                    record.SchemeType = Encoding.UTF8.GetString(line[..index]);
                //    break;
                //case 4:
                //    record.Reference = Encoding.UTF8.GetString(line[..index]);
                //    break;
                //case 5:
                //    //record.Created = DateTime.Parse(Encoding.UTF8.GetString(line[..index]));
                //    break;
                //case 6:
                //    //record.Updated = DateTime.Parse(Encoding.UTF8.GetString(line)[..index]);
                //    break;
                //case 7:
                //    record.CreatedBy = Encoding.UTF8.GetString(line[..index]);
                //    break;
                //case 8:
                //    record.UpdatedBy = Encoding.UTF8.GetString(line[..index]);

                return record;
            }         
            line = line[(index + 1)..];
        }

        return record;
    }
}

