using Volo.Abp.Application.Dtos;

namespace OnlineStore.Categories
{
    /// <summary>
    /// Input DTO for filtering and paginating categories
    /// </summary>
    public class GetCategoriesInput : PagedAndSortedResultRequestDto
    {
        public bool? IsActive { get; set; }

        public string SearchTerm { get; set; }

        public GetCategoriesInput()
        {
            // Default pagination
            MaxResultCount = 10;

            // Default sorting by DisplayOrder ascending
            Sorting = "DisplayOrder asc";
        }
    }
}