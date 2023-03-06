namespace TheOneStudio.UITemplate.UITemplate.Scenes.EndGame
{
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices;
    using UnityEngine.UI;
    using Zenject;

    public abstract class BaseEndGameScreenView : BaseView
    {
        public Button btnNext;
    }

    public abstract class BaseEndGameScreenPresenter<TView> : BaseScreenPresenter<TView> where TView : BaseEndGameScreenView
    {
        protected readonly UITemplateAdServiceWrapper UITemplateAdService;
        protected readonly UITemplateSoundService     SoundService;
        protected BaseEndGameScreenPresenter(SignalBus signalBus, UITemplateAdServiceWrapper uiTemplateAdService, UITemplateSoundService soundService) : base(signalBus)
        {
            this.UITemplateAdService = uiTemplateAdService;
            this.SoundService        = soundService;
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.View.btnNext.onClick.AddListener(this.OnClickNext);
        }

        public override void BindData() { }

        protected virtual void OnClickNext()
        {
            this.SoundService.PlaySoundClick();
        }
    }
}