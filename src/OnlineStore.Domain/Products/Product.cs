using System;
using OnlineStore;
using OnlineStore.Categories;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace OnlineStore.Products
{
    /// <summary>
    /// Product entity - represents products available in the online store
    /// Inherits full audit properties + multi-tenancy support
    /// </summary>
    public class Product : FullAuditedAggregateRoot<int>, IMultiTenant
    {
       
        public Guid? TenantId { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }

        public string DescriptionAr { get; set; }

        public string DescriptionEn { get; set; }

        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }

      
        /// <summary>
        /// Stock Keeping Unit - unique product identifier for inventory
        /// </summary>
        public string SKU { get; set; }

        /// <summary>
        /// Product price in the system currency
        /// Must be >= 0
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Available stock quantity
        /// Must be >= 0
        /// </summary>
        public int StockQuantity { get; set; }

        /// <summary>
        /// Indicates if product is active (can be managed/sold)
        /// Inactive products are hidden from operations
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Indicates if product is published (visible to customers)
        /// A product can be active but not published (internal use)
        /// </summary>
        public bool IsPublished { get; set; }

        protected Product()
        {
           
        }

      
        public Product(
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
            SetName(nameAr, nameEn);
            SetDescription(descriptionAr, descriptionEn);
            CategoryId = categoryId;
            SetSKU(sku);
            SetPrice(price);
            SetStock(stockQuantity);
            IsActive = isActive;
            IsPublished = isPublished;
        }

        public void SetName(string nameAr, string nameEn)
        {
            NameAr = Check.NotNullOrWhiteSpace(nameAr, nameof(nameAr));
            NameEn = Check.NotNullOrWhiteSpace(nameEn, nameof(nameEn));
        }

        public void SetDescription(string descriptionAr, string descriptionEn)
        {
            DescriptionAr = Check.NotNullOrWhiteSpace(descriptionAr, nameof(descriptionAr));
            DescriptionEn = Check.NotNullOrWhiteSpace(descriptionEn, nameof(descriptionEn));
        }

        public void SetSKU(string sku)
        {
            SKU = Check.NotNullOrWhiteSpace(sku, nameof(sku));
        }

        public void SetPrice(decimal price)
        {
            if (price < 0)
            {
                throw new BusinessException(OnlineStoreErrorCodes.PriceCannotBeNegative)
                    .WithData("Price", price)
                    .WithData("Message", "Price cannot be negative");
            }

            Price = price;
        }

        public void SetStock(int quantity)
        {
            if (quantity < 0)
            {
                throw new BusinessException(OnlineStoreErrorCodes.StockQuantityCannotBeNegative)
                    .WithData("Quantity", quantity)
                    .WithData("Message", "Stock quantity cannot be negative");
            }

            StockQuantity = quantity;
        }

        /// <summary>
        /// Updates stock by adding or removing quantity
        /// </summary>
        public void UpdateStock(int quantity)
        {
            var newStock = StockQuantity + quantity;

            if (newStock < 0)
            {
                throw new BusinessException(OnlineStoreErrorCodes.InsufficientStock)
                    .WithData("CurrentStock", StockQuantity)
                    .WithData("RequestedChange", quantity)
                    .WithData("Message", "Insufficient stock quantity");
            }

            StockQuantity = newStock;
        }

        /// <summary>
        /// Checks if sufficient stock is available
        /// </summary>
        public bool HasSufficientStock(int requestedQuantity)
        {
            return StockQuantity >= requestedQuantity;
        }

        /// <summary>
        /// Publishes the product (makes it visible to customers)
        /// Business rule: Product must be active to be published
        /// </summary>
        public void Publish()
        {
            if (!IsActive)
            {
                throw new BusinessException(OnlineStoreErrorCodes.CannotPublishInactiveProduct)
                    .WithData("ProductId", Id)
                    .WithData("ProductName", NameEn)
                    .WithData("Message", "Cannot publish an inactive product");
            }

            IsPublished = true;
        }

        /// <summary>
        /// Unpublishes the product (hides from customers)
        /// </summary>
        public void Unpublish()
        {
            IsPublished = false;
        }

        /// <summary>
        /// Activates the product
        /// </summary>
        public void Activate()
        {
            IsActive = true;
        }

        /// <summary>
        /// Deactivates the product (also unpublishes it)
        /// </summary>
        public void Deactivate()
        {
            IsActive = false;
            IsPublished = false; // Deactivating also unpublishes
        }

        public bool IsOutOfStock()
        {
            return StockQuantity == 0;
        }

        /// <summary>
        /// Checks if product is low on stock (threshold: 10 units)
        /// </summary>
        public bool IsLowStock(int threshold = 10)
        {
            return StockQuantity > 0 && StockQuantity <= threshold;
        }

        /// <summary>
        /// Calculates total inventory value for this product
        /// </summary>
        public decimal GetInventoryValue()
        {
            return Price * StockQuantity;
        }

        /// <summary>
        /// Updates complete product information
        /// </summary>
        public void Update(
            string nameAr,
            string nameEn,
            string descriptionAr,
            string descriptionEn,
            int categoryId,
            decimal price,
            int stockQuantity)
        {
            SetName(nameAr, nameEn);
            SetDescription(descriptionAr, descriptionEn);
            CategoryId = categoryId;
            SetPrice(price);
            SetStock(stockQuantity);
        }
    }
}