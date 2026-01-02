using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace OnlineStore.Products
{
    public class ProductsAppService : CrudAppService<Product, ProductDto, int, PagedAndSortedResultRequestDto, CreateUpdateProductDto>, IProductsAppService
    {
        public ProductsAppService(IRepository<Product, int> repository) : base(repository)
        {
        }
    }
}
