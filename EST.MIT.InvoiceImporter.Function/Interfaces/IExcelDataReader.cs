using System;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace EST.MIT.InvoiceImporter.Function.Interfaces;

public interface IExcelDataReader
{
    WorksheetPart GetWorksheet(string filePath, string sheetName);
}
