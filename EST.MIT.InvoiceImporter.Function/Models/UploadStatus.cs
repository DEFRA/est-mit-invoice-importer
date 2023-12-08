using System.ComponentModel.DataAnnotations;

namespace EST.MIT.InvoiceImporter.Function.Models;

public enum UploadStatus
{
    [Display(Name = "Upload successful")]
    Upload_successful,

    [Display(Name = "Upload failed")]
    Upload_failed,

    [Display(Name = "Upload validated")]
    Upload_validated,

    [Display(Name = "Uploaded")]
    Uploaded
}