using FluentValidation;
using Microsoft.Extensions.Localization;
using OnlineStore.Categories;
using OnlineStore.Localization;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineStore.Validators
{
    public class CreateUpdateCategoryValidator : AbstractValidator<CreateUpdateCategoryDto>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IStringLocalizer<OnlineStoreResource> _localizer;

        public CreateUpdateCategoryValidator(
            ICategoryRepository categoryRepository,
            IStringLocalizer<OnlineStoreResource> localizer)
        {
            _categoryRepository = categoryRepository;
            _localizer = localizer;

           

            RuleFor(x => x.NameAr)
                .NotEmpty()
                .WithMessage(_localizer["Validation:Category:NameArRequired"])
                .MaximumLength(500)
                .WithMessage(_localizer["Validation:Category:NameArMaxLength"]);


            RuleFor(x => x.NameEn)
                .NotEmpty()
                .WithMessage(_localizer["Validation:Category:NameEnRequired"])
                .MaximumLength(500)
                .WithMessage(_localizer["Validation:Category:NameEnMaxLength"])
             
                .MustAsync(async (dto, nameEn, context, cancellation) =>
                {
                  
                    return await _categoryRepository.IsNameUniqueAsync(nameEn, dto.Id);
                })
                .WithMessage(_localizer["Validation:Category:NameAlreadyExists"]);

            RuleFor(x => x.DescriptionAr)
                .NotEmpty()
                .WithMessage(_localizer["Validation:Category:DescriptionArRequired"])
                .MaximumLength(2000)
                .WithMessage(_localizer["Validation:Category:DescriptionArMaxLength"]);


            RuleFor(x => x.DescriptionEn)
                .NotEmpty()
                .WithMessage(_localizer["Validation:Category:DescriptionEnRequired"])
                .MaximumLength(2000)
                .WithMessage(_localizer["Validation:Category:DescriptionEnMaxLength"]);


            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0)
                .WithMessage(_localizer["Validation:Category:InvalidDisplayOrder"]);
        }
    }
}