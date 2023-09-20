using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using EST.MIT.InvoiceImporter.Function.Models;
using EST.MIT.InvoiceImporter.Function.TableEntities;

namespace EST.MIT.InvoiceImporter.Function.AutoMapperProfiles;

[ExcludeFromCodeCoverage]
public class ImportRequestMapper : Profile
{
    public ImportRequestMapper()
    {
        CreateMap<ImportRequestEntity, ImportRequest>();

    }
}
