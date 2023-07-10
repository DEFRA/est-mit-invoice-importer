using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Xunit;
using Newtonsoft.Json;
using Xunit.Abstractions;
using EST.MIT.InvoiceImporter.Function.Services;
using EST.MIT.InvoiceImporter.Function.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace EST.MIT.InvoiceImporter.Function.Test;

[Collection("Integration Test")]
public class ExcelDataReaderIntegrationTests
{
    private readonly string _filePath;
    private readonly string _sheetName;
    private IExcelDataReader reader;

    public ExcelDataReaderIntegrationTests()
    {
        _filePath = "TestData/MIT Invoice Import Spreadsheet.xlsx";
        _sheetName = "Invoices";
        reader = new ExcelDataReader(_filePath, _sheetName);
    }

    [Fact]
    public void ReadExcelData_ValidFile_ReturnsCorrectData()
    {
        // var worksheetPart = reader.GetWorksheet();

        var actualInvoice = reader.GetExcelData();

        Assert.NotNull(actualInvoice);
        Assert.Equal("InternalID_123", actualInvoice.Id);
        Assert.Equal("AR", actualInvoice.InvoiceType);
        Assert.Equal("RDT", actualInvoice.Organisation);
        Assert.Equal("CP", actualInvoice.SchemeType);
        Assert.Equal("First Payment", actualInvoice.AccountType);

    }
}