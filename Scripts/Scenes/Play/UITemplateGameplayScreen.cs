namespace TheOneStudio.UITemplate.UITemplate.Scenes.Play
{
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateGameplayScreen : BaseView
    {
        public Button homeButton;
    }

    [ScreenInfo(nameof(UITemplateGameplayScreen))]
    public class UITemplateGameplayScreenPresenter : BaseScreenPresenter<UITemplateGameplayScreen>
    {
        protected readonly SceneDirector SceneDirector;
        protected readonly ScreenManager ScreenManager;

        protected virtual string NextSceneToLoad => "1.MainScene";

        public UITemplateGameplayScreenPresenter(SignalBus signalBus, SceneDirector sceneDirector, ScreenManager screenManager) : base(signalBus)
        {
            this.SceneDirector = sceneDirector;
            this.ScreenManager = screenManager;
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.View.homeButton.onClick.AddListener(this.OnOpenHome);
        }

        protected virtual async void OnOpenHome() { await this.ScreenManager.OpenScreen<UITemplateHomeTapToPlayScreenPresenter>(); }

        protected virtual void OpenNextScene() { this.SceneDirector.LoadSingleSceneAsync(this.NextSceneToLoad); }

        public override void BindData() { }
    }
}