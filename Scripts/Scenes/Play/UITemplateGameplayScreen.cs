namespace TheOneStudio.UITemplate.UITemplate.Scenes.Play
{
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateGameplayScreen : BaseView
    {
        public Button homeButton;
    }

    [ScreenInfo(nameof(UITemplateGameplayScreen))]
    public class UITemplateGameplayScreenPresenter : UITemplateBaseScreenPresenter<UITemplateGameplayScreen>
    {
        protected readonly SceneDirector           SceneDirector;
        protected readonly ScreenManager           ScreenManager;
        protected readonly UITemplateSoundServices SoundServices;

        public UITemplateGameplayScreenPresenter(SignalBus signalBus, SceneDirector sceneDirector, ScreenManager screenManager, UITemplateSoundServices soundServices) : base(signalBus)
        {
            this.SceneDirector = sceneDirector;
            this.ScreenManager = screenManager;
            this.SoundServices = soundServices;
        }

        protected virtual string NextSceneToLoad => "1.MainScene";

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.View.homeButton.onClick.AddListener(this.OnOpenHome);
        }

        protected virtual async void OnOpenHome()
        {
            await this.ScreenManager.OpenScreen<UITemplateHomeTapToPlayScreenPresenter>();
        }

        protected virtual void OpenNextScene()
        {
            this.SceneDirector.LoadSingleSceneAsync(this.NextSceneToLoad);
        }

        public override void BindData()
        {
        }
    }
}