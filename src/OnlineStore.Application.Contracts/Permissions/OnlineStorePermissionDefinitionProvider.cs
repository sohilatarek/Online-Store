using OnlineStore.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace OnlineStore.Permissions;

public class OnlineStorePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(OnlineStorePermissions.GroupName);

        //Define your own permissions here. Example:
        //myGroup.AddPermission(OnlineStorePermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<OnlineStoreResource>(name);
    }
}
