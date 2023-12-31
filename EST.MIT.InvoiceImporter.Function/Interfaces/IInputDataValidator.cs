using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace EST.MIT.InvoiceImporter.Function.Interfaces;

public interface IInputDataValidator
{
    void ProcessData(WorkbookPart workbookPart, string sheetName);
    public string GetCellValue(Cell cell);
}
