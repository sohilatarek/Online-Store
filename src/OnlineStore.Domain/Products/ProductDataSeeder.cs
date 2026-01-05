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
                   
                    new Product(
                        nameAr: "زيت زيتون بكر ممتاز",
                        nameEn: "Extra Virgin Olive Oil",
                        descriptionAr: "زيت زيتون بكر ممتاز معصور على البارد من أجود أنواع الزيتون",
                        descriptionEn: "Cold-pressed extra virgin olive oil from premium quality olives",
                        categoryId: 1,
                        sku: "FOOD-OIL-001",
                        price: 25.99m,
                        stockQuantity: 150,
                        isActive: true,
                        isPublished: true
                    ),
                    new Product(
                        nameAr: "معكرونة إيطالية",
                        nameEn: "Italian Pasta",
                        descriptionAr: "معكرونة إيطالية أصلية مصنوعة من القمح الصلب",
                        descriptionEn: "Authentic Italian pasta made from durum wheat",
                        categoryId: 1,
                        sku: "FOOD-PASTA-001",
                        price: 12.50m,
                        stockQuantity: 200,
                        isActive: true,
                        isPublished: true
                    ),
                    new Product(
                        nameAr: "عسل طبيعي",
                        nameEn: "Natural Honey",
                        descriptionAr: "عسل طبيعي نقي 100% من مناحل محلية",
                        descriptionEn: "100% pure natural honey from local apiaries",
                        categoryId: 1,
                        sku: "FOOD-HONEY-001",
                        price: 35.00m,
                        stockQuantity: 75,
                        isActive: true,
                        isPublished: true
                    ),

                  
                    new Product(
                        nameAr: "منظف متعدد الأغراض",
                        nameEn: "Multi-Purpose Cleaner",
                        descriptionAr: "منظف قوي للأسطح المتعددة، آمن وفعال",
                        descriptionEn: "Powerful multi-surface cleaner, safe and effective",
                        categoryId: 2,
                        sku: "CLEAN-MPC-001",
                        price: 8.99m,
                        stockQuantity: 300,
                        isActive: true,
                        isPublished: true
                    ),
                    new Product(
                        nameAr: "سائل غسيل الأطباق",
                        nameEn: "Dish Washing Liquid",
                        descriptionAr: "سائل غسيل أطباق بتركيبة قوية لإزالة الدهون",
                        descriptionEn: "Powerful dish washing liquid with grease-cutting formula",
                        categoryId: 2,
                        sku: "CLEAN-DISH-001",
                        price: 6.50m,
                        stockQuantity: 250,
                        isActive: true,
                        isPublished: true
                    ),
                    new Product(
                        nameAr: "مسحوق غسيل الملابس",
                        nameEn: "Laundry Detergent Powder",
                        descriptionAr: "مسحوق غسيل فعال لجميع أنواع الأقمشة",
                        descriptionEn: "Effective laundry detergent for all fabric types",
                        categoryId: 2,
                        sku: "CLEAN-LAUNDRY-001",
                        price: 15.99m,
                        stockQuantity: 180,
                        isActive: true,
                        isPublished: true
                    ),

                    new Product(
                        nameAr: "شامبو للعناية بالشعر",
                        nameEn: "Hair Care Shampoo",
                        descriptionAr: "شامبو غني بالفيتامينات لشعر صحي ولامع",
                        descriptionEn: "Vitamin-enriched shampoo for healthy, shiny hair",
                        categoryId: 3,
                        sku: "CARE-SHAMPOO-001",
                        price: 18.50m,
                        stockQuantity: 120,
                        isActive: true,
                        isPublished: true
                    ),
                    new Product(
                        nameAr: "كريم ترطيب البشرة",
                        nameEn: "Skin Moisturizing Cream",
                        descriptionAr: "كريم مرطب عميق لجميع أنواع البشرة",
                        descriptionEn: "Deep moisturizing cream for all skin types",
                        categoryId: 3,
                        sku: "CARE-CREAM-001",
                        price: 24.99m,
                        stockQuantity: 90,
                        isActive: true,
                        isPublished: true
                    ),
                    new Product(
                        nameAr: "معجون أسنان",
                        nameEn: "Toothpaste",
                        descriptionAr: "معجون أسنان بالفلورايد لحماية فائقة",
                        descriptionEn: "Fluoride toothpaste for superior protection",
                        categoryId: 3,
                        sku: "CARE-TOOTH-001",
                        price: 5.99m,
                        stockQuantity: 400,
                        isActive: true,
                        isPublished: true
                    ),
                    new Product(
                        nameAr: "صابون سائل لليدين",
                        nameEn: "Liquid Hand Soap",
                        descriptionAr: "صابون سائل مضاد للبكتيريا بعطر منعش",
                        descriptionEn: "Antibacterial liquid hand soap with refreshing scent",
                        categoryId: 3,
                        sku: "CARE-SOAP-001",
                        price: 7.50m,
                        stockQuantity: 220,
                        isActive: true,
                        isPublished: true
                    ),

                 
                    new Product(
                        nameAr: "أكياس بلاستيكية صناعية",
                        nameEn: "Industrial Plastic Bags",
                        descriptionAr: "أكياس بلاستيكية عالية الجودة للاستخدامات الصناعية",
                        descriptionEn: "High-quality plastic bags for industrial use",
                        categoryId: 4,
                        sku: "PKG-BAGS-001",
                        price: 45.00m,
                        stockQuantity: 500,
                        isActive: true,
                        isPublished: true
                    ),
                    new Product(
                        nameAr: "ورق تغليف كرافت",
                        nameEn: "Kraft Wrapping Paper",
                        descriptionAr: "ورق كرافت قوي للتغليف والحماية",
                        descriptionEn: "Strong kraft paper for wrapping and protection",
                        categoryId: 4,
                        sku: "PKG-PAPER-001",
                        price: 22.50m,
                        stockQuantity: 350,
                        isActive: true,
                        isPublished: true
                    ),
                    new Product(
                        nameAr: "صناديق كرتون مموجة",
                        nameEn: "Corrugated Cardboard Boxes",
                        descriptionAr: "صناديق كرتون مموجة للشحن والتخزين",
                        descriptionEn: "Corrugated cardboard boxes for shipping and storage",
                        categoryId: 4,
                        sku: "PKG-BOXES-001",
                        price: 65.00m,
                        stockQuantity: 200,
                        isActive: true,
                        isPublished: true
                    ),
                    new Product(
                        nameAr: "شريط تغليف شفاف",
                        nameEn: "Clear Packaging Tape",
                        descriptionAr: "شريط لاصق قوي للتغليف والتعبئة",
                        descriptionEn: "Strong adhesive tape for packaging and sealing",
                        categoryId: 4,
                        sku: "PKG-TAPE-001",
                        price: 12.00m,
                        stockQuantity: 600,
                        isActive: true,
                        isPublished: true
                    ),

                 
                    // Active but not published (internal use only)
                    new Product(
                        nameAr: "منتج قيد الاختبار",
                        nameEn: "Product Under Testing",
                        descriptionAr: "منتج جديد قيد الاختبار قبل الإطلاق",
                        descriptionEn: "New product being tested before launch",
                        categoryId: 1,
                        sku: "TEST-SAMPLE-001",
                        price: 99.99m,
                        stockQuantity: 10,
                        isActive: true,
                        isPublished: false  // Not visible to customers yet
                    ),

                    // Low stock product
                    new Product(
                        nameAr: "منتج مخزون منخفض",
                        nameEn: "Low Stock Item",
                        descriptionAr: "منتج يحتاج إعادة تخزين",
                        descriptionEn: "Item that needs restocking",
                        categoryId: 2,
                        sku: "LOW-STOCK-001",
                        price: 15.00m,
                        stockQuantity: 5,  // Low stock
                        isActive: true,
                        isPublished: true
                    ),

                    // Out of stock product
                    new Product(
                        nameAr: "منتج نفد المخزون",
                        nameEn: "Out of Stock Product",
                        descriptionAr: "منتج غير متوفر حالياً",
                        descriptionEn: "Currently unavailable product",
                        categoryId: 3,
                        sku: "OUT-STOCK-001",
                        price: 30.00m,
                        stockQuantity: 0,  // Out of stock
                        isActive: true,
                        isPublished: true
                    )
                };

                await _productsRepository.InsertManyAsync(products);
            }
        }
    }
}