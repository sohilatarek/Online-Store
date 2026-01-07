using Volo.Abp.Application.Dtos;

namespace OnlineStore.Products
{
   
    public class GetProductsInput : PagedAndSortedResultRequestDto
    {
        
        public int? CategoryId { get; set; }

        public bool? IsActive { get; set; }

        public bool? IsPublished { get; set; }

     
        public string SearchTerm { get; set; }

       
        public decimal? MinPrice { get; set; }

        public decimal? MaxPrice { get; set; }

        public bool? IsLowStock { get; set; }

        public int LowStockThreshold { get; set; } = 10;

     
        public bool? IsOutOfStock { get; set; }


        public GetProductsInput()
        {
            // Default pagination
            MaxResultCount = 20;

            // Default sorting by creation time descending (newest first)
            Sorting = "CreationTime desc";
        }
    }
}