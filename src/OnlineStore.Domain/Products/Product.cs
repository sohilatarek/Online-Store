using Volo.Abp.Domain.Entities.Auditing;


public class Product : FullAuditedEntity<int>
{
    public string NameAr { get; set; }

    public string NameEn { get; set; }

    public string DescriptionAr { get; set; }

    public string DescriptionEn { get; set; }

    public int CategoryId { get; set; }

    public virtual Category Category { get; set; }

}