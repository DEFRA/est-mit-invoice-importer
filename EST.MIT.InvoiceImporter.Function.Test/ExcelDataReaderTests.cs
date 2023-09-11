using DocumentFormat.OpenXml.Spreadsheet;
using EST.MIT.InvoiceImporter.Function.Interfaces;
using EST.MIT.InvoiceImporter.Function.Models;
using EST.MIT.InvoiceImporter.Function.Services;
using EST.MIT.InvoiceImporter.Function.Test.Helpers;
using Moq;

namespace EST.MIT.InvoiceImporter.Function.Test;

public class ExcelDataReaderTests
{
    private ExcelDataReader reader;

    public ExcelDataReaderTests()
    {

        var workbookPart = SpreadsheetMocks.CreateMockWorkbookPartForInvoiceHeaderAndLine();
        var worksheetPart = SpreadsheetMocks.CreateMockWorksheetPartForInvoiceHeaderAndLine();
        reader = new ExcelDataReader(workbookPart, worksheetPart);
    }

    [Fact]
    public void GetExcelInvoice_ValidRow_ReturnsCorrectInvoice()
    {
        int rowNumber = 4;

        var actualInvoiceRow4 = reader.GetExcelInvoice(rowNumber);

        Assert.Equal("InternalID_123", actualInvoiceRow4.Id);
        Assert.Equal("AR", actualInvoiceRow4.InvoiceType);
        Assert.Equal("RDT", actualInvoiceRow4.Organisation);
        Assert.Equal("CP", actualInvoiceRow4.SchemeType);
        Assert.Equal("First Payment", actualInvoiceRow4.AccountType);
    }

    [Fact]
    public void GetExcelHeader_ValidRow_ReturnsCorrectInvoiceHeader()
    {
        var actualHeaderRow4 = reader.GetExcelHeader(4);

        Assert.Equal("100222", actualHeaderRow4.AgreementNumber);
        Assert.Equal("", actualHeaderRow4.ClaimRef);
        Assert.Equal("107377217", actualHeaderRow4.FRN);
        Assert.Equal("", actualHeaderRow4.PaymentRequestNumber);
        Assert.Equal("", actualHeaderRow4.ContractNumber);
        Assert.Equal("2613.69", actualHeaderRow4.Value);
        Assert.Equal("RD34", actualHeaderRow4.DeliveryBody);
        Assert.Equal("", actualHeaderRow4.DueDate);
        Assert.Equal("03/08/2022", actualHeaderRow4.RecoveryDate);
        Assert.Equal("RDPE RCA Project Ref 100669", actualHeaderRow4.Description);
    }

    [Fact]
    public void GetExcelLine_ValidRow_ReturnsCorrectInvoiceLine()
    {
        var actualLineRow4 = reader.GetExcelLine(4);

        Assert.Equal("2613.69", actualLineRow4.Value);
        Assert.Equal("GBP", actualLineRow4.Currency);
        Assert.Equal("ERD14", actualLineRow4.FundCode);
        Assert.Equal("SOS273", actualLineRow4.MainAccount);
        Assert.Equal("6106A", actualLineRow4.SchemeCode);
        Assert.Equal("2014", actualLineRow4.MarketingYear);
        Assert.Equal("P1 to P2 Transfer - 100% EU / Leader - M19 FA6a / RDD - LEADER", actualLineRow4.Description);
    }

