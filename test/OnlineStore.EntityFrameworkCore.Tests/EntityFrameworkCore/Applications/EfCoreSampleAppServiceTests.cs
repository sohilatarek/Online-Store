using OnlineStore.Samples;
using Xunit;

namespace OnlineStore.EntityFrameworkCore.Applications;

[Collection(OnlineStoreTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<OnlineStoreEntityFrameworkCoreTestModule>
{

}
