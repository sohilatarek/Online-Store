using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnlineStore.Categories;
using OnlineStore.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace OnlineStore.Categories
{
   
    public class EfCoreCategoryRepository : EfCoreRepository<OnlineStoreDbContext, Category, int>, ICategoryRepository
    {
        private readonly ILogger<EfCoreCategoryRepository> _logger;

        public EfCoreCategoryRepository(
            IDbContextProvider<OnlineStoreDbContext> dbContextProvider,
            ILogger<EfCoreCategoryRepository> logger)
            : base(dbContextProvider)
        {
            _logger = logger;
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
             try
            {
                var dbContext = await GetDbContextAsync();
              
                var result = await dbContext.Products
                    .Where(p => !p.IsDeleted)
                    .GroupBy(p => p.CategoryId)
                    .Select(g => new { CategoryId = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.CategoryId, x => x.Count, cancellationToken);
                
                _logger.LogInformation("GetProductCountsAsync: Successfully retrieved product counts for {Count} categories", result?.Count ?? 0);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetProductCountsAsync: FAILED. Exception Type: {ExceptionType}, Message: {Message}, StackTrace: {StackTrace}",
                    ex.GetType().Name, ex.Message, ex.StackTrace);
                throw;
            }
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
            _logger.LogInformation("GetListAsync: Starting. skipCount={SkipCount}, maxResultCount={MaxResultCount}, sorting={Sorting}, isActive={IsActive}, searchTerm={SearchTerm}",
                skipCount, maxResultCount, sorting, isActive, searchTerm);
            try
            {
                var dbSet = await GetDbSetAsync();
                _logger.LogInformation("GetListAsync: DbSet retrieved");

                var query = dbSet.AsQueryable();

                // Apply filters
                if (isActive.HasValue)
                {
                    query = query.Where(c => c.IsActive == isActive.Value);
                    _logger.LogInformation("GetListAsync: Applied isActive filter");
                }

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query = query.Where(c =>
                        c.NameEn.Contains(searchTerm) ||
                        c.NameAr.Contains(searchTerm));
                    _logger.LogInformation("GetListAsync: Applied searchTerm filter");
                }

                // Apply sorting
                if (!string.IsNullOrWhiteSpace(sorting))
                {
                    query = query.OrderBy(sorting);
                    _logger.LogInformation("GetListAsync: Applied custom sorting: {Sorting}", sorting);
                }
                else
                {
                    query = query.OrderBy(c => c.DisplayOrder).ThenBy(c => c.NameEn);
                    _logger.LogInformation("GetListAsync: Applied default sorting");
                }

                // Apply pagination
                query = query.Skip(skipCount).Take(maxResultCount);
                _logger.LogInformation("GetListAsync: Applied pagination");

                var result = await query.ToListAsync(cancellationToken);
                _logger.LogInformation("GetListAsync: Completed successfully. Retrieved {Count} categories", result?.Count ?? 0);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetListAsync: FAILED. Exception Type: {ExceptionType}, Message: {Message}, StackTrace: {StackTrace}",
                    ex.GetType().Name, ex.Message, ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Gets the total count with filtering
        /// </summary>
        public async Task<long> GetCountAsync(
            bool? isActive = null,
            string searchTerm = null,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GetCountAsync: Starting. isActive={IsActive}, searchTerm={SearchTerm}", isActive, searchTerm);
            try
            {
                var dbSet = await GetDbSetAsync();
                _logger.LogInformation("GetCountAsync: DbSet retrieved");

                var query = dbSet.AsQueryable();

                // Apply filters
                if (isActive.HasValue)
                {
                    query = query.Where(c => c.IsActive == isActive.Value);
                    _logger.LogInformation("GetCountAsync: Applied isActive filter: {IsActive}", isActive.Value);
                }

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query = query.Where(c =>
                        c.NameEn.Contains(searchTerm) ||
                        c.NameAr.Contains(searchTerm));
                    _logger.LogInformation("GetCountAsync: Applied searchTerm filter: {SearchTerm}", searchTerm);
                }

                var count = await query.LongCountAsync(cancellationToken);
                _logger.LogInformation("GetCountAsync: Completed successfully. Count={Count}", count);
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetCountAsync: FAILED. Exception Type: {ExceptionType}, Message: {Message}",
                    ex.GetType().Name, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Gets the next available ID for a new category
        /// Handles soft-deleted categories and finds gaps in ID sequence
        /// Uses transaction with SERIALIZABLE isolation to prevent race conditions
        /// </summary>
        public async Task<int> GetNextIdAsync(CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            
                 using var transaction = await dbContext.Database.BeginTransactionAsync(
                IsolationLevel.Serializable, cancellationToken);
            
            try
            {         var dbSet = await GetDbSetAsync();
                var existingIds = await dbSet
                    .IgnoreQueryFilters() 
                    .Select(c => c.Id)
                    .OrderBy(id => id)
                    .ToListAsync(cancellationToken);
                
                int nextId;
                
                if (existingIds.Count == 0)
                {
                    nextId = 1; 
                }
                else
                {
                   
                    int candidateId = 1;
                    
                    foreach (var id in existingIds)
                    {
                        if (id == candidateId)
                        {
                            candidateId++;
                        }
                        else if (id > candidateId)
                        {
                           
                            break;
                        }
                    }
                    
                 
                    if (candidateId <= existingIds.Max())
                    {
                        candidateId = existingIds.Max() + 1;
                    }
                    
                    nextId = candidateId;
                }
                
                await transaction.CommitAsync(cancellationToken);
                return nextId;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}