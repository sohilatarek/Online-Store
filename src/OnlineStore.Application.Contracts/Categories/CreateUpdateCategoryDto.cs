using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace OnlineStore.Categories
{
    public class CreateUpdateCategoryDto : EntityDto<int>
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

        public bool IsActive { get; set; } = true;

        [Range(0, int.MaxValue)]
        public int DisplayOrder { get; set; } = 0;
    }
}