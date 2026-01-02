using AutoMapper;
using OnlineStore.Categories;
using OnlineStore.Products;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineStore.Mapping
{
    public class ProductsMappingProfile : Profile
    {
        public ProductsMappingProfile() {
            CreateMap<Product, ProductDto>();
            CreateMap<CreateUpdateProductDto, Product>();
        }
    }
}
