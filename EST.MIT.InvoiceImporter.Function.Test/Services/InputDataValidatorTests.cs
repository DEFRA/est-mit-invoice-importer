using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using EST.MIT.InvoiceImporter.Function.Interfaces;
using EST.MIT.InvoiceImporter.Function.Services;

namespace EST.MIT.InvoiceImporter.Function.Test;

public class InputDataValidatorTests
{
    private IInputDataValidator toTest;

    public InputDataValidatorTests()
    {
        toTest = new InputDataValidator();
    }

    [Fact]
    public void TestGetCellValue_WhenCellValueIsNull()
    {
        Cell cell = new Cell();
        cell.CellValue = null;

        string result = toTest.GetCellValue(cell);
        Assert.Null(result);
    }

    [Fact]
    public void TestGetCellValue_WhenCellValueIsNotNull()
    {
        Cell cell = new Cell();
        cell.CellValue = new CellValue("testValue");
        cell.DataType = new EnumValue<CellValues>(CellValues.String);

        string result = toTest.GetCellValue(cell);
        Assert.Equal("testValue", result);
    }

    [Fact]
    public void TestProcessData_WhenSheetDoesNotExist()
    {
        var filePath = "TestData/MIT Invoice Import Spreadsheet.xlsx";
        using (var document = SpreadsheetDocument.Open(filePath, false))
        {
            WorkbookPart workbookPart = document.WorkbookPart;
            toTest = new InputDataValidator();

            Assert.Throws<ArgumentException>(() => toTest.ProcessData(workbookPart, "NonExistentSheet"));
        }
    }

    [Fact]
    public void TestProcessData_WhenSheetExists()
    {
        var filePath = "TestData/MIT Invoice Import Spreadsheet.xlsx";
        using (var document = SpreadsheetDocument.Open(filePath, false))
        {
            WorkbookPart workbookPart = document.WorkbookPart;
            toTest = new InputDataValidator();

            var exception = Record.Exception(() => toTest.ProcessData(workbookPart, "Invoices"));
            Assert.Null(exception);
        }
    }
}
