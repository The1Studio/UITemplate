namespace TheOneStudio.UITemplate.UITemplate.Scenes.EndGame
{
    using Core.AdsServices;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Signals;
    using TheOne.Logging;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using UnityEngine.Scripting;
    using UnityEngine.UI;

    public class UITemplateLoseOP2Screen : BaseEndGameScreenView
    {
        public Button btnContinue;
    }

    [ScreenInfo(nameof(UITemplateLoseOP2Screen))]
    public class UITemplateLoseOp2Presenter : BaseEndGameScreenPresenter<UITemplateLoseOP2Screen>
    {
        [Preserve]
        public UITemplateLoseOp2Presenter(SignalBus signalBus, ILoggerManager loggerManager, UITemplateAdServiceWrapper uiTemplateAdService, UITemplateSoundServices soundServices) : base(signalBus, loggerManager, uiTemplateAdService, soundServices)
        {
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.View.btnContinue.onClick.AddListener(this.OnContinue);
        }

        public override UniTask BindData()
        {
            base.BindData();
            this.SoundServices.PlaySoundLose();
            return UniTask.CompletedTask;
        }

        protected virtual void OnContinue()
        {
            this.UITemplateAdService.ShowRewardedAd("Lose_Continue", this.AfterWatchAd);
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        protected virtual void AfterWatchAd()
        {
        }

        protected override void OnClickNext()
        {
        }
    }
}