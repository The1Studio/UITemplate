namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main
{
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.Utilities.LogService;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main.Level;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using UnityEngine.Scripting;
    using UnityEngine.UI;

    public class UITemplateLevelSelectScreenView : BaseView
    {
        public Button                     HomeButton;
        public UITemplateCurrencyView     CoinText;
        public UITemplateLevelGridAdapter LevelGridAdapter;
    }

    [ScreenInfo(nameof(UITemplateLevelSelectScreenView))]
    public class UITemplateLevelSelectScreenPresenter : UITemplateBaseScreenPresenter<UITemplateLevelSelectScreenView>
    {
        [Preserve]
        public UITemplateLevelSelectScreenPresenter(
            SignalBus                         signalBus,
            ILogService                       logger,
            IScreenManager                    screenManager,
            UITemplateInventoryDataController uiTemplateInventoryDataController,
            UITemplateLevelDataController     uiTemplateLevelDataController
        ) : base(signalBus, logger)
        {
            this.uiTemplateInventoryDataController = uiTemplateInventoryDataController;
            this.uiTemplateLevelDataController     = uiTemplateLevelDataController;
            this.screenManager                     = screenManager;
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.View.HomeButton.onClick.AddListener(this.OnClickHome);
        }

        protected virtual void OnClickHome() { this.screenManager.OpenScreen<UITemplateHomeSimpleScreenPresenter>(); }

        public override async UniTask BindData()
        {
            var levelList    = this.getLevelList();
            var currentLevel = this.uiTemplateLevelDataController.GetCurrentLevelData.Level;
            await this.View.LevelGridAdapter.InitItemAdapter(levelList);
            this.View.LevelGridAdapter.SmoothScrollTo(currentLevel, 1);
        }

        private List<LevelData> getLevelList() { return this.uiTemplateLevelDataController.GetAllLevels(); }

        #region inject

        protected readonly IScreenManager                    screenManager;
        private readonly   UITemplateInventoryDataController uiTemplateInventoryDataController;
        private readonly   UITemplateLevelDataController     uiTemplateLevelDataController;

        #endregion
    }
}