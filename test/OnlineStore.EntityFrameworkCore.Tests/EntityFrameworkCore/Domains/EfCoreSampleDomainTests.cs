using OnlineStore.Samples;
using Xunit;

namespace OnlineStore.EntityFrameworkCore.Domains;

[Collection(OnlineStoreTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<OnlineStoreEntityFrameworkCoreTestModule>
{

}
