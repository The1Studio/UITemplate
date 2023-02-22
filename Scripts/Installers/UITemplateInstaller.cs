namespace TheOneStudio.UITemplate.UITemplate.Installers
{
    using GameFoundation.Scripts.Interfaces;
    using GameFoundation.Scripts.Utilities;
    using TheOneStudio.UITemplate.UITemplate.Interfaces;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using Zenject;

    public class UITemplateInstaller : Installer<UITemplateInstaller>
    {
        public override void InstallBindings()
        {
            this.BindLocalData<UITemplateUserLevelData>();
            this.BindLocalData<UITemplateUserShopData>();
            this.BindLocalData<UITemplateUserInventoryData>();
            this.BindLocalData<UITemplateUserSettingData>();
            this.BindLocalData<UITemplateUserDailyRewardData>();

            this.Container.Bind<IAdsSystem>().To<UITemPlateAdsSystem>().AsCached().NonLazy();
            this.Container.Bind<IIapSystem>().To<UITemplateIAPSystem>().AsCached().NonLazy();
            //Signal
            this.Container.DeclareSignal<UpdateCurrencySignal>();
        }

        private void BindLocalData<TLocalData>() where TLocalData : class, ILocalData
        {
            this.Container.Bind<TLocalData>().FromResolveGetter<HandleLocalDataServices>(services => services.Load<TLocalData>()).AsCached().NonLazy();
        }
    }
}