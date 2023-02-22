namespace TheOneStudio.UITemplate.UITemplate.Scenes.EndGame
{
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using TheOneStudio.UITemplate.UITemplate.Interfaces;
    using UnityEngine.UI;
    using Zenject;

    public abstract class BaseEndGameScreenView : BaseView
    {
        public Button btnNext;
    }

    public abstract class BaseEndGameScreenPresenter<TView> : BaseScreenPresenter<TView> where TView : BaseEndGameScreenView
    {
        protected readonly IAdsSystem AdsSystem;
        protected BaseEndGameScreenPresenter(SignalBus signalBus, IAdsSystem adsSystem) : base(signalBus) { this.AdsSystem = adsSystem; }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.View.btnNext.onClick.AddListener(this.OnClickNext);
        }

        public override void BindData() { }

        protected virtual void OnClickNext() { }
    }
}