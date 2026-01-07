using Microsoft.AspNetCore.Authorization;
using OnlineStore.Permissions;
using OnlineStore.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;

namespace OnlineStore.Products
{
    [RemoteService(false)]
  
    [Authorize(OnlineStorePermissions.Products.Default)]
    public class ProductsAppService : CrudAppService<Product, ProductDto, int, PagedAndSortedResultRequestDto, CreateUpdateProductDto>, IProductsAppService
    {
        private readonly IProductRepository _productRepository;
        private readonly ProductManager _productManager;

        public ProductsAppService(
            IProductRepository repository,
            ProductManager productManager) : base(repository)
        {
            _productRepository = repository;
            _productManager = productManager;

            // Set default permissions
            GetPolicyName = OnlineStorePermissions.Products.Default;
            GetListPolicyName = OnlineStorePermissions.Products.Default;
            CreatePolicyName = OnlineStorePermissions.Products.Create;
            UpdatePolicyName = OnlineStorePermissions.Products.Edit;
            DeletePolicyName = OnlineStorePermissions.Products.Delete;
        }

     
        [Authorize(OnlineStorePermissions.Products.Default)]
        public override async Task<PagedResultDto<ProductDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
           
            var productsInput = input as GetProductsInput ?? new GetProductsInput
            {
                SkipCount = input.SkipCount,
                MaxResultCount = input.MaxResultCount,
                Sorting = input.Sorting
            };

        
            var totalCount = await _productRepository.GetCountAsync(
                productsInput.CategoryId,
                productsInput.IsActive,
                productsInput.IsPublished,
                productsInput.SearchTerm,
                productsInput.MinPrice,
                productsInput.MaxPrice,
                productsInput.IsLowStock,
                productsInput.IsLowStock == true ? productsInput.LowStockThreshold : (int?)null,
                productsInput.IsOutOfStock
            );

            var products = await _productRepository.GetListAsync(
                productsInput.SkipCount,
                productsInput.MaxResultCount,
                productsInput.Sorting ?? OnlineStore.OnlineStoreConsts.DefaultProductSorting,
                productsInput.CategoryId,
                productsInput.IsActive,
                productsInput.IsPublished,
                productsInput.SearchTerm,
                productsInput.MinPrice,
                productsInput.MaxPrice,
                productsInput.IsLowStock,
                productsInput.IsLowStock == true ? productsInput.LowStockThreshold : (int?)null,
                productsInput.IsOutOfStock
            );

            return new PagedResultDto<ProductDto>(
                totalCount,
                ObjectMapper.Map<List<Product>, List<ProductDto>>(products)
            );
        }


        [Authorize(OnlineStorePermissions.Products.Default)]
        public override async Task<ProductDto> GetAsync(int id)
        {
          
            var product = await _productRepository.GetWithCategoryAsync(id, includeCategory: true);

            if (product == null)
            {
                throw new EntityNotFoundException(typeof(Product), id);
            }

            return ObjectMapper.Map<Product, ProductDto>(product);
        }

      
        [Authorize(OnlineStorePermissions.Products.Default)]
        public async Task<List<ProductDto>> GetByCategoryAsync(int categoryId, bool onlyPublished = false)
        {
            var products = await _productRepository.GetByCategoryAsync(categoryId, onlyPublished);
            return ObjectMapper.Map<List<Product>, List<ProductDto>>(products);
        }

        [AllowAnonymous]
        public async Task<List<ProductDto>> GetPublishedProductsAsync(int? categoryId = null)
        {
            var products = await _productRepository.GetPublishedProductsAsync(categoryId);
            return ObjectMapper.Map<List<Product>, List<ProductDto>>(products);
        }

        [Authorize(OnlineStorePermissions.Products.Create)]
        public override async Task<ProductDto> CreateAsync(CreateUpdateProductDto input)
        {if (input == null)
        {
           throw new UserFriendlyException(L["Product:InvalidInput"]);
        }

            var product = await _productManager.CreateAsync(
                input.NameAr,
                input.NameEn,
                input.DescriptionAr,
                input.DescriptionEn,
                input.CategoryId,
                input.SKU,
                input.Price,
                input.StockQuantity,
                input.IsActive,
                input.IsPublished
            );

            await _productRepository.InsertAsync(product);
            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<Product, ProductDto>(product);
        }

        
        [Authorize(OnlineStorePermissions.Products.Edit)]
        public override async Task<ProductDto> UpdateAsync(int id, CreateUpdateProductDto input)
        {
            if (input == null)
            {
                throw new UserFriendlyException(L["Product:InvalidInput"]);
            }

            var product = await _productRepository.GetAsync(id);

            if (product.SKU != input.SKU)
            {
                await _productManager.ChangeSKUAsync(product, input.SKU);
            }

            await _productManager.UpdateAsync(
                product,
                input.NameAr,
                input.NameEn,
                input.DescriptionAr,
                input.DescriptionEn,
                input.CategoryId,
                input.Price,
                input.StockQuantity
            );

            if (input.IsActive != product.IsActive)
            {
                if (input.IsActive)
                    _productManager.Activate(product);
                else
                    _productManager.Deactivate(product);
            }

            if (input.IsPublished != product.IsPublished)
            {
                if (input.IsPublished)
                    await _productManager.PublishAsync(product);
                else
                    _productManager.Unpublish(product);
            }

            await _productRepository.UpdateAsync(product);
            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<Product, ProductDto>(product);
        }

