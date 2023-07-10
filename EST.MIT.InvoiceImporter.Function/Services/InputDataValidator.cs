using System;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using EST.MIT.InvoiceImporter.Function.Interfaces;

namespace EST.MIT.InvoiceImporter.Function.Services;

public class InputDataValidator : IInputDataValidator
{

    public void ProcessData(WorkbookPart workbookPart, string sheetName)
    {
        Sheet theSheet = workbookPart.Workbook.Descendants<Sheet>().Where(s => s.Name == sheetName).FirstOrDefault();

        if (theSheet == null)
        {
            throw new ArgumentException("sheetName");
        }

        WorksheetPart worksheetPart = (WorksheetPart)(workbookPart.GetPartById(theSheet.Id));

        var rows = worksheetPart.Worksheet.Descendants<Row>();
        foreach (var row in rows)
        {
            foreach (Cell cell in row.Elements<Cell>())
            {
                string cellValue = GetCellValue(cell);
                // TODO: Process data into required format using generics 
            }
        }
    }

    public string GetCellValue(Cell cell)
    {
        if (cell.CellValue == null)
        {
            return null;
        }

        return cell.CellValue.InnerXml;
    }



}
