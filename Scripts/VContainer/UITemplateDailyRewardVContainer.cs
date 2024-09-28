#if GDK_VCONTAINER
#nullable enable
namespace TheOneStudio.UITemplate
{
    using System.Linq;
    using GameFoundation.Scripts.Utilities.Extension;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main.DailyReward;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main.DailyReward.Item;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main.DailyReward.Pack;
    using VContainer;

    public static class UITemplateDailyRewardVContainer
    {
        public static void RegisterDailyReward(this IContainerBuilder builder)
        {
            builder.Register<UITemplateDailyRewardItemViewHelper>(Lifetime.Singleton);
            builder.Register<UITemplateDailyRewardPackViewHelper>(Lifetime.Singleton);
            var concreteDailyRewardAnimationHelperType = typeof(DailyRewardAnimationHelper)
                .GetDerivedTypes()
                .OrderBy(type => type == typeof(DailyRewardAnimationHelper))
                .First();
            builder.Register(concreteDailyRewardAnimationHelperType, Lifetime.Singleton).As<DailyRewardAnimationHelper>();
        }
    }
}
#endif