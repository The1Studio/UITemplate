namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main
{
    using System.Collections.Generic;
    using System.Linq;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main.Level;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateLevelSelectScreenView : BaseView
    {
        public Button                     HomeButton;
        public UITemplateCurrencyView     CoinText;
        public UITemplateLevelGridAdapter LevelGridAdapter;
    }

    [ScreenInfo(nameof(UITemplateLevelSelectScreenView))]
    public class UITemplateLevelSelectScreenPresenter : BaseScreenPresenter<UITemplateLevelSelectScreenView>
    {
        #region inject

        protected readonly DiContainer         diContainer;
        protected readonly IScreenManager      screenManager;
        protected readonly UITemplateLevelData levelData;

        #endregion

        public UITemplateLevelSelectScreenPresenter(SignalBus signalBus, DiContainer diContainer, IScreenManager screenManager, UITemplateLevelData levelData) : base(signalBus)
        {
            this.levelData    = levelData;
            this.diContainer  = diContainer;
            this.screenManager = screenManager;
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.View.HomeButton.onClick.AddListener(this.OnClickHome);
        }

        protected virtual void OnClickHome() { this.screenManager.OpenScreen<UITemplateHomeSimpleScreenPresenter>(); }

        public override async void BindData()
        {
            this.View.CoinText.Subscribe(this.SignalBus);
            var levelList    = this.getLevelList();
            var currentLevel = this.levelData.CurrentLevel;
            await this.View.LevelGridAdapter.InitItemAdapter(levelList, this.diContainer);
            this.View.LevelGridAdapter.SmoothScrollTo(currentLevel, 1);
        }
        public override void Dispose()
        {
            base.Dispose();
            this.View.CoinText.Unsubscribe(this.SignalBus);
        }

        private List<LevelData> getLevelList()
        {
            return this.levelData.GetAllLevels();
        }
    }
}