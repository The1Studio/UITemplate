#if GDK_VCONTAINER
#nullable enable
namespace TheOneStudio.UITemplate
{
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main.DailyReward.Item;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main.DailyReward.Pack;
    using VContainer;

    public static class UITemplateDailyRewardVContainer
    {
        public static void RegisterUITemplateDailyReward(this IContainerBuilder builder)
        {
            builder.Register<UITemplateDailyRewardItemViewHelper>(Lifetime.Singleton);
            builder.Register<UITemplateDailyRewardPackViewHelper>(Lifetime.Singleton);
        }
    }
}
#endif