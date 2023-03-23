namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main
{
    using System.Collections.Generic;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
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
    public class UITemplateLevelSelectScreenPresenter : UITemplateBaseScreenPresenter<UITemplateLevelSelectScreenView>
    {
        public UITemplateLevelSelectScreenPresenter(SignalBus                         signalBus,                         DiContainer                   diContainer, IScreenManager screenManager, UITemplateUserLevelData userLevelData,
                                                    UITemplateInventoryDataController uiTemplateInventoryDataController, UITemplateLevelDataController uiTemplateLevelDataController) : base(signalBus)
        {
            this.UserLevelData                     = userLevelData;
            this.uiTemplateInventoryDataController = uiTemplateInventoryDataController;
            this.uiTemplateLevelDataController     = uiTemplateLevelDataController;
            this.diContainer                       = diContainer;
            this.screenManager                     = screenManager;
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.View.HomeButton.onClick.AddListener(this.OnClickHome);
        }

        protected virtual void OnClickHome()
        {
            this.screenManager.OpenScreen<UITemplateHomeSimpleScreenPresenter>();
        }

        public override async void BindData()
        {
            this.View.CoinText.Subscribe(this.SignalBus, this.uiTemplateInventoryDataController.GetCurrency().Value);
            var levelList    = this.getLevelList();
            var currentLevel = this.UserLevelData.CurrentLevel;
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
            return this.uiTemplateLevelDataController.GetAllLevels();
        }

        #region inject

        protected readonly DiContainer                       diContainer;
        protected readonly IScreenManager                    screenManager;
        protected readonly UITemplateUserLevelData           UserLevelData;
        private readonly   UITemplateInventoryDataController uiTemplateInventoryDataController;
        private readonly   UITemplateLevelDataController     uiTemplateLevelDataController;

        #endregion
    }
}