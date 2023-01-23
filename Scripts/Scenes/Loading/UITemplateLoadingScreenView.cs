namespace UITemplate.Scripts.Scenes.Loading
{
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using UnityEngine;
    using Zenject;

    public class UITemplateLoadingScreenView : BaseView
    {
    }

    [ScreenInfo(nameof(UITemplateLoadingScreenView))]
    public class UITemplateLoadingScreenPresenter : BaseScreenPresenter<UITemplateLoadingScreenView>
    {
        public UITemplateLoadingScreenPresenter(SignalBus signalBus) : base(signalBus)
        {
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.BindData();
        }

        public override void BindData()
        {
            Debug.Log("Bind loading data");
        }
    }
}