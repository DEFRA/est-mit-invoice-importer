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
        _invoice = Encoding.UTF8.GetBytes("InvoiceType, AccountType, Organisation, SchemeType, CreatedBy");
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

        for (int i = 0; i < 5; i++)
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
                    break;
                case 4:
                    record.CreatedBy = Encoding.UTF8.GetString(line[..index]);
                    break;
                case 5:
                    record.Reference = Encoding.UTF8.GetString(line[..index]);
                    return record;
            }         
            line = line[(index + 1)..];
        }

        return record;
    }
}

