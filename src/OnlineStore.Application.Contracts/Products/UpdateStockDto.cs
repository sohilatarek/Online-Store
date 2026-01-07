using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Products
{
    /// <summary>
    /// DTO for updating product stock quantity
    /// </summary>
    public class UpdateStockDto
    {
     
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be greater than or equal to zero")]
        public int StockQuantity { get; set; }
    }

    /// <summary>
    /// DTO for adjusting product stock (add or subtract)
    /// </summary>
    public class AdjustStockDto
    {
       
        [Required]
        public int QuantityChange { get; set; }

        [StringLength(500)]
        public string Reason { get; set; }
    }

    /// <summary>
    /// DTO for bulk stock updates
    /// </summary>
    public class BulkUpdateStockDto
    {
        
        [Required]
        public List<BulkStockItem> Items { get; set; }
    }

    /// <summary>
    /// Individual item in bulk stock update
    /// </summary>
    public class BulkStockItem
    {
       
        [Required]
        public int ProductId { get; set; }

      
        [Required]
        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }
    }

    /// <summary>
    /// DTO for checking stock availability
    /// </summary>
    public class CheckStockInput
    {
        
        [Required]
        public List<StockCheckItem> Items { get; set; }
    }

    /// <summary>
    /// Individual item to check stock for
    /// </summary>
    public class StockCheckItem
    {
      
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }

    /// <summary>
    /// Result of stock availability check
    /// </summary>
    public class StockCheckResultDto
    {
       
        public bool AllAvailable { get; set; }

        public List<StockCheckItemResultDto> Items { get; set; }
    }

    /// <summary>
    /// Result for individual item stock check
    /// </summary>
    public class StockCheckItemResultDto
    {
        
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string SKU { get; set; }

        public int RequestedQuantity { get; set; }

        public int AvailableQuantity { get; set; }

        public bool IsAvailable { get; set; }

        public string Message { get; set; }
    }
}