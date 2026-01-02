using AutoMapper;
using OnlineStore.Categories;

namespace OnlineStore.Mapping
{
    public class CategoriesMappingProfile : Profile
    {
        public CategoriesMappingProfile()
        {
            CreateMap<Category, CategoryDto>();
            CreateMap<CreateUpdateCategoryDto, Category>();
        }
    }
}
