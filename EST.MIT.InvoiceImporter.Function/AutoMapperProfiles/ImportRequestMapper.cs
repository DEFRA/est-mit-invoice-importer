using System;
using AutoMapper;
using EST.MIT.InvoiceImporter.Function.Models;
using EST.MIT.InvoiceImporter.Function.TableEntities;

namespace EST.MIT.InvoiceImporter.Function.AutoMapperProfiles;

public class ImportRequestMapper : Profile
{
    public ImportRequestMapper()
    {
        CreateMap<ImportRequestEntity, ImportRequest>()
            .ForMember(dest => dest.ImportRequestId, opt => opt.MapFrom(src => Guid.Parse(src.PartitionKey)))
            .ForMember(dest => dest.BlobPath, opt => opt.Ignore());

        CreateMap<ImportRequest, ImportRequestEntity>()
            .ForMember(dest => dest.PartitionKey, opt => opt.MapFrom(src => src.ImportRequestId.ToString()))
            .ForMember(dest => dest.RowKey, opt => opt.MapFrom(src => $"{src.ImportRequestId}_{src.Timestamp:O}"))
            .ForMember(dest => dest.ETag, opt => opt.Ignore());
    }
}
