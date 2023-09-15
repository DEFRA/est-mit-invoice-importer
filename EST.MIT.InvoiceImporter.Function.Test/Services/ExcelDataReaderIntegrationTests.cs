using EST.MIT.InvoiceImporter.Function.Services;
using EST.MIT.InvoiceImporter.Function.Interfaces;

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
        var actualInvoices = reader.GetExcelData();

        Assert.Single(actualInvoices);

        var actualInvoice = actualInvoices.First();

        Assert.NotNull(actualInvoice);
        Assert.Equal("InternalID_123", actualInvoice.Id);
        Assert.Equal("AR", actualInvoice.InvoiceType);
        Assert.Equal("RDT", actualInvoice.Organisation);
        Assert.Equal("CP", actualInvoice.SchemeType);
        Assert.Equal("First Payment", actualInvoice.AccountType);
    }
}