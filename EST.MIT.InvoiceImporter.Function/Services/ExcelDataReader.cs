using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using EST.MIT.InvoiceImporter.Function.Interfaces;
using EST.MIT.InvoiceImporter.Function.Models;
using System.Diagnostics.CodeAnalysis;

namespace EST.MIT.InvoiceImporter.Function.Services;

public class ExcelDataReader : IExcelDataReader
{
    private readonly WorkbookPart workbookPart;
    private readonly WorksheetPart worksheetPart;
    private static readonly int timeoutInSeconds = 60;
    private static readonly string UkDateFormat = "dd/MM/yyyy";

    [ExcludeFromCodeCoverage]
    public ExcelDataReader(string filePath, string sheetName)
    {
        SpreadsheetDocument document = SpreadsheetDocument.Open(filePath, false);

        workbookPart = document.WorkbookPart;
        Sheet theSheet = workbookPart.Workbook.Descendants<Sheet>().FirstOrDefault(s => s.Name == sheetName);

        if (theSheet == null)
        {
            throw new ArgumentException("sheetName");
        }

        worksheetPart = (WorksheetPart)(workbookPart.GetPartById(theSheet.Id));
    }

    [ExcludeFromCodeCoverage]
    public ExcelDataReader(WorkbookPart workbookPart, WorksheetPart worksheetPart)
    {
        this.workbookPart = workbookPart ?? throw new ArgumentNullException(nameof(workbookPart));
        this.worksheetPart = worksheetPart ?? throw new ArgumentNullException(nameof(worksheetPart));
    }

    public string GetCellValue(Cell cell)
    {
        if (cell == null)
        {
            return string.Empty;
        }

        string value = cell.InnerText;

        if (cell.DataType != null)
        {
            switch (cell.DataType.Value)
            {
                case CellValues.SharedString:
                    return GetSharedStringValue(value);
                case CellValues.Boolean:
                    return GetBooleanValue(value);
                case CellValues.Date:
                    return GetDateValue(value);
            }
        }

        return value;
    }

    private string GetSharedStringValue(string value)
    {
        if (Int32.TryParse(value, out int id))
        {
            SharedStringTablePart stringTablePart = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
            if (stringTablePart != null)
            {
                SharedStringItem item = stringTablePart.SharedStringTable.ElementAt(id) as SharedStringItem;
                if (item != null && item.Text != null)
                {
                    value = item.Text.Text;
                }
            }
        }
        return value;
    }

    private static string GetBooleanValue(string value)
    {
        return (value == "0") ? "FALSE" : "TRUE";
    }

    private static string GetDateValue(string value)
    {
        return DateTime.FromOADate(double.Parse(value)).ToString(UkDateFormat);
    }

    public Cell GetCell(string cellReference)
    {
        string columnName = string.Empty;
        uint rowIndex = 0;

        if (TryParseCellReference(cellReference, out columnName, out rowIndex))
        {
            Task<Row> rowTask = GetRowByIndexAsync(rowIndex);
            Row row = rowTask.Result;
            if (row != null)
            {
                Cell matchingCell = row.Elements<Cell>().FirstOrDefault(c => string.Equals(GetColumnName(c.CellReference), columnName, StringComparison.OrdinalIgnoreCase));
                if (matchingCell != null)
                {
                    return matchingCell;
                }
            }
        }

        return new Cell();
    }

    private Task<Row> GetRowByIndexAsync(uint rowIndex)
    {
        return Task.Run(() =>
        {
            return worksheetPart.Worksheet.Descendants<Row>().FirstOrDefault(r => r.RowIndex == rowIndex);
        });
    }

