using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using OnlineStore.Permissions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;                        
using Volo.Abp.Application.Dtos;
using Volo.Abp.Caching;
using Volo.Abp.MultiTenancy;

namespace OnlineStore.Categories
{
  
    [RemoteService(IsEnabled = true)] 
    [Authorize(OnlineStorePermissions.Categories.Default)]
    public class CachedCategoryAppService : ICategoriesAppService
    {
        private readonly CategoriesAppService _innerService;
        private readonly IDistributedCache<List<CategoryDto>> _listCache;
        private readonly IDistributedCache<CategoryDto> _singleCache;
        private readonly ICurrentTenant _currentTenant;
        private readonly ILogger<CachedCategoryAppService> _logger;

        public CachedCategoryAppService(
            CategoriesAppService innerService,
            IDistributedCache<List<CategoryDto>> listCache,
            IDistributedCache<CategoryDto> singleCache,
            ICurrentTenant currentTenant,
            ILogger<CachedCategoryAppService> logger)
        {
            _innerService = innerService;
            _listCache = listCache;
            _singleCache = singleCache;
            _currentTenant = currentTenant;
            _logger = logger;
        }
        [Authorize(OnlineStorePermissions.Categories.Default)]
        public async Task<PagedResultDto<CategoryDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            _logger.LogInformation("GetListAsync called with input: {@Input}", input);
            try
            {
                var result = await _innerService.GetListAsync(input);
                _logger.LogInformation("GetListAsync completed successfully. TotalCount: {TotalCount}", result.TotalCount);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetListAsync. Exception Type: {ExceptionType}, Message: {Message}", 
                    ex.GetType().Name, ex.Message);
                throw;
            }
        }

       
        public async Task<CategoryDto> GetAsync(int id)
        {
            var cacheKey = GetCategoryCacheKey(id);

            return await _singleCache.GetOrAddAsync(
                cacheKey,
                async () => await _innerService.GetAsync(id),
                () => new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                });
        }

       
        [Authorize(OnlineStorePermissions.Categories.Create)]  
        public async Task<CategoryDto> CreateAsync(CreateUpdateCategoryDto input)
        {
            var result = await _innerService.CreateAsync(input);
            await SafeInvalidateCacheAsync();
            return result;
        }

       
        [Authorize(OnlineStorePermissions.Categories.Edit)]  
        public async Task<CategoryDto> UpdateAsync(int id, CreateUpdateCategoryDto input)
        {
            var result = await _innerService.UpdateAsync(id, input);
            await SafeInvalidateCacheAsync(id);
            return result;
        }

     
        [Authorize(OnlineStorePermissions.Categories.Delete)]  
        public async Task DeleteAsync(int id)
        {
            await _innerService.DeleteAsync(id);
            await SafeInvalidateCacheAsync(id);
        }

        [Authorize(OnlineStorePermissions.Categories.Default)]
        public async Task<PagedResultDto<CategoryDto>> GetFilteredListAsync(GetCategoriesInput input)
        {
            _logger.LogInformation("GetFilteredListAsync called with input: {@Input}", input);
            try
            {
                _logger.LogInformation("Calling inner service GetFilteredListAsync");
                var result = await _innerService.GetFilteredListAsync(input);
                _logger.LogInformation("GetFilteredListAsync completed successfully. TotalCount: {TotalCount}, ItemsCount: {ItemsCount}", 
                    result.TotalCount, result.Items?.Count ?? 0);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetFilteredListAsync for input: {@Input}. Exception Type: {ExceptionType}, Message: {Message}, StackTrace: {StackTrace}", 
                    input, ex.GetType().Name, ex.Message, ex.StackTrace);
                throw;
            }
        }

        [AllowAnonymous] 
        public async Task<List<CategoryDto>> GetActiveListAsync()
        {
            var cacheKey = GetActiveCategoriesCacheKey();

            // GetOrAddAsync already handles cache check internally, no need for manual check
            return await _listCache.GetOrAddAsync(
                cacheKey,
                async () => 
                {
                    _logger.LogInformation("Cache MISS for key: {CacheKey}", cacheKey);
                    return await _innerService.GetActiveListAsync();
                },
                () => new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                });
        }

        [Authorize(OnlineStorePermissions.Categories.Delete)]  
        public async Task<bool> CanDeleteAsync(int id)
        {
            return await _innerService.CanDeleteAsync(id);
        }

        [Authorize(OnlineStorePermissions.Categories.Edit)] 
        public async Task<CategoryDto> ChangeDisplayOrderAsync(int id, int newOrder)
        {
            var result = await _innerService.ChangeDisplayOrderAsync(id, newOrder);
            await SafeInvalidateCacheAsync(id);
            return result;
        }

        [Authorize(OnlineStorePermissions.Categories.Edit)]  
        public async Task<CategoryDto> ActivateAsync(int id)
        {
            var result = await _innerService.ActivateAsync(id);
            await SafeInvalidateCacheAsync(id);
            return result;
        }

        [Authorize(OnlineStorePermissions.Categories.Edit)]  
        public async Task<CategoryDto> DeactivateAsync(int id)
        {
            var result = await _innerService.DeactivateAsync(id);
            await SafeInvalidateCacheAsync(id);
            return result;
        }

        // ==========================================
        // CACHE KEY HELPERS
        // ==========================================

        private string GetTenantId() => _currentTenant.Id?.ToString() ?? "host";
        
        private string GetCategoryCacheKey(int categoryId) => $"Categories:ById:{categoryId}:{GetTenantId()}";
        
        private string GetActiveCategoriesCacheKey() => $"Categories:Active:{GetTenantId()}";

        // ==========================================
        // CACHE INVALIDATION (WITH ERROR HANDLING)
        // ==========================================

        private async Task SafeInvalidateCacheAsync(int? categoryId = null)
        {
            try
            {
                await InvalidateCacheAsync(categoryId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, 
                    "Failed to invalidate cache for category {CategoryId}. " +
                    "Cache may be stale but operation succeeded.", 
                    categoryId);
                       }
        }

        private async Task InvalidateCacheAsync(int? categoryId = null)
        {
            await _listCache.RemoveAsync(GetActiveCategoriesCacheKey());

            if (categoryId.HasValue)
            {
                await _singleCache.RemoveAsync(GetCategoryCacheKey(categoryId.Value));
            }
        }
    }
}