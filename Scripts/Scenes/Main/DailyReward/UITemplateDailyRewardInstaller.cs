#if GDK_ZENJECT
namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main.DailyReward
{
    using System.Linq;
    using GameFoundation.Scripts.Utilities.Extension;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main.DailyReward.Item;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main.DailyReward.Pack;
    using Zenject;

    public class UITemplateDailyRewardInstaller : Installer<UITemplateDailyRewardInstaller>
    {
        public override void InstallBindings()
        {
            this.Container.Bind<UITemplateDailyRewardItemViewHelper>().AsCached();
            this.Container.Bind<UITemplateDailyRewardPackViewHelper>().AsCached();
            var concreteDailyRewardAnimationHelperType = ReflectionUtils.GetAllDerivedTypes<DailyRewardAnimationHelper>()
                                   .OrderBy(type => type == typeof(DailyRewardAnimationHelper))
                                   .First();
            this.Container.Bind<DailyRewardAnimationHelper>().To(concreteDailyRewardAnimationHelperType).AsCached();
        }
    }
}
#endif