using OnlineStore.Categories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace OnlineStore.Products
{
    public interface IProductsAppService : ICrudAppService<ProductDto, int, PagedAndSortedResultRequestDto, CreateUpdateProductDto>
    {
       
        Task<List<ProductDto>> GetByCategoryAsync(int categoryId, bool onlyPublished = false);

        Task<List<ProductDto>> GetPublishedProductsAsync(int? categoryId = null);

        Task<ProductDto> PublishAsync(int id);

        Task<ProductDto> UnpublishAsync(int id);

        Task<ProductDto> UpdateStockAsync(int id, UpdateStockDto input);

        Task<ProductDto> AdjustStockAsync(int id, AdjustStockDto input);

        Task BulkUpdateStockAsync(BulkUpdateStockDto input);

        Task<StockCheckResultDto> CheckStockAsync(CheckStockInput input);

        Task<List<ProductDto>> GetLowStockAsync(int? threshold = null);

        Task<List<ProductDto>> GetOutOfStockAsync();
    }
}
