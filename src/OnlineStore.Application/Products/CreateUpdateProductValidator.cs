using FluentValidation;
using Microsoft.Extensions.Localization;
using OnlineStore.Categories;
using OnlineStore.Localization;
using OnlineStore.Products;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace OnlineStore.Validators
{
    
    public class CreateUpdateProductValidator : AbstractValidator<CreateUpdateProductDto>
    {
        private readonly IProductRepository _productRepository;
        private readonly IRepository<Category, int> _categoryRepository;
        private readonly IStringLocalizer<OnlineStoreResource> _localizer;

        public CreateUpdateProductValidator(
            IProductRepository productRepository,
            IRepository<Category, int> categoryRepository,
            IStringLocalizer<OnlineStoreResource> localizer)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _localizer = localizer;


            RuleFor(x => x.NameAr)
                .NotEmpty()
                .WithMessage(_localizer["Validation:Product:NameArRequired"])
                .MaximumLength(500)
                .WithMessage(_localizer["Validation:Product:NameArMaxLength"]);


            RuleFor(x => x.NameEn)
                .NotEmpty()
                .WithMessage(_localizer["Validation:Product:NameEnRequired"])
                .MaximumLength(500)
                .WithMessage(_localizer["Validation:Product:NameEnMaxLength"]);

            RuleFor(x => x.DescriptionAr)
                .NotEmpty()
                .WithMessage(_localizer["Validation:Product:DescriptionArRequired"])
                .MaximumLength(2000)
                .WithMessage(_localizer["Validation:Product:DescriptionArMaxLength"]);

        
            RuleFor(x => x.DescriptionEn)
                .NotEmpty()
                .WithMessage(_localizer["Validation:Product:DescriptionEnRequired"])
                .MaximumLength(2000)
                .WithMessage(_localizer["Validation:Product:DescriptionEnMaxLength"]);


            RuleFor(x => x.SKU)
                .NotEmpty()
                .WithMessage(_localizer["Validation:Product:SKURequired"])
                .MaximumLength(50)
                .WithMessage(_localizer["Validation:Product:SKUMaxLength"])
                .Must(BeValidSKUFormat)
                .WithMessage(_localizer["Validation:Product:InvalidSKUFormat"])
                .CustomAsync(async (sku, context, cancellation) =>
                {
                   
                    if (string.IsNullOrWhiteSpace(sku))
                    {
                        return;
                    }

                    var dto = (CreateUpdateProductDto)context.InstanceToValidate;
                    var isUnique = await BeUniqueSKUAsync(dto, sku, cancellation);
                    if (!isUnique)
                    { context.AddFailure(_localizer["Validation:Product:SKUAlreadyExists"]);
                    }
                });

        

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0)
                .WithMessage(_localizer["Validation:Product:PriceMustBePositive"])
                .LessThanOrEqualTo(OnlineStore.OnlineStoreConsts.MaxPrice)
                .WithMessage(_localizer["Validation:Product:PriceTooHigh"]);

          
            RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0)
                .WithMessage(_localizer["Validation:Product:StockMustBePositive"])
                .LessThanOrEqualTo(OnlineStore.OnlineStoreConsts.MaxStockQuantity)
                .WithMessage(_localizer["Validation:Product:StockTooHigh"]);


            RuleFor(x => x.CategoryId)
                .GreaterThan(0)
                .WithMessage(_localizer["Validation:Product:CategoryRequired"])
                .MustAsync(CategoryExistsAsync)
                .WithMessage(_localizer["Validation:Product:CategoryNotFound"]);


            RuleFor(x => x)
                .Must(dto => !dto.IsPublished || dto.IsActive)
                .WithMessage(_localizer["Validation:Product:CannotPublishInactive"])
                .WithName("IsPublished");

          
            When(x => x.IsPublished && x.StockQuantity == 0, () =>
            {
                RuleFor(x => x.StockQuantity)
                    .NotEqual(0)
                    .WithMessage(_localizer["Validation:Product:PublishingWithZeroStock"])
                    .WithSeverity(Severity.Warning);
            });
        }

      
        private bool BeValidSKUFormat(string sku)
        {
            if (string.IsNullOrWhiteSpace(sku))
                return false;

            // Allow only uppercase letters, numbers, and hyphens
            var regex = new Regex(@"^[A-Z0-9-]+$");
            return regex.IsMatch(sku);
        }

        
        private async Task<bool> BeUniqueSKUAsync(
            CreateUpdateProductDto dto,
            string sku,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(sku))
            {
                return true; 
            }

            try
            {
                // For new products (Id == 0), pass null to excludeId
                // For existing products, pass the actual Id
                int? excludeId = dto.Id > 0 ? dto.Id : null;
                var isUnique = await _productRepository.IsSKUUniqueAsync(sku, excludeId);
                return isUnique;
            }
            catch (Exception ex)
            {
                
                return false;
            }
        }

    
        private async Task<bool> CategoryExistsAsync(
            int categoryId,
            CancellationToken cancellationToken)
        {
             if (categoryId <= 0)
            {
                return true; 
            }

            try
            {
                return await _categoryRepository.AnyAsync(c => c.Id == categoryId, cancellationToken);
            }
            catch
            {
               return false;
            }
        }
    }
}