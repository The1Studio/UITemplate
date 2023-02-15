namespace UITemplate.Scripts.Scenes.Main
{
    using System.Collections.Generic;
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

        [Inject] private readonly DiContainer         diContainer;
        [Inject] private readonly IScreenManager      screenManager;
        [Inject] private readonly UITemplateLevelData levelData;

        #endregion

        public UITemplateLevelSelectScreenPresenter(SignalBus signalBus) : base(signalBus) { }

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
            // add to list the rest of the levels as closed levels
            return getLevelListTest(0, 10000);
        }
        private List<UITemplateLevelItemModel> getLevelListTest(int from, int to)
        {
            var list = new List<UITemplateLevelItemModel>();

            #region test data

            for (int i = from; i < to; i++) 
                list.Add(
                    new UITemplateLevelItemModel(
                        new UITemplateLevelRecord(), 
                        i+1,
                        Models.LevelData.Status.Locked,
                        0
                        )
                    );
            list[0].LevelStatus    = Models.LevelData.Status.Passed;
            list[0].StarCount      = 3;
            list[1].LevelStatus    = Models.LevelData.Status.Passed;
            list[1].StarCount      = 2;
            list[2].LevelStatus    = Models.LevelData.Status.Passed;
            list[2].StarCount      = 1;
            list[3].LevelStatus    = Models.LevelData.Status.Skipped;
            list[3].StarCount      = 3;
            list[4].LevelStatus    = Models.LevelData.Status.Passed;
            list[4].StarCount      = 3;
            list[1000].LevelStatus = Models.LevelData.Status.Now;

            #endregion

            return list;
        }
    }
}