        [Authorize(OnlineStorePermissions.Products.Delete)]
        public override async Task DeleteAsync(int id)
        {
            await _productRepository.DeleteAsync(id);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        [Authorize(OnlineStorePermissions.Products.Publish)]
        public async Task<ProductDto> PublishAsync(int id)
        {
            var product = await _productRepository.GetAsync(id);
            await _productManager.PublishAsync(product);
            await _productRepository.UpdateAsync(product);
            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<Product, ProductDto>(product);
        }

       
        [Authorize(OnlineStorePermissions.Products.Publish)]
        public async Task<ProductDto> UnpublishAsync(int id)
        {
            var product = await _productRepository.GetAsync(id);
            _productManager.Unpublish(product);
            await _productRepository.UpdateAsync(product);
            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<Product, ProductDto>(product);
        }

      
        [Authorize(OnlineStorePermissions.Products.ManageStock)]
        public async Task<ProductDto> UpdateStockAsync(int id, UpdateStockDto input)
        {
            if (input == null)
            {
                throw new UserFriendlyException(L["Product:InvalidInput"]);
            }

            var product = await _productRepository.GetAsync(id);
            _productManager.UpdateStock(product, input.StockQuantity);
            await _productRepository.UpdateAsync(product);
            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<Product, ProductDto>(product);
        }

        [Authorize(OnlineStorePermissions.Products.ManageStock)]
        public async Task<ProductDto> AdjustStockAsync(int id, AdjustStockDto input)
        {
            if (input == null)
            {
                throw new UserFriendlyException(L["Product:InvalidInput"]);
            }

            var product = await _productRepository.GetAsync(id);
            _productManager.AdjustStock(product, input.QuantityChange);
            await _productRepository.UpdateAsync(product);
            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<Product, ProductDto>(product);
        }

      
        [Authorize(OnlineStorePermissions.Products.ManageStock)]
        public async Task BulkUpdateStockAsync(BulkUpdateStockDto input)
        {
            if (input == null)
            {
                throw new UserFriendlyException(L["Product:InvalidInput"]);
            }

            if (input.Items == null || input.Items.Count == 0)
            {
                throw new ArgumentException("Items list cannot be null or empty", nameof(input));
            }

           
            var productIds = input.Items.Select(i => i.ProductId).Distinct().ToList();
            var products = await _productRepository.GetByIdsAsync(productIds);
            var productDict = products.ToDictionary(p => p.Id);

            
            var missingProducts = input.Items
                .Where(item => !productDict.ContainsKey(item.ProductId))
                .Select(item => item.ProductId)
                .ToList();

            if (missingProducts.Any())
            {
                throw new EntityNotFoundException(typeof(Product), missingProducts.First());
            }

            foreach (var item in input.Items)
            {
                var product = productDict[item.ProductId];
                _productManager.UpdateStock(product, item.StockQuantity);
                await _productRepository.UpdateAsync(product);
            }

           
            await CurrentUnitOfWork.SaveChangesAsync();
        }

      
        [Authorize(OnlineStorePermissions.Products.Default)]
        public async Task<StockCheckResultDto> CheckStockAsync(CheckStockInput input)
        {
            if (input == null)
            {
                throw new UserFriendlyException(L["Product:InvalidInput"]);
            }

            if (input.Items == null || input.Items.Count == 0)
            {
                throw new ArgumentException("Items list cannot be null or empty", nameof(input));
            }

           
            var productIds = input.Items.Select(i => i.ProductId).Distinct().ToList();
            var products = await _productRepository.GetByIdsAsync(productIds);
            var productDict = products.ToDictionary(p => p.Id);

            var results = new List<StockCheckItemResultDto>();

            foreach (var item in input.Items)
            {
                if (!productDict.TryGetValue(item.ProductId, out var product))
                {
                    results.Add(new StockCheckItemResultDto
                    {
                        ProductId = item.ProductId,
                        ProductName = "Unknown",
                        SKU = "N/A",
                        RequestedQuantity = item.Quantity,
                        AvailableQuantity = 0,
                        IsAvailable = false,
                        Message = "Product not found"
                    });
                    continue;
                }

                var isAvailable = _productManager.HasSufficientStock(product, item.Quantity);

                results.Add(new StockCheckItemResultDto
                {
                    ProductId = product.Id,
                    ProductName = product.NameEn,
                    SKU = product.SKU,
                    RequestedQuantity = item.Quantity,
                    AvailableQuantity = product.StockQuantity,
                    IsAvailable = isAvailable,
                    Message = isAvailable ? "Stock available" : $"Insufficient stock. Available: {product.StockQuantity}"
                });
            }

            return new StockCheckResultDto
            {
                AllAvailable = results.All(r => r.IsAvailable),
                Items = results
            };
        }

        [Authorize(OnlineStorePermissions.Products.Default)]
        public async Task<List<ProductDto>> GetLowStockAsync(int? threshold = null)
        {
            var stockThreshold = threshold ?? OnlineStore.OnlineStoreConsts.DefaultLowStockThreshold;
            var products = await _productRepository.GetLowStockAsync(stockThreshold);
            return ObjectMapper.Map<List<Product>, List<ProductDto>>(products);
        }

        [Authorize(OnlineStorePermissions.Products.Default)]
        public async Task<List<ProductDto>> GetOutOfStockAsync()
        {
            var products = await _productRepository.GetOutOfStockAsync();
            return ObjectMapper.Map<List<Product>, List<ProductDto>>(products);
        }
    }
}