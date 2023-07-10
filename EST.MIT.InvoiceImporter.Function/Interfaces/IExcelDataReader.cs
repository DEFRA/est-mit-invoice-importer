using DocumentFormat.OpenXml.Spreadsheet;
using EST.MIT.InvoiceImporter.Function.Models;

namespace EST.MIT.InvoiceImporter.Function.Interfaces;

public interface IExcelDataReader
{
    string GetCellValue(Cell cell);
    Cell GetCell(string cellReference);
    string GetColumnName(string cellReference);
    int GetColumnIndexFromName(string columnName);
    ExcelInvoice GetExcelInvoice(int rowNumber);
    ExcelHeader GetExcelHeader(int rowNumber);
    ExcelLine GetExcelLine(int rowNumber);
    ExcelInvoice GetExcelData();
}
