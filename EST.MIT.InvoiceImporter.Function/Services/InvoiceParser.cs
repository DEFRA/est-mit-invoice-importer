using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.Logging;
using InvoiceImporter.Function.Services;

namespace EST.MIT.InvoiceImporter.Function.Services;
public class InvoiceParser : IInvoiceParser
{
    private InvoiceRange invoiceRange;

    public List<T> Parse<T>(Stream blobStream, string invoiceAccountType, string invoiceItemType, ILogger log) where T : new()
    {
        using SpreadsheetDocument doc = SpreadsheetDocument.Open(blobStream, false);
        var worksheetPart = GetWorkSheet(doc, invoiceAccountType);
        var lastRow = GetLastRowNum(worksheetPart);
        invoiceRange = InvoiceRange.SelectInvoiceRange(invoiceItemType, lastRow);
        var rows = GetRows(worksheetPart);
        return GetCellValues<T>(doc, rows);
    }

    private static WorksheetPart GetWorkSheet(SpreadsheetDocument doc, string sheetName)
    {
        var workSheet = doc.WorkbookPart.Workbook.Descendants<Sheet>().FirstOrDefault(s => s.Name == sheetName);
        return (WorksheetPart)doc.WorkbookPart.GetPartById(workSheet.Id);
    }

    private IEnumerable<Row> GetRows(WorksheetPart worksheetPart)
    {
        return worksheetPart.Worksheet.GetFirstChild<SheetData>().Elements<Row>()
            .Where(r => r.RowIndex.Value >= invoiceRange.FirstRowNum && r.RowIndex.Value <= invoiceRange.LastRowNum);
    }

    private static uint GetLastRowNum(WorksheetPart worksheetPart)
    {
        return worksheetPart.Worksheet.Descendants<Row>().LastOrDefault().RowIndex.Value;
    }

    private List<T> GetCellValues<T>(SpreadsheetDocument doc, IEnumerable<Row> rows) where T : new()
    {
        var cellValuesList = new List<T>();
        var cellCount = 0;
        foreach (var row in rows)
        {
            var invoiceValues = new Dictionary<int, string>();

            foreach (var cell in row.Descendants<Cell>())
            {
                var cellValue = GetCellValue(doc, cell);

                var columnName = InvoiceUtil.GetColumnName(cell.CellReference.Value);
                if (InvoiceUtil.CompareColumn(columnName, invoiceRange.FirstColumn) >= 0
                    && InvoiceUtil.CompareColumn(columnName, invoiceRange.LastColumn) <= 0 && cellValue != null)
                {
                    invoiceValues.Add(cellCount, cellValue);
                    cellCount++;
                }
            }
            cellValuesList.Add(InvoiceUtil.DictionaryToObject<T>(invoiceValues));
            cellCount = 0;
        }

        return cellValuesList;
    }

    private static string GetCellValue(SpreadsheetDocument doc, Cell cell)
    {
        string value = cell.CellValue?.InnerText;
        if (cell.DataType?.Value == CellValues.SharedString)
        {
            return doc.WorkbookPart.SharedStringTablePart.SharedStringTable.ChildElements.GetItem(int.Parse(value)).InnerText;
        }
        return value;
    }
}

