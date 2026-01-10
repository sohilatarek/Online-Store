using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using OnlineStore.Permissions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Caching;
using Volo.Abp.MultiTenancy;

namespace OnlineStore.Products
{
    [RemoteService(IsEnabled = true)] 
    [Authorize(OnlineStorePermissions.Products.Default)]
    public class CachedProductAppService : IProductsAppService
    {
        private readonly ProductsAppService _innerService;
        private readonly IDistributedCache<List<ProductDto>> _listCache;
        private readonly IDistributedCache<ProductDto> _singleCache;
        private readonly ICurrentTenant _currentTenant;

        public CachedProductAppService(
            ProductsAppService innerService,
            IDistributedCache<List<ProductDto>> listCache,
            IDistributedCache<ProductDto> singleCache,
            ICurrentTenant currentTenant)
        {
            _innerService = innerService;
            _listCache = listCache;
            _singleCache = singleCache;
            _currentTenant = currentTenant;
        }

        public async Task<PagedResultDto<ProductDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            return await _innerService.GetListAsync(input);
        }

        public async Task<ProductDto> GetAsync(int id)
        {
            var tenantId = _currentTenant.Id?.ToString() ?? "host";
            var cacheKey = $"Products:ById:{id}:{tenantId}";

            return await _singleCache.GetOrAddAsync(
                cacheKey,
                async () => await _innerService.GetAsync(id),
                () => new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
                }
            );
        }

        [Authorize(OnlineStorePermissions.Products.Create)]  
        public async Task<ProductDto> CreateAsync(CreateUpdateProductDto input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            var result = await _innerService.CreateAsync(input);
            await InvalidateCacheAsync(result.CategoryId);
            return result;
        }

        [Authorize(OnlineStorePermissions.Products.Edit)]  
        public async Task<ProductDto> UpdateAsync(int id, CreateUpdateProductDto input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            var result = await _innerService.UpdateAsync(id, input);
            await InvalidateCacheAsync(result.CategoryId, id);
            return result;
        }

        [Authorize(OnlineStorePermissions.Products.Delete)] 
        public async Task DeleteAsync(int id)
        {
            var product = await _innerService.GetAsync(id);
            await _innerService.DeleteAsync(id);
            await InvalidateCacheAsync(product.CategoryId, id);
        }

        public async Task<List<ProductDto>> GetByCategoryAsync(int categoryId, bool onlyPublished = false)
        {
            var tenantId = _currentTenant.Id?.ToString() ?? "host";
            var cacheKey = $"Products:ByCategoryId:{categoryId}:{onlyPublished}:{tenantId}";

            return await _listCache.GetOrAddAsync(
                cacheKey,
                async () => await _innerService.GetByCategoryAsync(categoryId, onlyPublished),
                () => new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
                }
            );
        }

        [AllowAnonymous] 
        public async Task<List<ProductDto>> GetPublishedProductsAsync(int? categoryId = null)
        {
            var tenantId = _currentTenant.Id?.ToString() ?? "host";
            var categoryKey = categoryId?.ToString() ?? "all";
            var cacheKey = $"Products:Published:{categoryKey}:{tenantId}";

            return await _listCache.GetOrAddAsync(
                cacheKey,
                async () => await _innerService.GetPublishedProductsAsync(categoryId),
                () => new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
                }
            );
        }

        [Authorize(OnlineStorePermissions.Products.Publish)] 
        public async Task<ProductDto> PublishAsync(int id)
        {
            var result = await _innerService.PublishAsync(id);
            await InvalidateCacheAsync(result.CategoryId, id);
            return result;
        }

        [Authorize(OnlineStorePermissions.Products.Publish)] 
        public async Task<ProductDto> UnpublishAsync(int id)
        {
            var result = await _innerService.UnpublishAsync(id);
            await InvalidateCacheAsync(result.CategoryId, id);
            return result;
        }

        [Authorize(OnlineStorePermissions.Products.ManageStock)] 
        public async Task<ProductDto> UpdateStockAsync(int id, UpdateStockDto input)
        {
            var result = await _innerService.UpdateStockAsync(id, input);
            await InvalidateCacheAsync(result.CategoryId, id);
            return result;
        }

        [Authorize(OnlineStorePermissions.Products.ManageStock)] 
        public async Task<ProductDto> AdjustStockAsync(int id, AdjustStockDto input)
        {
            var result = await _innerService.AdjustStockAsync(id, input);
            await InvalidateCacheAsync(result.CategoryId, id);
            return result;
        }

        [Authorize(OnlineStorePermissions.Products.ManageStock)]  
        public async Task BulkUpdateStockAsync(BulkUpdateStockDto input)
        {
            await _innerService.BulkUpdateStockAsync(input);
            await InvalidateAllProductCachesAsync();
        }

        public async Task<StockCheckResultDto> CheckStockAsync(CheckStockInput input)
        {
            return await _innerService.CheckStockAsync(input);
        }

        public async Task<List<ProductDto>> GetLowStockAsync(int? threshold = null)
        {
            return await _innerService.GetLowStockAsync(threshold);
        }

        public async Task<List<ProductDto>> GetOutOfStockAsync()
        {
            return await _innerService.GetOutOfStockAsync();
        }

        private async Task InvalidateCacheAsync(int categoryId, int? productId = null)
        {
            var tenantId = _currentTenant.Id?.ToString() ?? "host";
            
            // Invalidate category-specific caches
            await _listCache.RemoveAsync($"Products:ByCategoryId:{categoryId}:true:{tenantId}");
            await _listCache.RemoveAsync($"Products:ByCategoryId:{categoryId}:false:{tenantId}");
            await _listCache.RemoveAsync($"Products:Published:{categoryId}:{tenantId}");
            
            // Invalidate global published products cache
            await _listCache.RemoveAsync($"Products:Published:all:{tenantId}");
            
            // Invalidate aggregate query caches
            await _listCache.RemoveAsync($"Products:LowStock:{tenantId}");
            await _listCache.RemoveAsync($"Products:OutOfStock:{tenantId}");

            // Invalidate single product cache if product ID provided
            if (productId.HasValue)
            {
                await _singleCache.RemoveAsync($"Products:ById:{productId.Value}:{tenantId}");
            }
        }

        private async Task InvalidateAllProductCachesAsync()
        {
            var tenantId = _currentTenant.Id?.ToString() ?? "host";
            
            // Invalidate all published products caches
            await _listCache.RemoveAsync($"Products:Published:all:{tenantId}");
            
            // Invalidate aggregate query caches
            await _listCache.RemoveAsync($"Products:LowStock:{tenantId}");
            await _listCache.RemoveAsync($"Products:OutOfStock:{tenantId}");
        }
    }
}