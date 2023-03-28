namespace TheOneStudio.UITemplate.UITemplate.Scenes.Leaderboard
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;
    using Random = UnityEngine.Random;

    public class UITemplateLeaderboardPopupView : BaseView
    {
        public UITemplateLeaderboardAdapter Adapter;
        public Button                       CloseButton;
    }

    [PopupInfo(nameof(UITemplateLeaderboardPopupView))]
    public class UITemplateLeaderBoardPopupPresenter : BasePopupPresenter<UITemplateLeaderboardPopupView>
    {
        #region inject

        private readonly DiContainer diContainer;

        #endregion
        
        public UITemplateLeaderBoardPopupPresenter(SignalBus signalBus, DiContainer diContainer) : base(signalBus) { this.diContainer = diContainer; }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.View.CloseButton.onClick.AddListener(() => this.CloseView());
        }

        public override async void BindData()
        {
            var TestList = new List<UITemplateLeaderboardItemModel>();
            for (var i = 0; i < 100; i++)
            {
                TestList.Add(new UITemplateLeaderboardItemModel(i, "VN", NVJOBNameGen.GiveAName(Random.Range(1, 8)), false));
            }

            var newIndex = 6;
            var oldIndex = 97;

            var currentRegion = System.Globalization.RegionInfo.CurrentRegion;
            Debug.Log(currentRegion);


            TestList[newIndex].IsYou = true;
            TestList[oldIndex].IsYou = true;

            await this.View.Adapter.InitItemAdapter(TestList, this.diContainer);
            this.View.Adapter.ScrollTo(99);
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            this.View.Adapter.SmoothScrollTo(newIndex - 2, 3);
            //Do animation
        }
    }
}