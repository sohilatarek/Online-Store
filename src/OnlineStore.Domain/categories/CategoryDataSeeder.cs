using OnlineStore.Categories;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace OnlineStore.categories
{
    public class CategoryDataSeeder : IDataSeedContributor, ITransientDependency
    {
        private readonly IRepository<Category, int> _categoriesRepository;

        public CategoryDataSeeder(IRepository<Category, int> categoriesRepository)
        {
            _categoriesRepository = categoriesRepository;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (!await _categoriesRepository.AnyAsync())
            {
                var categories = new List<Category>
                {
                    new Category(
                        id: 1,
                        nameAr: "المنتجات الغذائية الاستهلاكية",
                        nameEn: "Consumer Food Products",
                        descriptionAr: "المنتجات المخصصة للاستهلاك البشري، وتشمل الأغذية المعبأة والمصنعة.",
                        descriptionEn: "Products intended for human consumption, including packaged and processed foods.",
                        isActive: true,
                        displayOrder: 1
                    ),

                    new Category(
                        id: 2,
                        nameAr: "مستلزمات التنظيف المنزلية",
                        nameEn: "Household Cleaning Supplies",
                        descriptionAr: "المنتجات الكيميائية وغير الكيميائية المستخدمة لأغراض التنظيف المنزلي والتجاري.",
                        descriptionEn: "Chemical and non-chemical products used for domestic and commercial cleaning purposes.",
                        isActive: true,
                        displayOrder: 2
                    ),

                    new Category(
                        id: 3,
                        nameAr: "العناية الشخصية ومستحضرات التجميل",
                        nameEn: "Personal Care & Cosmetics",
                        descriptionAr: "المنتجات المتعلقة بالنظافة الشخصية والعناية والمظهر العام.",
                        descriptionEn: "Products related to personal hygiene, grooming, and cosmetic enhancement.",
                        isActive: true,
                        displayOrder: 3
                    ),

                    new Category(
                        id: 4,
                        nameAr: "مواد التعبئة والتغليف الصناعية",
                        nameEn: "Packaging & Industrial Materials",
                        descriptionAr: "المواد المستخدمة في التعبئة والتخزين والاستخدامات الصناعية المختلفة.",
                        descriptionEn: "Materials used for packaging, storage, and industrial manufacturing applications.",
                        isActive: true,
                        displayOrder: 4
                    ),

               
                    new Category(
                        id: 5,
                        nameAr: "فئة غير نشطة",
                        nameEn: "Inactive Category",
                        descriptionAr: "فئة معطلة مؤقتاً للاختبار",
                        descriptionEn: "Temporarily disabled category for testing purposes",
                        isActive: false, 
                        displayOrder: 99
                    )
                };

                await _categoriesRepository.InsertManyAsync(categories);
            }
        }
    }
}