    [Fact]
    public void GetExcelData_ValidWorksheet_ReturnsCorrectData()
    {
        var actualInvoices = reader.GetExcelData();

        Assert.Single(actualInvoices);

        var actualInvoiceRow4 = actualInvoices.First();

        Assert.Equal("InternalID_123", actualInvoiceRow4.Id);
        Assert.Equal("AR", actualInvoiceRow4.InvoiceType);
        Assert.Equal("RDT", actualInvoiceRow4.Organisation);
        Assert.Equal("CP", actualInvoiceRow4.SchemeType);
        Assert.Equal("First Payment", actualInvoiceRow4.AccountType);

        Assert.NotEmpty(actualInvoiceRow4.Headers);
        var actualHeaderRow4 = actualInvoiceRow4.Headers.First();
        Assert.Equal("100222", actualHeaderRow4.AgreementNumber);
        Assert.Equal("", actualHeaderRow4.ClaimRef);
        Assert.Equal("107377217", actualHeaderRow4.FRN);
        Assert.Equal("", actualHeaderRow4.PaymentRequestNumber);
        Assert.Equal("", actualHeaderRow4.ContractNumber);
        Assert.Equal("2613.69", actualHeaderRow4.Value);
        Assert.Equal("RD34", actualHeaderRow4.DeliveryBody);
        Assert.Equal("", actualHeaderRow4.DueDate);
        Assert.Equal("03/08/2022", actualHeaderRow4.RecoveryDate);
        Assert.Equal("RDPE RCA Project Ref 100669", actualHeaderRow4.Description);

        Assert.NotEmpty(actualHeaderRow4.Lines);
        Assert.Equal(2, actualHeaderRow4.Lines.Count);

        var actualLineRow4 = actualHeaderRow4.Lines.First();
        Assert.Equal("2613.69", actualLineRow4.Value);
        Assert.Equal("GBP", actualLineRow4.Currency);
        Assert.Equal("ERD14", actualLineRow4.FundCode);
        Assert.Equal("SOS273", actualLineRow4.MainAccount);
        Assert.Equal("6106A", actualLineRow4.SchemeCode);

        var actualLineRow5 = actualHeaderRow4.Lines[1];
        Assert.Equal("53378.76", actualLineRow5.Value);
        Assert.Equal("GBP", actualLineRow5.Currency);
        Assert.Equal("ERD14", actualLineRow5.FundCode);
        Assert.Equal("SOS273", actualLineRow5.MainAccount);
        Assert.Equal("6106A", actualLineRow5.SchemeCode);
    }

    [Fact]
    public void GetExcelData_Populates_HierarchicalDataModel()
    {
        var actualInvoices = reader.GetExcelData();

        Assert.Single(actualInvoices);

        var actualInvoiceRow4 = actualInvoices.First();

        Assert.Single(actualInvoiceRow4.Headers);

        var actualHeaderRow4 = actualInvoiceRow4.Headers.First();
        Assert.Equal(2, actualHeaderRow4.Lines.Count);
    }

    [Fact]
    public void GetColumnIndexFromName_ReturnsCorrectIndex()
    {
        int result = reader.GetColumnIndexFromName("A");

        Assert.Equal(1, result);
    }

