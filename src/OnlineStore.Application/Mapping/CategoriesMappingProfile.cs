using AutoMapper;
using OnlineStore.Categories;

namespace OnlineStore.Mapping
{
    public class CategoriesMappingProfile : Profile
    {
        public CategoriesMappingProfile()
        {
           

            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.ProductCount, opt => opt.Ignore()); // Populated separately when needed

            CreateMap<CreateUpdateCategoryDto, Category>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) 
                .ForMember(dest => dest.TenantId, opt => opt.Ignore()) 
                .ForMember(dest => dest.CreationTime, opt => opt.Ignore()) 
                .ForMember(dest => dest.CreatorId, opt => opt.Ignore()) 
                .ForMember(dest => dest.LastModificationTime, opt => opt.Ignore()) 
                .ForMember(dest => dest.LastModifierId, opt => opt.Ignore()) 
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeleterId, opt => opt.Ignore())
                .ForMember(dest => dest.DeletionTime, opt => opt.Ignore()) 
                .ForMember(dest => dest.ConcurrencyStamp, opt => opt.Ignore()) 
                .ForMember(dest => dest.ExtraProperties, opt => opt.Ignore()); // Handled by ABP
        }
    }
}