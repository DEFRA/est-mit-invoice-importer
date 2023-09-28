using System.ComponentModel.DataAnnotations;

namespace EST.MIT.InvoiceImporter.Function.Models;

public enum UploadStatus
{
    [Display(Name = "REQUIRED")]
    Required,

    [Display(Name = "UPLOADED")]
    Uploaded,

    [Display(Name = "UPLOADING")]
    Uploading,

    [Display(Name = "VALIDATING")]
    Validating,

    [Display(Name = "REJECTED")]
    Rejected,
}