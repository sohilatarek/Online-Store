using System;
using Volo.Abp.Application.Dtos;

namespace OnlineStore.Products
{
    
    public class ProductDto : FullAuditedEntityDto<int>
    {
     
        public string NameAr { get; set; }

        public string NameEn { get; set; }

        public string DescriptionAr { get; set; }

        public string DescriptionEn { get; set; }

        public int CategoryId { get; set; }

        public string CategoryNameAr { get; set; }

        public string CategoryNameEn { get; set; }

        public string CategoryDescriptionAr { get; set; }

        public string CategoryDescriptionEn { get; set; }

        public string SKU { get; set; }

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public bool IsActive { get; set; }

        public bool IsPublished { get; set; }

        /// <summary>
        /// Calculated: Total inventory value (Price * StockQuantity)
        /// </summary>
        public decimal InventoryValue => Price * StockQuantity;

        /// <summary>
        /// Calculated: Is the product out of stock
        /// </summary>
        public bool IsOutOfStock => StockQuantity == 0;

        /// <summary>
        /// Calculated: Is the product low on stock (threshold: 10)
        /// </summary>
        public bool IsLowStock => StockQuantity > 0 && StockQuantity <= 10;
    }
}