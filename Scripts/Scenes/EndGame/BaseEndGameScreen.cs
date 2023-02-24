namespace TheOneStudio.UITemplate.UITemplate.Scenes.EndGame
{
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices;
    using UnityEngine.UI;
    using Zenject;

    public abstract class BaseEndGameScreenView : BaseView
    {
        public Button btnNext;
    }

    public abstract class BaseEndGameScreenPresenter<TView> : BaseScreenPresenter<TView> where TView : BaseEndGameScreenView
    {
        protected readonly AdServiceWrapper AdService;
        protected BaseEndGameScreenPresenter(SignalBus signalBus, AdServiceWrapper adService) : base(signalBus) { this.AdService = adService; }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.View.btnNext.onClick.AddListener(this.OnClickNext);
        }

        public override void BindData() { }

        protected virtual void OnClickNext() { }
    }
}