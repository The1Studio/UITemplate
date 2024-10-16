#if GDK_VCONTAINER
#nullable enable
namespace TheOneStudio.UITemplate
{
    using GameFoundation.Scripts.Utilities.Extension;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main.DailyReward;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main.DailyReward.Item;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main.DailyReward.Pack;
    using VContainer;

    public static class UITemplateDailyRewardVContainer
    {
        public static void RegisterDailyReward(this IContainerBuilder builder)
        {
            builder.RegisterFromDerivedType<UITemplateDailyRewardItemViewHelper>();
            builder.RegisterFromDerivedType<UITemplateDailyRewardPackViewHelper>();
            builder.RegisterFromDerivedType<DailyRewardAnimationHelper>();
        }
    }
}
#endif