using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace EST.MIT.InvoiceImporter.Function.Test.Helpers;

public class SpreadsheetMocks
{
    public static WorksheetPart CreateMockWorksheetPartForInvoiceHeaderAndLine()
    {
        var stream = new MemoryStream();
        var spreadsheet = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook);

        var workbookPart = spreadsheet.AddWorkbookPart();
        workbookPart.Workbook = new Workbook();

        var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
        worksheetPart.Worksheet = new Worksheet();

        var sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

        var row = sheetData.AppendChild(new Row());
        row.RowIndex = 4;

        row.AppendChild(new Cell { CellValue = new CellValue("InternalID_123"), DataType = new EnumValue<CellValues>(CellValues.SharedString), CellReference = "A4" });
        row.AppendChild(new Cell { CellValue = new CellValue("AR"), DataType = new EnumValue<CellValues>(CellValues.SharedString), CellReference = "B4" });
        row.AppendChild(new Cell { CellValue = new CellValue("RDT"), DataType = new EnumValue<CellValues>(CellValues.SharedString), CellReference = "C4" });
        row.AppendChild(new Cell { CellValue = new CellValue("CP"), DataType = new EnumValue<CellValues>(CellValues.SharedString), CellReference = "D4" });
        row.AppendChild(new Cell { CellValue = new CellValue("First Payment"), DataType = new EnumValue<CellValues>(CellValues.SharedString), CellReference = "E4" });

        row.AppendChild(new Cell { CellValue = new CellValue("100222"), DataType = new EnumValue<CellValues>(CellValues.String), CellReference = "F4" });
        row.AppendChild(new Cell { CellValue = new CellValue(""), DataType = new EnumValue<CellValues>(CellValues.String), CellReference = "G4" });
        row.AppendChild(new Cell { CellValue = new CellValue("107377217"), DataType = new EnumValue<CellValues>(CellValues.String), CellReference = "H4" });
        row.AppendChild(new Cell { CellValue = new CellValue(""), DataType = new EnumValue<CellValues>(CellValues.String), CellReference = "I4" });
        row.AppendChild(new Cell { CellValue = new CellValue(""), DataType = new EnumValue<CellValues>(CellValues.String), CellReference = "J4" });
        row.AppendChild(new Cell { CellValue = new CellValue("2613.69"), DataType = new EnumValue<CellValues>(CellValues.String), CellReference = "K4" });
        row.AppendChild(new Cell { CellValue = new CellValue("RD34"), DataType = new EnumValue<CellValues>(CellValues.String), CellReference = "L4" });
        row.AppendChild(new Cell { CellValue = new CellValue(""), DataType = new EnumValue<CellValues>(CellValues.String), CellReference = "M4" });
        row.AppendChild(new Cell { CellValue = new CellValue("03/08/2022"), DataType = new EnumValue<CellValues>(CellValues.String), CellReference = "N4" });
        row.AppendChild(new Cell { CellValue = new CellValue("RDPE RCA Project Ref 100669"), DataType = new EnumValue<CellValues>(CellValues.String), CellReference = "O4" });

        row.AppendChild(new Cell { CellValue = new CellValue(""), DataType = new EnumValue<CellValues>(CellValues.String), CellReference = "P4" });

        row.AppendChild(new Cell { CellValue = new CellValue("2613.69"), DataType = new EnumValue<CellValues>(CellValues.String), CellReference = "Q4" });
        row.AppendChild(new Cell { CellValue = new CellValue("GBP"), DataType = new EnumValue<CellValues>(CellValues.String), CellReference = "R4" });
        row.AppendChild(new Cell { CellValue = new CellValue("ERD14"), DataType = new EnumValue<CellValues>(CellValues.String), CellReference = "S4" });
        row.AppendChild(new Cell { CellValue = new CellValue("SOS273"), DataType = new EnumValue<CellValues>(CellValues.String), CellReference = "T4" });
        row.AppendChild(new Cell { CellValue = new CellValue("6106A"), DataType = new EnumValue<CellValues>(CellValues.String), CellReference = "U4" });
        row.AppendChild(new Cell { CellValue = new CellValue("2014"), DataType = new EnumValue<CellValues>(CellValues.String), CellReference = "V4" });
        row.AppendChild(new Cell { CellValue = new CellValue("P1 to P2 Transfer - 100% EU / Leader - M19 FA6a / RDD - LEADER"), DataType = new EnumValue<CellValues>(CellValues.String), CellReference = "W4" });

