using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OnlineStore.EntityFrameworkCore;
using OnlineStore.Products;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace OnlineStore.Repositories
{
 
    public class EfCoreProductRepository : EfCoreRepository<OnlineStoreDbContext, Product, int>, IProductRepository
    {
        public EfCoreProductRepository(IDbContextProvider<OnlineStoreDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
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

        /// <summary>
        /// Checks if SKU is unique
        /// Uses UQ_Products_SKU_TenantId index
        /// </summary>
        public async Task<bool> IsSKUUniqueAsync(
            string sku,
            int? excludeId = null,
            CancellationToken cancellationToken = default)
        {
            var dbSet = await GetDbSetAsync();

            var query = dbSet.Where(p => p.SKU == sku);

            if (excludeId.HasValue)
            {
                query = query.Where(p => p.Id != excludeId.Value);
            }

            return !await query.AnyAsync(cancellationToken);
        }

        /// <summary>
        /// Finds a product by SKU
        /// Uses IX_Products_SKU index
        /// </summary>
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

        /// <summary>
        /// Gets products with low stock
        /// Uses IX_Products_StockQuantity index
        /// </summary>
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

        /// <summary>
        /// Gets products with advanced filtering and pagination
        /// Uses multiple indexes for optimal performance
        /// </summary>
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

        /// <summary>
        /// Gets the total count with filtering
        /// </summary>
        public async Task<long> GetCountAsync(
            int? categoryId = null,
            bool? isActive = null,
            bool? isPublished = null,
            string searchTerm = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
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

            return await query.LongCountAsync(cancellationToken);
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
    }
}