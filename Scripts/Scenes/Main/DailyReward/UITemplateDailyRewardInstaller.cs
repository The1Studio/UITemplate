namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main.DailyReward
{
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main.DailyReward.Item;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main.DailyReward.Pack;
    using Zenject;

    public class UITemplateDailyRewardInstaller : Installer<UITemplateDailyRewardInstaller>
    {
        public override void InstallBindings()
        {
            this.Container.Bind<UITemplateDailyRewardItemViewHelper>().AsCached();
            this.Container.Bind<UITemplateDailyRewardPackViewHelper>().AsCached();
        }
    }
}