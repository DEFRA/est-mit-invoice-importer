using System;

namespace InvoiceImporter.Function.Models;

public class ImportRequest
{
    public string FileName { get; set; }
    public int FileSize { get; set; }
    public string FileType { get; set; }
    public DateTimeOffset Timestamp { get; set; }
}