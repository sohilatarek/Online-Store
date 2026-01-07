using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace OnlineStore.Products
{
    public interface IProductRepository : IRepository<Product, int>
    {
      
        Task<List<Product>> GetByCategoryAsync(
            int categoryId,
            bool onlyPublished = false,
            CancellationToken cancellationToken = default);

        
        Task<bool> IsSKUUniqueAsync(
            string sku,
            int? excludeId = null,
            CancellationToken cancellationToken = default);

        Task<Product> FindBySKUAsync(
            string sku,
            CancellationToken cancellationToken = default);

        Task<List<Product>> GetLowStockAsync(
            int threshold = 10,
            CancellationToken cancellationToken = default);

      
        Task<decimal> GetTotalInventoryValueAsync(CancellationToken cancellationToken = default);

     
        Task<Dictionary<int, int>> GetStockByCategoryAsync(CancellationToken cancellationToken = default);

        Task<List<Product>> GetListAsync(
            int skipCount,
            int maxResultCount,
            string sorting = null,
            int? categoryId = null,
            bool? isActive = null,
            bool? isPublished = null,
            string searchTerm = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            bool? isLowStock = null,
            int? lowStockThreshold = null,
            bool? isOutOfStock = null,
            CancellationToken cancellationToken = default);

        Task<long> GetCountAsync(
            int? categoryId = null,
            bool? isActive = null,
            bool? isPublished = null,
            string searchTerm = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            bool? isLowStock = null,
            int? lowStockThreshold = null,
            bool? isOutOfStock = null,
            CancellationToken cancellationToken = default);

        Task<Product> GetWithCategoryAsync(
            int id,
            bool includeCategory = true,
            CancellationToken cancellationToken = default);

        Task<List<Product>> GetOutOfStockAsync(CancellationToken cancellationToken = default);

        Task<List<Product>> GetPublishedProductsAsync(
            int? categoryId = null,
            CancellationToken cancellationToken = default);

        Task<List<Product>> GetByIdsAsync(
            List<int> ids,
            CancellationToken cancellationToken = default);
    }
}