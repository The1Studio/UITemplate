namespace TheOneStudio.UITemplate.UITemplate.Scenes.Leaderboard
{
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateLeaderBoardPopupView : BaseView
    {
        public UITemplateLeaderboardAdapter Adapter;
        public Button                       CloseButton;
    }

    public class UITemplateLeaderBoardPopupPresenter : BasePopupPresenter<UITemplateLeaderBoardPopupView>
    {
        public UITemplateLeaderBoardPopupPresenter(SignalBus signalBus) : base(signalBus)
        {
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.View.CloseButton.onClick.AddListener(() => this.CloseView());
        }

        public override void BindData()
        {
        }
    }
}