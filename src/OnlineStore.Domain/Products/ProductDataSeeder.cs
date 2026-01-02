using OnlineStore.Products;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace OnlineStore.Products
{
    public class ProductDataSeeder : IDataSeedContributor, ITransientDependency
    {
        private readonly IRepository<Product, int> _productsRepository;

        public ProductDataSeeder(IRepository<Product, int> productsRepository)
        {
            _productsRepository = productsRepository;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (!await _productsRepository.AnyAsync())
            {
                var products = new List<Product>
                {
                    // Consumer Food Products (CategoryId: 1)
                    new Product
                    {
                        NameAr = "زيت زيتون بكر ممتاز",
                        NameEn = "Extra Virgin Olive Oil",
                        DescriptionAr = "زيت زيتون بكر ممتاز معصور على البارد من أجود أنواع الزيتون",
                        DescriptionEn = "Cold-pressed extra virgin olive oil from premium quality olives",
                        CategoryId = 1
                    },
                    new Product
                    {
                        NameAr = "معكرونة إيطالية",
                        NameEn = "Italian Pasta",
                        DescriptionAr = "معكرونة إيطالية أصلية مصنوعة من القمح الصلب",
                        DescriptionEn = "Authentic Italian pasta made from durum wheat",
                        CategoryId = 1
                    },
                    new Product
                    {
                        NameAr = "عسل طبيعي",
                        NameEn = "Natural Honey",
                        DescriptionAr = "عسل طبيعي نقي 100% من مناحل محلية",
                        DescriptionEn = "100% pure natural honey from local apiaries",
                        CategoryId = 1
                    },

                    // Household Cleaning Supplies (CategoryId: 2)
                    new Product
                    {
                        NameAr = "منظف متعدد الأغراض",
                        NameEn = "Multi-Purpose Cleaner",
                        DescriptionAr = "منظف قوي للأسطح المتعددة، آمن وفعال",
                        DescriptionEn = "Powerful multi-surface cleaner, safe and effective",
                        CategoryId = 2
                    },
                    new Product
                    {
                        NameAr = "سائل غسيل الأطباق",
                        NameEn = "Dish Washing Liquid",
                        DescriptionAr = "سائل غسيل أطباق بتركيبة قوية لإزالة الدهون",
                        DescriptionEn = "Powerful dish washing liquid with grease-cutting formula",
                        CategoryId = 2
                    },
                    new Product
                    {
                        NameAr = "مسحوق غسيل الملابس",
                        NameEn = "Laundry Detergent Powder",
                        DescriptionAr = "مسحوق غسيل فعال لجميع أنواع الأقمشة",
                        DescriptionEn = "Effective laundry detergent for all fabric types",
                        CategoryId = 2
                    },

                    // Personal Care & Cosmetics (CategoryId: 3)
                    new Product
                    {
                        NameAr = "شامبو للعناية بالشعر",
                        NameEn = "Hair Care Shampoo",
                        DescriptionAr = "شامبو غني بالفيتامينات لشعر صحي ولامع",
                        DescriptionEn = "Vitamin-enriched shampoo for healthy, shiny hair",
                        CategoryId = 3
                    },
                    new Product
                    {
                        NameAr = "كريم ترطيب البشرة",
                        NameEn = "Skin Moisturizing Cream",
                        DescriptionAr = "كريم مرطب عميق لجميع أنواع البشرة",
                        DescriptionEn = "Deep moisturizing cream for all skin types",
                        CategoryId = 3
                    },
                    new Product
                    {
                        NameAr = "معجون أسنان",
                        NameEn = "Toothpaste",
                        DescriptionAr = "معجون أسنان بالفلورايد لحماية فائقة",
                        DescriptionEn = "Fluoride toothpaste for superior protection",
                        CategoryId = 3
                    },
                    new Product
                    {
                        NameAr = "صابون سائل لليدين",
                        NameEn = "Liquid Hand Soap",
                        DescriptionAr = "صابون سائل مضاد للبكتيريا بعطر منعش",
                        DescriptionEn = "Antibacterial liquid hand soap with refreshing scent",
                        CategoryId = 3
                    },

                    // Packaging & Industrial Materials (CategoryId: 4)
                    new Product
                    {
                        NameAr = "أكياس بلاستيكية صناعية",
                        NameEn = "Industrial Plastic Bags",
                        DescriptionAr = "أكياس بلاستيكية عالية الجودة للاستخدامات الصناعية",
                        DescriptionEn = "High-quality plastic bags for industrial use",
                        CategoryId = 4
                    },
                    new Product
                    {
                        NameAr = "ورق تغليف كرافت",
                        NameEn = "Kraft Wrapping Paper",
                        DescriptionAr = "ورق كرافت قوي للتغليف والحماية",
                        DescriptionEn = "Strong kraft paper for wrapping and protection",
                        CategoryId = 4
                    },
                    new Product
                    {
                        NameAr = "صناديق كرتون مموجة",
                        NameEn = "Corrugated Cardboard Boxes",
                        DescriptionAr = "صناديق كرتون مموجة للشحن والتخزين",
                        DescriptionEn = "Corrugated cardboard boxes for shipping and storage",
                        CategoryId = 4
                    },
                    new Product
                    {
                        NameAr = "شريط تغليف شفاف",
                        NameEn = "Clear Packaging Tape",
                        DescriptionAr = "شريط لاصق قوي للتغليف والتعبئة",
                        DescriptionEn = "Strong adhesive tape for packaging and sealing",
                        CategoryId = 4
                    }
                };

                await _productsRepository.InsertManyAsync(products);
            }
        }
    }
}