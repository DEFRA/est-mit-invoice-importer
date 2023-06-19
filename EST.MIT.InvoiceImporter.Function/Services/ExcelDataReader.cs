using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using EST.MIT.InvoiceImporter.Function.Interfaces;

namespace EST.MIT.InvoiceImporter.Function.Services;

public class ExcelDataReader : IExcelDataReader
{
    public WorksheetPart GetWorksheet(string filePath, string sheetName)
    {
        SpreadsheetDocument document = SpreadsheetDocument.Open(filePath, false);

        WorkbookPart workbookPart = document.WorkbookPart;
        Sheet theSheet = workbookPart.Workbook.Descendants<Sheet>().Where(s => s.Name == sheetName).FirstOrDefault();

        if (theSheet == null)
        {
            throw new ArgumentException("sheetName");
        }

        WorksheetPart wsPart = (WorksheetPart)(workbookPart.GetPartById(theSheet.Id));

        return wsPart;
    }
}