    [Fact]
    public void GetCellValue_ReturnsEmptyString_ForNullCell()
    {
        Cell cell = null;

        var result = reader.GetCellValue(cell);

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void GetCellValue_ReturnsInnerText_ForCellWithoutDataType()
    {
        var cell = new Cell();
        cell.CellValue = new CellValue("Test Value");

        var result = reader.GetCellValue(cell);

        Assert.Equal("Test Value", result);
    }

    [Fact]
    public void GetCellValue_ReturnsSharedStringValue_ForCellWithSharedStringDataType()
    {
        var cell = new Cell { DataType = CellValues.SharedString };
        var cellValue = new CellValue("1");
        cell.Append(cellValue);

        var result = reader.GetCellValue(cell);

        Assert.Equal("AR", result);
    }

    [Fact]
    public void GetCellValue_ReturnsBooleanValue_ForCellWithBooleanDataType()
    {
        var cell = new Cell { DataType = CellValues.Boolean };
        var cellValue = new CellValue("0");
        cell.Append(cellValue);

        var result = reader.GetCellValue(cell);

        Assert.Equal("FALSE", result);
    }

    [Fact]
    public void GetCellValue_ReturnsDateValue_ForCellWithDateDataType()
    {
        var cell = new Cell { DataType = CellValues.Date };
        var cellValue = new CellValue("44927");
        cell.Append(cellValue);

        var result = reader.GetCellValue(cell);

        Assert.Equal("01/01/2023", result);
    }

    [Fact]
    public void GetCell_ReturnsMatchingCell()
    {
        string cellReference = "A1";

        var result = reader.GetCell(cellReference);

        Assert.NotNull(result);
    }

    [Fact]
    public void GetLastPopulatedRowNumber_ReturnsCorrectRowNumber_WhenWorksheetHasData()
    {
        var lastPopulatedRowNumber = reader.GetLastPopulatedRowNumber();

        Assert.Equal(5, lastPopulatedRowNumber);
    }

    [Fact]
    public void GetLastPopulatedRowNumber_ReturnsZero_WhenWorksheetIsEmpty()
    {
        var workbookPart = SpreadsheetMocks.CreateMockWorkbookPartForEmptyWorksheet();
        var worksheetPart = SpreadsheetMocks.CreateMockWorksheetPartForEmptyWorksheet();
        var reader = new ExcelDataReader(workbookPart, worksheetPart);

        var lastPopulatedRowNumber = reader.GetLastPopulatedRowNumber();

        Assert.Equal(0, lastPopulatedRowNumber);
    }

    [Fact]
    public void GetLastRowWithValueInColumnA_ReturnsCorrectRowNumber_WhenWorksheetHasData()
    {
        var lastRowNumber = reader.GetLastRowWithValueInColumnA();

        Assert.Equal(4, lastRowNumber);
    }

    [Fact]
    public void GetLastRowWithValueInColumnA_ReturnsZero_WhenWorksheetIsEmpty()
    {
        var workbookPart = SpreadsheetMocks.CreateMockWorkbookPartForEmptyWorksheet();
        var worksheetPart = SpreadsheetMocks.CreateMockWorksheetPartForEmptyWorksheet();
        var reader = new ExcelDataReader(workbookPart, worksheetPart);

        var lastRowNumber = reader.GetLastRowWithValueInColumnA();

        Assert.Equal(0, lastRowNumber);
    }

    [Fact]
    public void CheckInvoiceExistsOnRow_ReturnsTrue_WhenRowContainsValuesInColumnsFToO()
    {
        var rowNumber = 4;
        var isInvoice = reader.CheckInvoiceExistsOnRow(rowNumber);

        Assert.True(isInvoice);
    }


    [Fact]
    public void CheckCheckHeaderExistsOnRowExistsOnRow_ReturnsTrue_WhenRowContainsValuesInColumnsFToO()
    {
        var rowNumber = 4;
        var isHeader = reader.CheckHeaderExistsOnRow(rowNumber);

        Assert.True(isHeader);
    }

    [Fact]
    public void CheckLineExistsOnRow_ReturnsTrue_WhenRowContainsValuesInColumnsQToW()
    {
        var rowNumber = 4;
        var isLine = reader.CheckLineExistsOnRow(rowNumber);

        Assert.True(isLine);
    }

    [Fact]
    public void CheckInvoiceExistsOnRow_ReturnsFalse_WhenRowDoesNotContainValuesInColumnsAToE()
    {
        var workbookPart = SpreadsheetMocks.CreateMockWorkbookPartForEmptyWorksheet();
        var worksheetPart = SpreadsheetMocks.CreateMockWorksheetPartForEmptyWorksheet();
        var reader = new ExcelDataReader(workbookPart, worksheetPart);

        var rowNumber = 4;
        var isInvoice = reader.CheckLineExistsOnRow(rowNumber);

        Assert.False(isInvoice);
    }

    [Fact]
    public void CheckHeaderExistsOnRow_ReturnsFalse_WhenRowDoesNotContainValuesInColumnsFToO()
    {
        var workbookPart = SpreadsheetMocks.CreateMockWorkbookPartForEmptyWorksheet();
        var worksheetPart = SpreadsheetMocks.CreateMockWorksheetPartForEmptyWorksheet();
        var reader = new ExcelDataReader(workbookPart, worksheetPart);

        var rowNumber = 4;
        var isHeader = reader.CheckHeaderExistsOnRow(rowNumber);

        Assert.False(isHeader);
    }

    [Fact]
    public void CheckLineExistsOnRow_ReturnsFalse_WhenRowDoesNotContainValuesInColumnsQToW()
    {
        var workbookPart = SpreadsheetMocks.CreateMockWorkbookPartForEmptyWorksheet();
        var worksheetPart = SpreadsheetMocks.CreateMockWorksheetPartForEmptyWorksheet();
        var reader = new ExcelDataReader(workbookPart, worksheetPart);

        var rowNumber = 4;
        var isLine = reader.CheckLineExistsOnRow(rowNumber);

        Assert.False(isLine);
    }

    [Fact]
    public void DetermineRowType_ReturnsTrue()
    {

        var actualInvoices = reader.GetExcelData();

        bool invoiceExists = reader.CheckInvoiceExistsOnRow(4);
        bool headerExists = reader.CheckHeaderExistsOnRow(4);
        bool lineExists = reader.CheckLineExistsOnRow(4);

        Assert.True(invoiceExists);
        Assert.True(headerExists);
        Assert.True(lineExists);
    }

    [Fact]
    public void DetermineRowType_ReturnsFalse()
    {
        var workbookPart = SpreadsheetMocks.CreateMockWorkbookPartForEmptyWorksheet();
        var worksheetPart = SpreadsheetMocks.CreateMockWorksheetPartForEmptyWorksheet();
        var reader = new ExcelDataReader(workbookPart, worksheetPart);

        var actualInvoices = reader.GetExcelData();

        bool invoiceExists = reader.CheckInvoiceExistsOnRow(4);
        bool headerExists = reader.CheckHeaderExistsOnRow(4);
        bool lineExists = reader.CheckLineExistsOnRow(4);

        Assert.False(invoiceExists);
        Assert.False(headerExists);
        Assert.False(lineExists);
    }

}
