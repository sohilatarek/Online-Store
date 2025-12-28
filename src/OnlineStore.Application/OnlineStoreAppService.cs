using OnlineStore.Localization;
using Volo.Abp.Application.Services;

namespace OnlineStore;

/* Inherit your application services from this class.
 */
public abstract class OnlineStoreAppService : ApplicationService
{
    protected OnlineStoreAppService()
    {
        LocalizationResource = typeof(OnlineStoreResource);
    }
}
