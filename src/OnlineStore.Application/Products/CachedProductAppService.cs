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
        private readonly ILogger<CachedProductAppService> _logger;

        public CachedProductAppService(
            ProductsAppService innerService,
            IDistributedCache<List<ProductDto>> listCache,
            IDistributedCache<ProductDto> singleCache,
            ICurrentTenant currentTenant,
            ILogger<CachedProductAppService> logger)
        {
            _innerService = innerService;
            _listCache = listCache;
            _singleCache = singleCache;
            _currentTenant = currentTenant;
            _logger = logger;
        }

        public async Task<PagedResultDto<ProductDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            return await _innerService.GetListAsync(input);
        }

        public async Task<ProductDto> GetAsync(int id)
        {
            var cacheKey = GetProductCacheKey(id);

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
            await SafeInvalidateCacheAsync(result.CategoryId);
            return result;
        }

        [Authorize(OnlineStorePermissions.Products.Edit)]  
        public async Task<ProductDto> UpdateAsync(int id, CreateUpdateProductDto input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            // Get old product to capture old category ID for cache invalidation
            var oldProduct = await _innerService.GetAsync(id);
            var oldCategoryId = oldProduct.CategoryId;

            var result = await _innerService.UpdateAsync(id, input);
            
            // Invalidate both old and new category caches if category changed
            await SafeInvalidateCacheAsync(oldCategoryId, id);
            if (oldCategoryId != result.CategoryId)
            {
                await SafeInvalidateCacheAsync(result.CategoryId, id);
            }
            
            return result;
        }

        [Authorize(OnlineStorePermissions.Products.Delete)] 
        public async Task DeleteAsync(int id)
        {
            // Get category ID before deletion (more efficient than full GetAsync)
            var product = await _innerService.GetAsync(id);
            var categoryId = product.CategoryId;
            
            await _innerService.DeleteAsync(id);
            await SafeInvalidateCacheAsync(categoryId, id);
        }

        public async Task<List<ProductDto>> GetByCategoryAsync(int categoryId, bool onlyPublished = false)
        {
            var cacheKey = GetProductsByCategoryCacheKey(categoryId, onlyPublished);

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
            var cacheKey = GetPublishedProductsCacheKey(categoryId);

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
            await SafeInvalidateCacheAsync(result.CategoryId, id);
            return result;
        }

        [Authorize(OnlineStorePermissions.Products.Publish)] 
        public async Task<ProductDto> UnpublishAsync(int id)
        {
            var result = await _innerService.UnpublishAsync(id);
            await SafeInvalidateCacheAsync(result.CategoryId, id);
            return result;
        }

        [Authorize(OnlineStorePermissions.Products.ManageStock)] 
        public async Task<ProductDto> UpdateStockAsync(int id, UpdateStockDto input)
        {
            var result = await _innerService.UpdateStockAsync(id, input);
            await SafeInvalidateCacheAsync(result.CategoryId, id);
            return result;
        }

        [Authorize(OnlineStorePermissions.Products.ManageStock)] 
        public async Task<ProductDto> AdjustStockAsync(int id, AdjustStockDto input)
        {
            var result = await _innerService.AdjustStockAsync(id, input);
            await SafeInvalidateCacheAsync(result.CategoryId, id);
            return result;
        }

        [Authorize(OnlineStorePermissions.Products.ManageStock)]  
        public async Task BulkUpdateStockAsync(BulkUpdateStockDto input)
        {
            await _innerService.BulkUpdateStockAsync(input);
            await SafeInvalidateAllProductCachesAsync();
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

        // ==========================================
        // CACHE KEY HELPERS
        // ==========================================

        private string GetTenantId() => _currentTenant.Id?.ToString() ?? "host";
        
        private string GetProductCacheKey(int productId) => $"Products:ById:{productId}:{GetTenantId()}";
        
        private string GetProductsByCategoryCacheKey(int categoryId, bool onlyPublished) => 
            $"Products:ByCategoryId:{categoryId}:{onlyPublished}:{GetTenantId()}";
        
        private string GetPublishedProductsCacheKey(int? categoryId)
        {
            var categoryKey = categoryId?.ToString() ?? "all";
            return $"Products:Published:{categoryKey}:{GetTenantId()}";
        }

        // ==========================================
        // CACHE INVALIDATION (WITH ERROR HANDLING)
        // ==========================================

        private async Task SafeInvalidateCacheAsync(int categoryId, int? productId = null)
        {
            try
            {
                await InvalidateCacheAsync(categoryId, productId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, 
                    "Failed to invalidate cache for category {CategoryId}, product {ProductId}. " +
                    "Cache may be stale but operation succeeded.", 
                    categoryId, productId);
                // Don't throw - cache invalidation failure shouldn't break the operation
            }
        }

        private async Task SafeInvalidateAllProductCachesAsync()
        {
            try
            {
                await InvalidateAllProductCachesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, 
                    "Failed to invalidate all product caches. " +
                    "Cache may be stale but operation succeeded.");
                // Don't throw - cache invalidation failure shouldn't break the operation
            }
        }

        private async Task InvalidateCacheAsync(int categoryId, int? productId = null)
        {
            // Invalidate category-specific caches
            await _listCache.RemoveAsync(GetProductsByCategoryCacheKey(categoryId, true));
            await _listCache.RemoveAsync(GetProductsByCategoryCacheKey(categoryId, false));
            await _listCache.RemoveAsync(GetPublishedProductsCacheKey(categoryId));
            
            // Invalidate global published products cache
            await _listCache.RemoveAsync(GetPublishedProductsCacheKey(null));
            
            // Invalidate aggregate query caches
            var tenantId = GetTenantId();
            await _listCache.RemoveAsync($"Products:LowStock:{tenantId}");
            await _listCache.RemoveAsync($"Products:OutOfStock:{tenantId}");

            // Invalidate single product cache if product ID provided
            if (productId.HasValue)
            {
                await _singleCache.RemoveAsync(GetProductCacheKey(productId.Value));
            }
        }

        private async Task InvalidateAllProductCachesAsync()
        {
            var tenantId = GetTenantId();
            
            // Invalidate all published products caches
            await _listCache.RemoveAsync(GetPublishedProductsCacheKey(null));
            
            // Invalidate aggregate query caches
            await _listCache.RemoveAsync($"Products:LowStock:{tenantId}");
            await _listCache.RemoveAsync($"Products:OutOfStock:{tenantId}");
        }
    }
}