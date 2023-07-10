using System.Diagnostics.CodeAnalysis;
namespace EST.MIT.InvoiceImporter.Function.Models;

[ExcludeFromCodeCoverage]
public static class InvoiceItemType
{
    public static readonly string InvoiceHeader = "InvoiceHeader";
    public static readonly string InvoiceDetail = "InvoiceDetail";
}