    private static bool TryParseCellReference(string cellReference, out string columnName, out uint rowIndex)
    {
        columnName = string.Empty;
        rowIndex = 0;

        try
        {
            string parsedColumnName = string.Empty;
            uint parsedRowIndex = 0;

            Task task = Task.Run(() =>
            {
                var timeout = TimeSpan.FromSeconds(timeoutInSeconds);
                parsedColumnName = Regex.Replace(cellReference, @"\d", string.Empty, RegexOptions.None, timeout);
                parsedRowIndex = uint.Parse(Regex.Replace(cellReference, "[^0-9]", string.Empty, RegexOptions.None, timeout));
            });

            task.Wait();

            columnName = parsedColumnName;
            rowIndex = parsedRowIndex;

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public string GetColumnName(string cellReference)
    {
        if (string.IsNullOrEmpty(cellReference))
        {
            return null;
        }

        var timeout = TimeSpan.FromSeconds(timeoutInSeconds);
        var match = Regex.Match(cellReference, @"([A-Za-z]+)", RegexOptions.None, timeout);
        return match.Success ? match.Value : null;
    }


    public int GetColumnIndexFromName(string columnName)
    {
        int columnIndex = 0;
        for (int i = 0; i < columnName.Length; i++)
        {
            columnIndex *= 26;
            columnIndex += columnName[i] - 'A' + 1;
        }
        return columnIndex;
    }

    public ExcelInvoice GetExcelInvoice(int rowNumber)
    {
        var invoice = new ExcelInvoice
        {
            Id = GetCellValue(GetCell($"A{rowNumber}")),
            InvoiceType = GetCellValue(GetCell($"B{rowNumber}")),
            Organisation = GetCellValue(GetCell($"C{rowNumber}")),
            SchemeType = GetCellValue(GetCell($"D{rowNumber}")),
            AccountType = GetCellValue(GetCell($"E{rowNumber}"))
        };

        return invoice;
    }

    public ExcelHeader GetExcelHeader(int rowNumber)
    {
        var header = new ExcelHeader
        {
            AgreementNumber = GetCellValue(GetCell($"F{rowNumber}")),
            ClaimRef = GetCellValue(GetCell($"G{rowNumber}")),
            FRN = GetCellValue(GetCell($"H{rowNumber}")),
            PaymentRequestNumber = GetCellValue(GetCell($"I{rowNumber}")),
            ContractNumber = GetCellValue(GetCell($"J{rowNumber}")),
            Value = GetCellValue(GetCell($"K{rowNumber}")),
            DeliveryBody = GetCellValue(GetCell($"L{rowNumber}")),
            DueDate = GetCellValue(GetCell($"M{rowNumber}")),
            RecoveryDate = GetCellValue(GetCell($"N{rowNumber}")),
            Description = GetCellValue(GetCell($"O{rowNumber}"))
        };

        return header;
    }

    public ExcelLine GetExcelLine(int rowNumber)
    {
        var line = new ExcelLine
        {
            Value = GetCellValue(GetCell($"Q{rowNumber}")),
            Currency = GetCellValue(GetCell($"R{rowNumber}")),
            FundCode = GetCellValue(GetCell($"S{rowNumber}")),
            MainAccount = GetCellValue(GetCell($"T{rowNumber}")),
            SchemeCode = GetCellValue(GetCell($"U{rowNumber}")),
            MarketingYear = GetCellValue(GetCell($"V{rowNumber}")),
            Description = GetCellValue(GetCell($"W{rowNumber}"))
        };

        return line;
    }

    public List<ExcelInvoice> GetExcelData()
    {
        int rowNumber = 4;
        var invoices = new List<ExcelInvoice>();
        int maxRowNumber = GetLastPopulatedRowNumber();
        ExcelInvoice currentInvoice = null;
        ExcelHeader currentHeader = null;

        while (rowNumber <= maxRowNumber)
        {
            var invoiceDataExists = CheckInvoiceExistsOnRow(rowNumber);
            var headerDataExists = CheckHeaderExistsOnRow(rowNumber);
            var lineDataExists = CheckLineExistsOnRow(rowNumber);

            if (invoiceDataExists && headerDataExists && lineDataExists)
            {
                currentInvoice = GetExcelInvoice(rowNumber);
                currentInvoice.Headers = new List<ExcelHeader>();

                currentHeader = GetExcelHeader(rowNumber);
                currentHeader.Lines = new List<ExcelLine>();

                var line = GetExcelLine(rowNumber);
                currentHeader.Lines.Add(line);

                currentInvoice.Headers.Add(currentHeader);
            }
            else if (currentInvoice != null && !invoiceDataExists && headerDataExists && lineDataExists)
            {
                currentHeader = GetExcelHeader(rowNumber);
                currentHeader.Lines = new List<ExcelLine>();

                var line = GetExcelLine(rowNumber);
                currentHeader.Lines.Add(line);

                currentInvoice.Headers.Add(currentHeader);
            }
            else if (currentInvoice != null && currentHeader != null && !invoiceDataExists && !headerDataExists && lineDataExists)
            {
                var line = GetExcelLine(rowNumber);
                currentHeader.Lines.Add(line);
            }

            if (rowNumber == maxRowNumber && currentInvoice != null)
            {
                invoices.Add(currentInvoice);
            }

            rowNumber++;
        }

        return invoices;
    }

    public int GetLastPopulatedRowNumber()
    {
        return Convert.ToInt32(worksheetPart.Worksheet.Elements<SheetData>().First().Elements<Row>().LastOrDefault()?.RowIndex?.Value ?? 0);
    }

    public int GetLastRowWithValueInColumnA()
    {
        int lastPopulatedRowNumber = GetLastPopulatedRowNumber();

        for (int rowNumber = lastPopulatedRowNumber; rowNumber >= 1; rowNumber--)
        {
            string cellValue = GetCellValue(GetCell($"A{rowNumber}"));

            if (!string.IsNullOrEmpty(cellValue))
            {
                return rowNumber;
            }
        }

        return 0;
    }

    public ExcelRowType DetermineRowType(int rowNumber)
    {
        bool invoiceExists = CheckInvoiceExistsOnRow(rowNumber);
        bool headerExists = CheckHeaderExistsOnRow(rowNumber);
        bool lineExists = CheckLineExistsOnRow(rowNumber);

        if (invoiceExists && headerExists && lineExists)
        {
            return ExcelRowType.InvoiceHeaderLine;
        }
        else if (headerExists && lineExists)
        {
            return ExcelRowType.HeaderLine;
        }
        else if (lineExists)
        {
            return ExcelRowType.Line;
        }
        else
        {
            return ExcelRowType.Undefined;
        }
    }

    public bool CheckInvoiceExistsOnRow(int rowNumber)
    {
        string colAValue = GetCellValue(GetCell($"A{rowNumber}"));
        string colBValue = GetCellValue(GetCell($"B{rowNumber}"));
        string colCValue = GetCellValue(GetCell($"C{rowNumber}"));
        string colDValue = GetCellValue(GetCell($"D{rowNumber}"));
        string colEValue = GetCellValue(GetCell($"E{rowNumber}"));

        return !string.IsNullOrEmpty(colAValue) &&
               !string.IsNullOrEmpty(colBValue) &&
               !string.IsNullOrEmpty(colCValue) &&
               !string.IsNullOrEmpty(colDValue) &&
               !string.IsNullOrEmpty(colEValue);
    }

    public bool CheckHeaderExistsOnRow(int rowNumber)
    {
        string colFValue = GetCellValue(GetCell($"F{rowNumber}"));
        string colHValue = GetCellValue(GetCell($"H{rowNumber}"));
        string colKValue = GetCellValue(GetCell($"K{rowNumber}"));
        string colLValue = GetCellValue(GetCell($"L{rowNumber}"));
        string colOValue = GetCellValue(GetCell($"O{rowNumber}"));

        return !string.IsNullOrEmpty(colFValue) &&
               !string.IsNullOrEmpty(colHValue) &&
               !string.IsNullOrEmpty(colKValue) &&
               !string.IsNullOrEmpty(colLValue) &&
               !string.IsNullOrEmpty(colOValue);
    }

    public bool CheckLineExistsOnRow(int rowNumber)
    {
        string colQValue = GetCellValue(GetCell($"Q{rowNumber}"));
        string colRValue = GetCellValue(GetCell($"R{rowNumber}"));
        string colSValue = GetCellValue(GetCell($"S{rowNumber}"));
        string colTValue = GetCellValue(GetCell($"T{rowNumber}"));
        string colUValue = GetCellValue(GetCell($"U{rowNumber}"));
        string colVValue = GetCellValue(GetCell($"V{rowNumber}"));
        string colWValue = GetCellValue(GetCell($"W{rowNumber}"));

        return !string.IsNullOrEmpty(colQValue) &&
               !string.IsNullOrEmpty(colRValue) &&
               !string.IsNullOrEmpty(colSValue) &&
               !string.IsNullOrEmpty(colTValue) &&
               !string.IsNullOrEmpty(colUValue) &&
               !string.IsNullOrEmpty(colVValue) &&
               !string.IsNullOrEmpty(colWValue);
    }
}
