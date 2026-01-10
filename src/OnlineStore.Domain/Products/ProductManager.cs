using Microsoft.Extensions.Logging;
using OnlineStore;
using OnlineStore.Categories;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace OnlineStore.Products
{
    public class ProductManager : DomainService
    {
        private readonly IProductRepository _productRepository;
        private readonly IRepository<Category, int> _categoryRepository;

        public ProductManager(
            IProductRepository productRepository,
            IRepository<Category, int> categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        /// <summary>
        /// Creates a new product with business rule validation
        /// </summary>
        public async Task<Product> CreateAsync(
            string nameAr,
            string nameEn,
            string descriptionAr,
            string descriptionEn,
            int categoryId,
            string sku,
            decimal price,
            int stockQuantity = 0,
            bool isActive = true,
            bool isPublished = false)
        {
            // Business Rule: SKU must be unique per tenant
            await CheckSKUUniquenessAsync(sku, null);

            // Business Rule: Category must exist
            await ValidateCategoryExistsAsync(categoryId);

            // Business Rule: Price must be valid
            ValidatePrice(price);

            // Business Rule: Stock must be valid
            ValidateStock(stockQuantity);

            // Create the product
            var product = new Product(
                nameAr,
                nameEn,
                descriptionAr,
                descriptionEn,
                categoryId,
                sku,
                price,
                stockQuantity,
                isActive,
                isPublished
            );

            return product;
        }

        /// <summary>
        /// Updates an existing product with validation
        /// </summary>
        public async Task UpdateAsync(
            Product product,
            string nameAr,
            string nameEn,
            string descriptionAr,
            string descriptionEn,
            int categoryId,
            decimal price,
            int stockQuantity)
        {
            // Business Rule: Category must exist if changed
            if (product.CategoryId != categoryId)
            {
                await ValidateCategoryExistsAsync(categoryId);
            }

            // Business Rule: Price must be valid
            ValidatePrice(price);

            // Business Rule: Stock must be valid
            ValidateStock(stockQuantity);

            // Update the product
            product.Update(nameAr, nameEn, descriptionAr, descriptionEn, categoryId, price, stockQuantity);
        }

        /// <summary>
        /// Updates product SKU with uniqueness check
        /// </summary>
        public async Task ChangeSKUAsync(Product product, string newSku)
        {
            if (product.SKU != newSku)
            {
                await CheckSKUUniquenessAsync(newSku, product.Id);
                product.SetSKU(newSku);
            }
        }

        /// <summary>
        /// Publishes a product (makes it visible to customers)
        /// </summary>
        public async Task PublishAsync(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            // Business Rule: Product must be active to be published
            if (!product.IsActive)
            {
                throw new BusinessException(OnlineStoreErrorCodes.CannotPublishInactiveProduct)
                    .WithData("ProductId", product.Id)
                    .WithData("ProductName", product.NameEn)
                    .WithData("Message", "Cannot publish an inactive product. Please activate it first.");
            }

            // Warning: Publishing product with zero stock
            if (product.StockQuantity == 0)
            {
                Logger.LogWarning($"Publishing product '{product.NameEn}' (ID: {product.Id}) with zero stock.");
            }

            product.Publish();
        }

        /// <summary>
        /// Unpublishes a product (hides from customers)
        /// </summary>
        public void Unpublish(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            product.Unpublish();
        }

        /// <summary>
        /// Activates a product
        /// </summary>
        public void Activate(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            product.Activate();
        }

        /// <summary>
        /// Deactivates a product (also unpublishes it)
        /// </summary>
        public void Deactivate(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            product.Deactivate();
        }

        /// <summary>
        /// Updates product stock quantity
        /// </summary>
        public void UpdateStock(Product product, int quantity)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            ValidateStock(quantity);
            product.SetStock(quantity);
        }

        /// <summary>
        /// Adjusts stock by adding or removing quantity
        /// </summary>
        public void AdjustStock(Product product, int quantityChange)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            product.UpdateStock(quantityChange);
        }

        /// <summary>
        /// Updates product price
        /// </summary>
        public void UpdatePrice(Product product, decimal newPrice)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            ValidatePrice(newPrice);
            product.SetPrice(newPrice);
        }

        /// <summary>
        /// Checks if product has sufficient stock for an order
        /// </summary>
        public bool HasSufficientStock(Product product, int requestedQuantity)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            return product.HasSufficientStock(requestedQuantity);
        }

        /// <summary>
        /// Reserves stock for an order (reduces stock quantity)
        /// </summary>
        public void ReserveStock(Product product, int quantity)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            if (!product.HasSufficientStock(quantity))
            {
                throw new BusinessException(OnlineStoreErrorCodes.InsufficientStock)
                    .WithData("ProductId", product.Id)
                    .WithData("ProductName", product.NameEn)
                    .WithData("AvailableStock", product.StockQuantity)
                    .WithData("RequestedQuantity", quantity)
                    .WithData("Message", $"Insufficient stock. Available: {product.StockQuantity}, Requested: {quantity}");
            }

            product.UpdateStock(-quantity);
        }

        /// <summary>
        /// Releases reserved stock (adds back to stock)
        /// </summary>
        public void ReleaseStock(Product product, int quantity)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            ValidateStock(quantity);
            product.UpdateStock(quantity);
        }

        // ==========================================
        // PRIVATE HELPER METHODS
        // ==========================================

        /// <summary>
        /// Ensures SKU is unique within tenant
        /// </summary>
        private async Task CheckSKUUniquenessAsync(string sku, int? excludeId = null)
        {
            var isUnique = await _productRepository.IsSKUUniqueAsync(sku, excludeId);

            if (!isUnique)
            {
                throw new BusinessException(OnlineStoreErrorCodes.SKUAlreadyExists)
                    .WithData("SKU", sku)
                    .WithData("Message", $"Product with SKU '{sku}' already exists.");
            }
        }

        /// <summary>
        /// Validates that category exists and is active
        /// </summary>
        private async Task ValidateCategoryExistsAsync(int categoryId)
        {
            var category = await _categoryRepository.FirstOrDefaultAsync(c => c.Id == categoryId);

            if (category == null)
            {
                throw new BusinessException(OnlineStoreErrorCodes.CategoryNotFound)
                    .WithData("CategoryId", categoryId)
                    .WithData("Message", $"Category with ID {categoryId} does not exist.");
            }

            // Business Rule: Category must be active to be used
            if (!category.IsActive)
            {
                throw new BusinessException(OnlineStoreErrorCodes.CategoryNotActive)
                    .WithData("CategoryId", categoryId)
                    .WithData("CategoryName", category.NameEn)
                    .WithData("Message", $"Category with ID {categoryId} is not active and cannot be used.");
            }
        }

        /// <summary>
        /// Validates price is non-negative
        /// </summary>
        private void ValidatePrice(decimal price)
        {
            if (price < 0)
            {
                throw new BusinessException(OnlineStoreErrorCodes.PriceCannotBeNegative)
                    .WithData("Price", price)
                    .WithData("Message", "Price cannot be negative.");
            }
        }

        /// <summary>
        /// Validates stock quantity is non-negative
        /// </summary>
        private void ValidateStock(int quantity)
        {
            if (quantity < 0)
            {
                throw new BusinessException(OnlineStoreErrorCodes.StockQuantityCannotBeNegative)
                    .WithData("Quantity", quantity)
                    .WithData("Message", "Stock quantity cannot be negative.");
            }
        }
    }
}