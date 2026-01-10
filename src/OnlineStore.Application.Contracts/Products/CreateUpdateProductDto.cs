using Volo.Abp.Application.Dtos;

namespace OnlineStore.Products
{
  
    public class CreateUpdateProductDto : EntityDto<int>
    {
    
        public string NameAr { get; set; }

        public string NameEn { get; set; }

       
        public string DescriptionAr { get; set; }

     
        public string DescriptionEn { get; set; }

       
        public int CategoryId { get; set; }

        /// <summary>
        /// Stock Keeping Unit - unique product identifier
        /// Format: CATEGORY-TYPE-NUMBER (e.g., FOOD-OIL-001)
        /// </summary>
        public string SKU { get; set; }

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsPublished { get; set; } = false;
    }
}