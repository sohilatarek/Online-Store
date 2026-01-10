using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OnlineStore;
using OnlineStore.EntityFrameworkCore;
using OnlineStore.Products;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.MultiTenancy;

namespace OnlineStore.Products
{
 
    public class EfCoreProductRepository : EfCoreRepository<OnlineStoreDbContext, Product, int>, IProductRepository
    {
        private readonly ICurrentTenant _currentTenant;

        public EfCoreProductRepository(
            IDbContextProvider<OnlineStoreDbContext> dbContextProvider,
            ICurrentTenant currentTenant)
            : base(dbContextProvider)
        {
            _currentTenant = currentTenant;
        }

        /// <summary>
        /// Gets products by category with optional published filter
        /// Uses IX_Products_CategoryId_IsActive index
        /// </summary>
        public async Task<List<Product>> GetByCategoryAsync(
            int categoryId,
            bool onlyPublished = false,
            CancellationToken cancellationToken = default)
        {
            var dbSet = await GetDbSetAsync();

            var query = dbSet
                .Include(p => p.Category)
                .Where(p => p.CategoryId == categoryId);

            if (onlyPublished)
            {
                query = query.Where(p => p.IsActive && p.IsPublished);
            }

            return await query
                .OrderBy(p => p.NameEn)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> IsSKUUniqueAsync(
            string sku,
            int? excludeId = null,
            CancellationToken cancellationToken = default)
        {
            var dbSet = await GetDbSetAsync();

            var query = dbSet.Where(p => p.SKU == sku);

            // Apply tenant filtering for multi-tenancy support
            if (_currentTenant.Id.HasValue)
            {
                query = query.Where(p => p.TenantId == _currentTenant.Id);
            }
            else
            {
                query = query.Where(p => p.TenantId == null);
            }

            if (excludeId.HasValue)
            {
                query = query.Where(p => p.Id != excludeId.Value);
            }

            return !await query.AnyAsync(cancellationToken);
        }

        public async Task<Product> FindBySKUAsync(
            string sku,
            CancellationToken cancellationToken = default)
        {
            var dbSet = await GetDbSetAsync();

            return await dbSet
                .Include(p => p.Category)
                .Where(p => p.SKU == sku)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<Product>> GetLowStockAsync(
            int threshold = 10,
            CancellationToken cancellationToken = default)
        {
            var dbSet = await GetDbSetAsync();

            return await dbSet
                .Include(p => p.Category)
                .Where(p => p.StockQuantity > 0 && p.StockQuantity <= threshold)
                .OrderBy(p => p.StockQuantity)
                .ThenBy(p => p.NameEn)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Calculates total inventory value
        /// Optimized aggregate query
        /// </summary>
        public async Task<decimal> GetTotalInventoryValueAsync(CancellationToken cancellationToken = default)
        {
            var dbSet = await GetDbSetAsync();

            return await dbSet
                .Where(p => !p.IsDeleted)
                .SumAsync(p => p.Price * p.StockQuantity, cancellationToken);
        }

        public async Task<Dictionary<int, int>> GetStockByCategoryAsync(CancellationToken cancellationToken = default)
        {
            var dbSet = await GetDbSetAsync();

            return await dbSet
                .Where(p => !p.IsDeleted)
                .GroupBy(p => p.CategoryId)
                .Select(g => new { CategoryId = g.Key, TotalStock = g.Sum(p => p.StockQuantity) })
                .ToDictionaryAsync(x => x.CategoryId, x => x.TotalStock, cancellationToken);
        }

        public async Task<List<Product>> GetListAsync(
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
            CancellationToken cancellationToken = default)
        {
            var dbSet = await GetDbSetAsync();

            var query = dbSet
                .Include(p => p.Category)
                .AsQueryable();

            // Apply filters
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            if (isActive.HasValue)
            {
                query = query.Where(p => p.IsActive == isActive.Value);
            }

            if (isPublished.HasValue)
            {
                query = query.Where(p => p.IsPublished == isPublished.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p =>
                    p.NameEn.Contains(searchTerm) ||
                    p.NameAr.Contains(searchTerm) ||
                    p.SKU.Contains(searchTerm));
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            if (isOutOfStock == true)
            {
                query = query.Where(p => p.StockQuantity == 0);
            }
            else if (isLowStock == true)
            {
                var threshold = lowStockThreshold ?? OnlineStore.OnlineStoreConsts.DefaultLowStockThreshold;
                query = query.Where(p => p.StockQuantity > 0 && p.StockQuantity <= threshold);
            }

            // Apply sorting
            if (!string.IsNullOrWhiteSpace(sorting))
            {
                query = query.OrderBy(sorting);
            }
            else
            {
                query = query.OrderByDescending(p => p.CreationTime);
            }

            // Apply pagination
            query = query.Skip(skipCount).Take(maxResultCount);

            return await query.ToListAsync(cancellationToken);
        }

    
        public async Task<long> GetCountAsync(
            int? categoryId = null,
            bool? isActive = null,
            bool? isPublished = null,
            string searchTerm = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            bool? isLowStock = null,
            int? lowStockThreshold = null,
            bool? isOutOfStock = null,
            CancellationToken cancellationToken = default)
        {
            var dbSet = await GetDbSetAsync();

            var query = dbSet.AsQueryable();

            // Apply same filters as GetListAsync
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            if (isActive.HasValue)
            {
                query = query.Where(p => p.IsActive == isActive.Value);
            }

            if (isPublished.HasValue)
            {
                query = query.Where(p => p.IsPublished == isPublished.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p =>
                    p.NameEn.Contains(searchTerm) ||
                    p.NameAr.Contains(searchTerm) ||
                    p.SKU.Contains(searchTerm));
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

           
            if (isOutOfStock == true)
            {
                query = query.Where(p => p.StockQuantity == 0);
            }
            else if (isLowStock == true)
            {
                var threshold = lowStockThreshold ?? OnlineStore.OnlineStoreConsts.DefaultLowStockThreshold;
                query = query.Where(p => p.StockQuantity > 0 && p.StockQuantity <= threshold);
            }

            return await query.LongCountAsync(cancellationToken);
        }
        
       
        public async Task<Product> GetWithCategoryAsync(
            int id,
            bool includeCategory = true,
            CancellationToken cancellationToken = default)
        {
            var dbSet = await GetDbSetAsync();

            var query = dbSet.AsQueryable();

            if (includeCategory)
            {
                query = query.Include(p => p.Category);
            }

            return await query.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        /// <summary>
        /// Gets products that are out of stock
        /// Uses IX_Products_StockQuantity index
        /// </summary>
        public async Task<List<Product>> GetOutOfStockAsync(CancellationToken cancellationToken = default)
        {
            var dbSet = await GetDbSetAsync();

            return await dbSet
                .Include(p => p.Category)
                .Where(p => p.StockQuantity == 0 && p.IsActive)
                .OrderBy(p => p.NameEn)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Gets active and published products for public display
        /// Uses IX_Products_TenantId_IsActive_IsPublished composite index
        /// </summary>
        public async Task<List<Product>> GetPublishedProductsAsync(
            int? categoryId = null,
            CancellationToken cancellationToken = default)
        {
            var dbSet = await GetDbSetAsync();

            var query = dbSet
                .Include(p => p.Category)
                .Where(p => p.IsActive && p.IsPublished);

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            return await query
                .OrderBy(p => p.NameEn)
                .ToListAsync(cancellationToken);
        }

   
        public async Task<List<Product>> GetByIdsAsync(
            List<int> ids,
            CancellationToken cancellationToken = default)
        {
            if (ids == null || ids.Count == 0)
            {
                return new List<Product>();
            }

            var dbSet = await GetDbSetAsync();

            return await dbSet
                .Include(p => p.Category)
                .Where(p => ids.Contains(p.Id))
                .ToListAsync(cancellationToken);
        }
    }
}