namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Signals;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.FeaturesConfig;
    using TheOneStudio.UITemplate.UITemplate.Scenes.IapScene;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main;
    using Zenject;

    public class UITemplateAutoOpenStartedPackServices : IInitializable
    {
        private readonly SignalBus                  signalBus;
        private readonly UITemplateCommonController uiTemplateCommonController;
        private readonly ScreenManager              screenManager;
        private readonly UITemplateFeatureConfig    uiTemplateFeatureConfig;

        public UITemplateAutoOpenStartedPackServices(SignalBus signalBus, UITemplateCommonController uiTemplateCommonController, ScreenManager screenManager, UITemplateFeatureConfig uiTemplateFeatureConfig)
        {
            this.signalBus                  = signalBus;
            this.uiTemplateCommonController = uiTemplateCommonController;
            this.screenManager              = screenManager;
            this.uiTemplateFeatureConfig    = uiTemplateFeatureConfig;
        }

        public void Initialize() { this.signalBus.Subscribe<ScreenShowSignal>(this.OnScreenShowSignal); }

        private void OnScreenShowSignal(ScreenShowSignal obj)
        {
            if (!this.uiTemplateCommonController.IsFirstTimeOpenGame) return;
            if (!this.uiTemplateFeatureConfig.IsIAPEnable) return;

            if (obj.ScreenPresenter is UITemplateHomeSimpleScreenPresenter or UITemplateHomeTapToPlayScreenPresenter)
            {
                this.uiTemplateCommonController.ChangeGameIsAlreadyOpened();

                _ = this.screenManager.OpenScreen<UITemplateStartPackScreenPresenter, UITemplateStaterPackModel>(new UITemplateStaterPackModel()
                {
                });
            }
        }
    }
}