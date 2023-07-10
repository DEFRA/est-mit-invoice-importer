using System;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EST.MIT.InvoiceImporter.Function.Interfaces;
using EST.MIT.InvoiceImporter.Function.Models;

namespace EST.MIT.InvoiceImporter.Function.Interfaces;

public interface IExcelDataReader
{
    string GetCellValue(Cell cell);
    string GetSharedStringValue(Cell cell, string value);
    string GetBooleanValue(string value);
    string GetDateValue(string value);
    Cell GetCell(string cellReference);
    string GetColumnName(string cellReference);
    int GetColumnIndexFromName(string columnName);
    ExcelInvoice GetExcelInvoice(int rowNumber);
    ExcelHeader GetExcelHeader(int rowNumber);
    ExcelLine GetExcelLine(int rowNumber);
    ExcelInvoice GetExcelData();
}
