using AutoMapper;
using EbookStore.Application.DtoModels.Ebooks;
using EbookStore.Domain.Entities;

namespace EbookStore.Application.Common.Mappings.Catalog;

public class EbookMappingProfile : Profile
{
    public EbookMappingProfile()
    {
        CreateMap<Ebook, EbookDto>()
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category));

        CreateMap<EbookCreateDto, Ebook>();
        CreateMap<EbookUpdateDto, Ebook>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
    }
}