        return worksheetPart;
    }


    public static WorkbookPart CreateMockWorkbookPartForInvoiceHeaderAndLine()
    {
        var stream = new MemoryStream();
        var spreadsheet = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook);

        var workbookPart = spreadsheet.AddWorkbookPart();
        workbookPart.Workbook = new Workbook();

        var sharedStringTablePart = workbookPart.AddNewPart<SharedStringTablePart>();
        var sharedStringTable = new SharedStringTable();
        sharedStringTablePart.SharedStringTable = sharedStringTable;

        // Add shared strings to the shared string table
        sharedStringTable.AppendChild(new SharedStringItem(new Text("InternalID_123")));
        sharedStringTable.AppendChild(new SharedStringItem(new Text("AR")));
        sharedStringTable.AppendChild(new SharedStringItem(new Text("RDT")));
        sharedStringTable.AppendChild(new SharedStringItem(new Text("CP")));
        sharedStringTable.AppendChild(new SharedStringItem(new Text("First Payment")));

        var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
        worksheetPart.Worksheet = new Worksheet();

        var sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

        var row = sheetData.AppendChild(new Row());
        row.RowIndex = 4;

        row.AppendChild(new Cell { CellValue = new CellValue("InternalID_123"), DataType = new EnumValue<CellValues>(CellValues.SharedString), CellReference = "A4" });
        row.AppendChild(new Cell { CellValue = new CellValue("AR"), DataType = new EnumValue<CellValues>(CellValues.SharedString), CellReference = "B4" });
        row.AppendChild(new Cell { CellValue = new CellValue("RDT"), DataType = new EnumValue<CellValues>(CellValues.SharedString), CellReference = "C4" });
        row.AppendChild(new Cell { CellValue = new CellValue("CP"), DataType = new EnumValue<CellValues>(CellValues.SharedString), CellReference = "D4" });
        row.AppendChild(new Cell { CellValue = new CellValue("First Payment"), DataType = new EnumValue<CellValues>(CellValues.SharedString), CellReference = "E4" });

        row.AppendChild(new Cell { CellValue = new CellValue("100222"), DataType = new EnumValue<CellValues>(CellValues.String), CellReference = "F4" });
        row.AppendChild(new Cell { CellValue = new CellValue(""), DataType = new EnumValue<CellValues>(CellValues.String), CellReference = "G4" });
        row.AppendChild(new Cell { CellValue = new CellValue("107377217"), DataType = new EnumValue<CellValues>(CellValues.String), CellReference = "H4" });
        row.AppendChild(new Cell { CellValue = new CellValue(""), DataType = new EnumValue<CellValues>(CellValues.String), CellReference = "I4" });
        row.AppendChild(new Cell { CellValue = new CellValue(""), DataType = new EnumValue<CellValues>(CellValues.String), CellReference = "J4" });
        row.AppendChild(new Cell { CellValue = new CellValue("2613.69"), DataType = new EnumValue<CellValues>(CellValues.String), CellReference = "K4" });
        row.AppendChild(new Cell { CellValue = new CellValue("RD34"), DataType = new EnumValue<CellValues>(CellValues.String), CellReference = "L4" });
        row.AppendChild(new Cell { CellValue = new CellValue(""), DataType = new EnumValue<CellValues>(CellValues.String), CellReference = "M4" });
        row.AppendChild(new Cell { CellValue = new CellValue("03/08/2022"), DataType = new EnumValue<CellValues>(CellValues.String), CellReference = "N4" });
        row.AppendChild(new Cell { CellValue = new CellValue("RDPE RCA Project Ref 100669"), DataType = new EnumValue<CellValues>(CellValues.String), CellReference = "O4" });

        row.AppendChild(new Cell { CellValue = new CellValue(""), DataType = new EnumValue<CellValues>(CellValues.String), CellReference = "P4" });

        row.AppendChild(new Cell { CellValue = new CellValue("2613.69"), DataType = new EnumValue<CellValues>(CellValues.String), CellReference = "Q4" });
        row.AppendChild(new Cell { CellValue = new CellValue("GBP"), DataType = new EnumValue<CellValues>(CellValues.String), CellReference = "R4" });
        row.AppendChild(new Cell { CellValue = new CellValue("ERD14"), DataType = new EnumValue<CellValues>(CellValues.String), CellReference = "S4" });
        row.AppendChild(new Cell { CellValue = new CellValue("SOS273"), DataType = new EnumValue<CellValues>(CellValues.String), CellReference = "T4" });
        row.AppendChild(new Cell { CellValue = new CellValue("6106A"), DataType = new EnumValue<CellValues>(CellValues.String), CellReference = "U4" });
        row.AppendChild(new Cell { CellValue = new CellValue("2014"), DataType = new EnumValue<CellValues>(CellValues.String), CellReference = "V4" });
        row.AppendChild(new Cell { CellValue = new CellValue("P1 to P2 Transfer - 100% EU / Leader - M19 FA6a / RDD - LEADER"), DataType = new EnumValue<CellValues>(CellValues.String), CellReference = "W4" });

        return workbookPart;
    }






}
