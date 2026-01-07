using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Categories;
using OnlineStore.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace OnlineStore.Repositories
{
   
    public class EfCoreCategoryRepository : EfCoreRepository<OnlineStoreDbContext, Category, int>, ICategoryRepository
    {
        public EfCoreCategoryRepository(IDbContextProvider<OnlineStoreDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        /// <summary>
        /// Gets all active categories ordered by DisplayOrder
        /// Uses IX_Categories_IsActive index
        /// </summary>
        public async Task<List<Category>> GetActiveListAsync(CancellationToken cancellationToken = default)
        {
            var dbSet = await GetDbSetAsync();

            return await dbSet
                .Where(c => c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .ThenBy(c => c.NameEn)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Checks if a category name is unique
        /// Uses UQ_Categories_NameEn_TenantId index
        /// </summary>
        public async Task<bool> IsNameUniqueAsync(
            string name,
            int? excludeId = null,
            CancellationToken cancellationToken = default)
        {
            var dbSet = await GetDbSetAsync();

            var query = dbSet.Where(c => c.NameEn == name);

            if (excludeId.HasValue)
            {
                query = query.Where(c => c.Id != excludeId.Value);
            }

            return !await query.AnyAsync(cancellationToken);
        }

        public async Task<Category> FindByNameAsync(
            string name,
            CancellationToken cancellationToken = default)
        {
            var dbSet = await GetDbSetAsync();

            return await dbSet
                .Where(c => c.NameEn == name)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Dictionary<int, int>> GetProductCountsAsync(CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            return await dbContext.Products
                .Where(p => !p.IsDeleted)
                .GroupBy(p => p.CategoryId)
                .Select(g => new { CategoryId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.CategoryId, x => x.Count, cancellationToken);
        }

        public async Task<int> GetProductCountAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            return await dbContext.Products
                .Where(p => p.CategoryId == categoryId && !p.IsDeleted)
                .CountAsync(cancellationToken);
        }

        public async Task<bool> HasProductsAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            return await dbContext.Products
                .Where(p => p.CategoryId == categoryId && !p.IsDeleted)
                .AnyAsync(cancellationToken);
        }

        /// <summary>
        /// Gets categories with pagination and filtering
        /// Uses multiple indexes for optimization
        /// </summary>
        public async Task<List<Category>> GetListAsync(
            int skipCount,
            int maxResultCount,
            string sorting = null,
            bool? isActive = null,
            string searchTerm = null,
            CancellationToken cancellationToken = default)
        {
            var dbSet = await GetDbSetAsync();

            var query = dbSet.AsQueryable();

            // Apply filters
            if (isActive.HasValue)
            {
                query = query.Where(c => c.IsActive == isActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(c =>
                    c.NameEn.Contains(searchTerm) ||
                    c.NameAr.Contains(searchTerm));
            }

            // Apply sorting
            if (!string.IsNullOrWhiteSpace(sorting))
            {
                query = query.OrderBy(sorting);
            }
            else
            {
                query = query.OrderBy(c => c.DisplayOrder).ThenBy(c => c.NameEn);
            }

            // Apply pagination
            query = query.Skip(skipCount).Take(maxResultCount);

            return await query.ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Gets the total count with filtering
        /// </summary>
        public async Task<long> GetCountAsync(
            bool? isActive = null,
            string searchTerm = null,
            CancellationToken cancellationToken = default)
        {
            var dbSet = await GetDbSetAsync();

            var query = dbSet.AsQueryable();

            // Apply filters
            if (isActive.HasValue)
            {
                query = query.Where(c => c.IsActive == isActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(c =>
                    c.NameEn.Contains(searchTerm) ||
                    c.NameAr.Contains(searchTerm));
            }

            return await query.LongCountAsync(cancellationToken);
        }
    }
}