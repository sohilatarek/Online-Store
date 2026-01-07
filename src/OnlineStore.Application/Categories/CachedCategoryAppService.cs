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

       
        public async Task<PagedResultDto<CategoryDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            return await _innerService.GetListAsync(input);
        }

       
        public async Task<CategoryDto> GetAsync(int id)
        {
            var tenantId = _currentTenant.Id?.ToString() ?? "host";
            var cacheKey = $"Categories:ById:{id}:{tenantId}";

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
            await InvalidateCacheAsync();
            return result;
        }

       
        [Authorize(OnlineStorePermissions.Categories.Edit)]  
        public async Task<CategoryDto> UpdateAsync(int id, CreateUpdateCategoryDto input)
        {
            var result = await _innerService.UpdateAsync(id, input);
            await InvalidateCacheAsync(id);
            return result;
        }

     
        [Authorize(OnlineStorePermissions.Categories.Delete)]  
        public async Task DeleteAsync(int id)
        {
            await _innerService.DeleteAsync(id);
            await InvalidateCacheAsync(id);
        }

    

        public async Task<PagedResultDto<CategoryDto>> GetFilteredListAsync(GetCategoriesInput input)
        {
            return await _innerService.GetFilteredListAsync(input);
        }

        [AllowAnonymous] 
        public async Task<List<CategoryDto>> GetActiveListAsync()
        {
            var tenantId = _currentTenant.Id?.ToString() ?? "host";
            var cacheKey = $"Categories:Active:{tenantId}";

            var cached = await _listCache.GetAsync(cacheKey);
            if (cached != null)
            {
                _logger.LogInformation("Cache HIT for key: {CacheKey}", cacheKey);
                return cached;
            }

            _logger.LogInformation("Cache MISS for key: {CacheKey}", cacheKey);

            return await _listCache.GetOrAddAsync(
                cacheKey,
                async () => await _innerService.GetActiveListAsync(),
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
            await InvalidateCacheAsync(id);
            return result;
        }

        [Authorize(OnlineStorePermissions.Categories.Edit)]  
        public async Task<CategoryDto> ActivateAsync(int id)
        {
            var result = await _innerService.ActivateAsync(id);
            await InvalidateCacheAsync(id);
            return result;
        }

        [Authorize(OnlineStorePermissions.Categories.Edit)]  
        public async Task<CategoryDto> DeactivateAsync(int id)
        {
            var result = await _innerService.DeactivateAsync(id);
            await InvalidateCacheAsync(id);
            return result;
        }

        private async Task InvalidateCacheAsync(int? categoryId = null)
        {
            var tenantId = _currentTenant.Id?.ToString() ?? "host";
            await _listCache.RemoveAsync($"Categories:Active:{tenantId}");

            if (categoryId.HasValue)
            {
                await _singleCache.RemoveAsync($"Categories:ById:{categoryId.Value}:{tenantId}");
            }
        }
    }
}