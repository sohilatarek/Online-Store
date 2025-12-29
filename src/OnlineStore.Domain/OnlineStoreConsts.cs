using Volo.Abp.Identity;

namespace OnlineStore;

public static class OnlineStoreConsts
{
    public const int GeneralTextMaxLength = 500;
    public const int DescriptionTextMaxLength = 2000;
    public const string AdminEmailDefaultValue = IdentityDataSeedContributor.AdminEmailDefaultValue;
    public const string AdminPasswordDefaultValue = IdentityDataSeedContributor.AdminPasswordDefaultValue;
}
