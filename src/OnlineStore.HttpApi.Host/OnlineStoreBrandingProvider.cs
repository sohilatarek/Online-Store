using Microsoft.Extensions.Localization;
using OnlineStore.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace OnlineStore;

[Dependency(ReplaceServices = true)]
public class OnlineStoreBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<OnlineStoreResource> _localizer;

    public OnlineStoreBrandingProvider(IStringLocalizer<OnlineStoreResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
