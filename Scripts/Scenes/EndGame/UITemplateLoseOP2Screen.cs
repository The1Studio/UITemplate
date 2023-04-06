namespace TheOneStudio.UITemplate.UITemplate.Scenes.EndGame
{
    using Core.AdsServices;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateLoseOP2Screen : BaseEndGameScreenView
    {
        public Button btnContinue;
    }

    [ScreenInfo(nameof(UITemplateLoseOP2Screen))]
    public class UITemplateLoseOp2Presenter : BaseEndGameScreenPresenter<UITemplateLoseOP2Screen>
    {
        public UITemplateLoseOp2Presenter(SignalBus signalBus, UITemplateAdServiceWrapper uiTemplateAdService, UITemplateSoundServices soundServices) : base(signalBus, uiTemplateAdService, soundServices)
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
            this.UITemplateAdService.ShowMREC(AdViewPosition.Centered);
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
            this.UITemplateAdService.HideMREC(AdViewPosition.Centered);
        }

        protected virtual void AfterWatchAd()
        {
        }

        protected override void OnClickNext()
        {
        }
    }
}