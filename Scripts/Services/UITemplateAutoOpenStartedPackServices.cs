namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Signals;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.IapScene;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main;
    using Zenject;

    public class UITemplateAutoOpenStartedPackServices : IInitializable
    {
        private readonly SignalBus                  signalBus;
        private readonly UITemplateCommonController uiTemplateCommonController;
        private readonly string                     statedPackID;
        private readonly ScreenManager              screenManager;

        public UITemplateAutoOpenStartedPackServices(SignalBus signalBus, UITemplateCommonController uiTemplateCommonController, string statedPackID, ScreenManager screenManager)
        {
            this.signalBus                  = signalBus;
            this.uiTemplateCommonController = uiTemplateCommonController;
            this.statedPackID               = statedPackID;
            this.screenManager              = screenManager;
        }

        public void Initialize() { this.signalBus.Subscribe<ScreenShowSignal>(this.OnScreenShowSignal); }

        private void OnScreenShowSignal(ScreenShowSignal obj)
        {
            if (!this.uiTemplateCommonController.IsFirstTimeOpenGame) return;

            if (obj.ScreenPresenter is UITemplateHomeSimpleScreenPresenter or UITemplateHomeTapToPlayScreenPresenter)
            {
                this.uiTemplateCommonController.ChangeGameIsAlreadyOpened();

                this.screenManager.OpenScreen<UITemplateStartPackScreenPresenter, UITemplateStaterPackModel>(new UITemplateStaterPackModel()
                {
                   
                });
            }
        }
    }
}