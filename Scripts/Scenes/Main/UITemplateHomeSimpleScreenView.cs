namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main
{
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.Utilities.LogService;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Configs.GameEvents;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using UnityEngine.Scripting;
    using UnityEngine.UI;

    public class UITemplateHomeSimpleScreenView : BaseView
    {
        public Button                      PlayButton;
        public Button                      LevelButton;
        public Button                      ShopButton;
        public UITemplateSettingButtonView SettingButtonView;
    }

    [ScreenInfo(nameof(UITemplateHomeSimpleScreenView))]
    public class UITemplateHomeSimpleScreenPresenter : UITemplateBaseScreenPresenter<UITemplateHomeSimpleScreenView>
    {
        [Preserve]
        public UITemplateHomeSimpleScreenPresenter(
            SignalBus           signalBus,
            ILogService         logger,
            IScreenManager      screenManager,
            GameFeaturesSetting gameFeaturesSetting
        ) : base(signalBus, logger)
        {
            this.ScreenManager       = screenManager;
            this.gameFeaturesSetting = gameFeaturesSetting;
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            if (this.gameFeaturesSetting.enableInitHomeScreenManually)
            {
                this.OpenViewAsync().Forget();
            }
            this.View.PlayButton.onClick.AddListener(this.OnClickPlay);

            if (this.View.LevelButton != null)
            {
                this.View.LevelButton.onClick.AddListener(this.OnClickLevel);
            }

            if (this.View.ShopButton != null)
            {
                this.View.ShopButton.onClick.AddListener(this.OnClickShop);
            }
        }

        protected virtual void OnClickLevel() { }

        protected virtual void OnClickShop() { }

        protected virtual void OnClickPlay() { }

        public override UniTask BindData() { return UniTask.CompletedTask; }

        #region inject

        protected readonly IScreenManager      ScreenManager;
        private readonly   GameFeaturesSetting gameFeaturesSetting;

        #endregion
    }
}