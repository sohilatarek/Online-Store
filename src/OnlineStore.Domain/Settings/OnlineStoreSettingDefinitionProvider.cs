using Volo.Abp.Settings;

namespace OnlineStore.Settings;

public class OnlineStoreSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(OnlineStoreSettings.MySetting1));
    }
}
