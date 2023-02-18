namespace UITemplate.Scripts.Scenes.Main
{
    using System.Collections.Generic;
    using System.Linq;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using UITemplate.Scripts.Blueprints;
    using UITemplate.Scripts.Models;
    using UITemplate.Scripts.Scenes.Main.Level;
    using UITemplate.Scripts.Scenes.Popups;
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

        private readonly DiContainer         diContainer;
        private readonly IScreenManager      screenManager;
        private readonly UITemplateLevelData levelData;

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

        private void OnClickHome() { this.screenManager.OpenScreen<UITemplateHomeSimpleScreenPresenter>(); }

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

        private List<UITemplateLevelItemModel> getLevelList()
        {
            // get level list from local data
            var levelList = this.levelData.LevelToLevelData.Values.Cast<UITemplateLevelItemModel>().ToList();
            var allLevels = this.levelData.GetAllLevels().Cast<UITemplateLevelItemModel>().ToList();
            // add to list the rest of the levels as locked levels
            levelList.AddRange(allLevels.GetRange(levelList.Count, allLevels.Count - levelList.Count));
            // return levelList;
            return this.getLevelListWithoutStarTest(0, 10000);
        }
        private List<UITemplateLevelItemModel> getLevelListWithoutStarTest(int from, int to) // Temporary method for testing. Remove it when you have a real data source
        {
            var list = new List<UITemplateLevelItemModel>();

            #region test data

            for (int i = from; i < to; i++)
                list.Add(
                    new UITemplateLevelItemModel(
                        new UITemplateLevelRecord(),
                        i + 1,
                        Models.LevelData.Status.Locked
                    )
                );
            list[0].LevelStatus = Models.LevelData.Status.Passed;
            list[1].LevelStatus = Models.LevelData.Status.Passed;
            list[2].LevelStatus = Models.LevelData.Status.Passed;
            list[3].LevelStatus = Models.LevelData.Status.Skipped;
            list[4].LevelStatus = Models.LevelData.Status.Passed;

            #endregion

            return list;
        }
    }
}