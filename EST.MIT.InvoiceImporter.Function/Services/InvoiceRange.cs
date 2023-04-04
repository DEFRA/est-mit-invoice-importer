using System.Text.RegularExpressions;
using EST.MIT.InvoiceImporter.Function.Services;

namespace InvoiceImporter.Function.Services;

public class InvoiceRange
{
    public uint FirstRowNum { get; set; }
    public uint LastRowNum { get; set; }
    public string FirstColumn { get; set; }
    public string LastColumn { get; set; }

    public InvoiceRange(uint firstRowNum, uint lastRowNum, string firstColumn, string lastColumn)
    {
        FirstRowNum = firstRowNum;
        LastRowNum = lastRowNum;
        FirstColumn = firstColumn;
        LastColumn = lastColumn;
    }

    private static InvoiceRange InvoiceHeader(uint lastRow)
    {
        var firstRowNum = InvoiceUtil.GetRowIndex("B5");
        var lastRowNum = InvoiceUtil.GetRowIndex($"H{lastRow}");
        var firstColumn = InvoiceUtil.GetColumnName("B5");
        var lastColumn = InvoiceUtil.GetColumnName($"H{lastRow}");

        return new InvoiceRange(firstRowNum, lastRowNum, firstColumn, lastColumn);
    }

    private static InvoiceRange InvoiceDetail(uint lastRow)
    {
        var firstRowNum = InvoiceUtil.GetRowIndex("B5");
        var lastRowNum = InvoiceUtil.GetRowIndex($"AA{lastRow}");
        var firstColumn = InvoiceUtil.GetColumnName("Q5");
        var lastColumn = InvoiceUtil.GetColumnName($"AA{lastRow}");

        return new InvoiceRange(firstRowNum, lastRowNum, firstColumn, lastColumn);
    }

    public static InvoiceRange SelectInvoiceRange(string invoiceItemType, uint lastRow) => invoiceItemType switch
    {
        "InvoiceHeader" => InvoiceHeader(lastRow),
        "InvoiceDetail" => InvoiceDetail(lastRow),
        _ => throw new System.Exception("Invalid invoice item type")
    };
}
