using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using DocumentFormat.OpenXml.Spreadsheet;
using EST.MIT.InvoiceImporter.Function.Services;
using DocumentFormat.OpenXml.Packaging;
using EST.MIT.InvoiceImporter.Function.Test.Helpers;
using EST.MIT.InvoiceImporter.Function.Models;
using Moq;
using Microsoft.Extensions.Logging;

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

        var result = reader.GetExcelInvoice(rowNumber);

        Assert.Equal("InternalID_123", result.Id);
        Assert.Equal("AR", result.InvoiceType);
        Assert.Equal("RDT", result.Organisation);
        Assert.Equal("CP", result.SchemeType);
        Assert.Equal("First Payment", result.AccountType);
    }

    [Fact]
    public void GetExcelInvoice_ValidRow_ReturnsCorrectInvoiceHeader()
    {
        var actualHeader = reader.GetExcelHeader(4);

        Assert.Equal("100222", actualHeader.AgreementNumber);
        Assert.Equal("", actualHeader.ClaimRef);
        Assert.Equal("107377217", actualHeader.FRN);
        Assert.Equal("", actualHeader.PaymentRequestNumber);
        Assert.Equal("", actualHeader.ContractNumber);
        Assert.Equal("2613.69", actualHeader.Value);
        Assert.Equal("RD34", actualHeader.DeliveryBody);
        Assert.Equal("", actualHeader.DueDate);
        Assert.Equal("03/08/2022", actualHeader.RecoveryDate);
        Assert.Equal("RDPE RCA Project Ref 100669", actualHeader.Description);
    }

    [Fact]
    public void GetExcelLine_ValidRow_ReturnsCorrectInvoiceLine()
    {
        var actualLine = reader.GetExcelLine(4);

        Assert.Equal("2613.69", actualLine.Value);
        Assert.Equal("GBP", actualLine.Currency);
        Assert.Equal("ERD14", actualLine.FundCode);
        Assert.Equal("SOS273", actualLine.MainAccount);
        Assert.Equal("6106A", actualLine.SchemeCode);
        Assert.Equal("2014", actualLine.MarketingYear);
        Assert.Equal("P1 to P2 Transfer - 100% EU / Leader - M19 FA6a / RDD - LEADER", actualLine.Description);
    }

    [Fact]
    public void GetExcelData_ValidWorksheet_ReturnsCorrectData()
    {
        var mockWorksheetPart = SpreadsheetMocks.CreateMockWorksheetPartForInvoiceHeaderAndLine();

        var actualInvoice = reader.GetExcelData();
        Assert.Equal("InternalID_123", actualInvoice.Id);
        Assert.Equal("AR", actualInvoice.InvoiceType);
        Assert.Equal("RDT", actualInvoice.Organisation);
        Assert.Equal("CP", actualInvoice.SchemeType);
        Assert.Equal("First Payment", actualInvoice.AccountType);

        Assert.NotEmpty(actualInvoice.Headers);
        var actualHeader = actualInvoice.Headers.First();
        Assert.Equal("100222", actualHeader.AgreementNumber);
        Assert.Equal("", actualHeader.ClaimRef);
        Assert.Equal("107377217", actualHeader.FRN);
        Assert.Equal("", actualHeader.PaymentRequestNumber);
        Assert.Equal("", actualHeader.ContractNumber);
        Assert.Equal("2613.69", actualHeader.Value);
        Assert.Equal("RD34", actualHeader.DeliveryBody);
        Assert.Equal("", actualHeader.DueDate);
        Assert.Equal("03/08/2022", actualHeader.RecoveryDate);
        Assert.Equal("RDPE RCA Project Ref 100669", actualHeader.Description);

        Assert.NotEmpty(actualHeader.Lines);
        var actualLine = actualHeader.Lines.First();
        Assert.Equal("2613.69", actualLine.Value);
        Assert.Equal("GBP", actualLine.Currency);
        Assert.Equal("ERD14", actualLine.FundCode);
        Assert.Equal("SOS273", actualLine.MainAccount);
        Assert.Equal("6106A", actualLine.SchemeCode);
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
    public void GetCell_ReturnsEmptyCell_OnTimeout()
    {
        string cellReference = "A1";
        TimeSpan timeout = TimeSpan.FromSeconds(61);

        var result = reader.GetCell(cellReference);

        Assert.NotNull(result);
    }
}
