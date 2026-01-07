using AutoMapper;
using OnlineStore.Products;

namespace OnlineStore.Mapping
{
   
    public class ProductsMappingProfile : Profile
    {
        public ProductsMappingProfile()
        {

            CreateMap<Product, ProductDto>()
                // Map Category navigation property fields
                .ForMember(dest => dest.CategoryNameAr,
                    opt => opt.MapFrom(src => src.Category != null ? src.Category.NameAr : null))
                .ForMember(dest => dest.CategoryNameEn,
                    opt => opt.MapFrom(src => src.Category != null ? src.Category.NameEn : null))
                .ForMember(dest => dest.CategoryDescriptionAr,
                    opt => opt.MapFrom(src => src.Category != null ? src.Category.DescriptionAr : null))
                .ForMember(dest => dest.CategoryDescriptionEn,
                    opt => opt.MapFrom(src => src.Category != null ? src.Category.DescriptionEn : null))
                // Calculated properties are handled by ProductDto itself
                .ForMember(dest => dest.InventoryValue, opt => opt.Ignore())
                .ForMember(dest => dest.IsOutOfStock, opt => opt.Ignore())
                .ForMember(dest => dest.IsLowStock, opt => opt.Ignore());

            CreateMap<CreateUpdateProductDto, Product>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) 
                .ForMember(dest => dest.Category, opt => opt.Ignore()) 
                .ForMember(dest => dest.TenantId, opt => opt.Ignore()) 
                .ForMember(dest => dest.CreationTime, opt => opt.Ignore()) 
                .ForMember(dest => dest.CreatorId, opt => opt.Ignore()) 
                .ForMember(dest => dest.LastModificationTime, opt => opt.Ignore()) 
                .ForMember(dest => dest.LastModifierId, opt => opt.Ignore()) 
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeleterId, opt => opt.Ignore()) 
                .ForMember(dest => dest.DeletionTime, opt => opt.Ignore())
                .ForMember(dest => dest.ConcurrencyStamp, opt => opt.Ignore())
                .ForMember(dest => dest.ExtraProperties, opt => opt.Ignore());
        }
    }
}