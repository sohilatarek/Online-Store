using OnlineStore.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace OnlineStore.Permissions
{
  
    public class OnlineStorePermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            // Create the permission group
            var onlineStoreGroup = context.AddGroup(
                OnlineStorePermissions.GroupName,
                L("Permission:OnlineStore"));

            // ==========================================
            // CATEGORIES PERMISSIONS
            // ==========================================

            var categoriesPermission = onlineStoreGroup.AddPermission(
                OnlineStorePermissions.Categories.Default,
                L("Permission:Categories"));

            categoriesPermission.AddChild(
                OnlineStorePermissions.Categories.Create,
                L("Permission:Categories.Create"));

            categoriesPermission.AddChild(
                OnlineStorePermissions.Categories.Edit,
                L("Permission:Categories.Edit"));

            categoriesPermission.AddChild(
                OnlineStorePermissions.Categories.Delete,
                L("Permission:Categories.Delete"));

            // ==========================================
            // PRODUCTS PERMISSIONS
            // ==========================================

            var productsPermission = onlineStoreGroup.AddPermission(
                OnlineStorePermissions.Products.Default,
                L("Permission:Products"));

            productsPermission.AddChild(
                OnlineStorePermissions.Products.Create,
                L("Permission:Products.Create"));

            productsPermission.AddChild(
                OnlineStorePermissions.Products.Edit,
                L("Permission:Products.Edit"));

            productsPermission.AddChild(
                OnlineStorePermissions.Products.Delete,
                L("Permission:Products.Delete"));

            productsPermission.AddChild(
                OnlineStorePermissions.Products.Publish,
                L("Permission:Products.Publish"));

            productsPermission.AddChild(
                OnlineStorePermissions.Products.ManageStock,
                L("Permission:Products.ManageStock"));
        }

        /// <summary>
        /// Helper method for localization
        /// </summary>
        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<OnlineStoreResource>(name);
        }
    }
}