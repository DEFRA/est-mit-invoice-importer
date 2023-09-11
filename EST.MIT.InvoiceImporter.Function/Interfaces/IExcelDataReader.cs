using System.Collections.Generic;
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
    List<ExcelInvoice> GetExcelData();
    int GetLastPopulatedRowNumber();
    int GetLastRowWithValueInColumnA();
    ExcelRowType DetermineRowType(int rowNumber);

    bool CheckInvoiceExistsOnRow(int rowNumber);
    bool CheckHeaderExistsOnRow(int rowNumber);
    bool CheckLineExistsOnRow(int rowNumber);
}
