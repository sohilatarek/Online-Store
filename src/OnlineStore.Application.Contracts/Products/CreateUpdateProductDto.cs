using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace OnlineStore.Products
{
   
    public class CreateUpdateProductDto : EntityDto<int>
    {
        
        [Required]
        [StringLength(500)]
        public string NameAr { get; set; }

      
        [Required]
        [StringLength(500)]
        public string NameEn { get; set; }

      
        [Required]
        [StringLength(2000)]
        public string DescriptionAr { get; set; }

        [Required]
        [StringLength(2000)]
        public string DescriptionEn { get; set; }

        
        [Required]
        public int CategoryId { get; set; }

        /// <summary>
        /// Stock Keeping Unit - unique product identifier
        /// Format: CATEGORY-TYPE-NUMBER (e.g., FOOD-OIL-001)
        /// </summary>
        [Required]
        [StringLength(50)]
        [RegularExpression(@"^[A-Z0-9-]+$", ErrorMessage = "SKU must contain only uppercase letters, numbers, and hyphens")]
        public string SKU { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than or equal to zero")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be greater than or equal to zero")]
        public int StockQuantity { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsPublished { get; set; } = false;
    }
}