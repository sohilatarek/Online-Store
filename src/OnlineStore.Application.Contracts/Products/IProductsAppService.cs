using OnlineStore.Categories;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace OnlineStore.Products
{
    public interface IProductsAppService : ICrudAppService<ProductDto, int, PagedAndSortedResultRequestDto, CreateUpdateProductDto>
    {
    }
}
