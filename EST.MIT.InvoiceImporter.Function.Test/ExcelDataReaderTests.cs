using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using DocumentFormat.OpenXml.Spreadsheet;
using EST.MIT.InvoiceImporter.Function.Services;
using DocumentFormat.OpenXml.Packaging;

namespace EST.MIT.InvoiceImporter.Function.Test;

public class ExcelDataReaderTests
{
    [Fact]
    public void GetWorksheetPart_ShouldGetWorksheetCorrectly()
    {
        var filePath = "TestData/MIT Invoice Import Spreadsheet.xlsx";
        var sheetName = "suggestion AG";
        var reader = new ExcelDataReader();

        WorksheetPart worksheetPart = reader.GetWorksheet(filePath, sheetName);

        worksheetPart.Should().NotBeNull();
        worksheetPart.Worksheet.Should().NotBeNull();
    }
}
