using AutoMapper;
using EbookStore.Application.DtoModels.Categories;
using EbookStore.Domain.Entities;

namespace EbookStore.Application.Common.Mappings.Catalog
{
    public class CategoryMappingProfile : Profile
    {
        public CategoryMappingProfile()
        {
            CreateMap<Category, CategoryDto>();

            CreateMap<CategoryCreateDto, Category>();
            CreateMap<CategoryUpdateDto, Category>();
        }
    }
}
