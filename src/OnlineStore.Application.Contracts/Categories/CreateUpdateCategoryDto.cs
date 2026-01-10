using Volo.Abp.Application.Dtos;

namespace OnlineStore.Categories
{
    public class CreateUpdateCategoryDto : EntityDto<int>
    {
        public string NameAr { get; set; }
        public string NameEn { get; set; }

        public string DescriptionAr { get; set; }

        public string DescriptionEn { get; set; }

        public bool IsActive { get; set; } = true;
        public int DisplayOrder { get; set; } = 0;
    }
}