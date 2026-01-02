using Volo.Abp.Domain.Entities.Auditing;

namespace OnlineStore.Categories
{
    public class Category : FullAuditedEntity<int>
    {
        public Category(int id, string nameAr, string nameEn, string descriptionAr, string descriptionEn) : base(id)
        {
            Id = id;
            NameAr = nameAr;
            NameEn = nameEn;
            DescriptionAr = descriptionAr;
            DescriptionEn = descriptionEn;
        }

        public string NameAr { get; set; }

        public string NameEn { get; set; }

        public string DescriptionAr { get; set; }

        public string DescriptionEn { get; set; }
    }
}