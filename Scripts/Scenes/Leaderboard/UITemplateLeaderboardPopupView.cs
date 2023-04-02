namespace TheOneStudio.UITemplate.UITemplate.Scenes.Leaderboard
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Services.CountryFlags.CountryFlags.Scripts;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;
    using Object = UnityEngine.Object;
    using Random = UnityEngine.Random;

    public class UITemplateLeaderboardPopupView : BaseView
    {
        public UITemplateLeaderboardAdapter Adapter;
        public Button                       CloseButton;
        public Transform                    YourRankerParentTransform;
        public CountryFlags                 CountryFlags;
        public int                          MaxLevel    = 200;
        public int                          LowestRank  = 68365;
        public int                          HighestRank = 156;
    }

    [PopupInfo(nameof(UITemplateLeaderboardPopupView), false)]
    public class UITemplateLeaderBoardPopupPresenter : BasePopupPresenter<UITemplateLeaderboardPopupView>
    {
        #region inject

        private readonly DiContainer                   diContainer;
        private readonly UITemplateLevelDataController uiTemplateLevelDataController;

        #endregion

        private GameObject              yourClone;
        private CancellationTokenSource animationCancelTokenSource;
        private List<Tween>             animationTweenList = new();

        public UITemplateLeaderBoardPopupPresenter(SignalBus signalBus, DiContainer diContainer, UITemplateLevelDataController uiTemplateLevelDataController) : base(signalBus)
        {
            this.diContainer                   = diContainer;
            this.uiTemplateLevelDataController = uiTemplateLevelDataController;
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.View.CloseButton.onClick.AddListener(this.CloseView);
        }

        private int GetRankWithLevel(int level) => (int)(this.View.LowestRank - 1f * level / this.View.MaxLevel * this.View.LowestRank + this.View.HighestRank);

        public override async UniTask BindData()
        {
            var indexPadding   = 4;
            var scrollDuration = 3;
            var scaleTime      = 1f;

            var TestList = new List<UITemplateLeaderboardItemModel>();

            var currentLevel = this.uiTemplateLevelDataController.GetCurrentLevelData.Level;
            var oldRank      = this.GetRankWithLevel(currentLevel - 1);
            var newRank      = this.GetRankWithLevel(currentLevel);
            var newIndex     = indexPadding;
            var oldIndex     = (oldRank - newRank - indexPadding);

            for (var i = newRank - indexPadding; i < oldRank + indexPadding; i++)
            {
                TestList.Add(new UITemplateLeaderboardItemModel(i, this.View.CountryFlags.GetRandomFlag(), NVJOBNameGen.GiveAName(Random.Range(1, 8)), false));
            }

            TestList[newIndex].IsYou       = true;
            TestList[oldIndex].IsYou       = true;
            TestList[oldIndex].CountryFlag = this.View.CountryFlags.GetLocalDeviceFlagByDeviceLang();
            TestList[oldIndex].Name        = "You";

            //Setup view
            await this.View.Adapter.InitItemAdapter(TestList, this.diContainer);
            this.View.Adapter.ScrollTo(oldIndex - indexPadding);
            
            //Create your clone
            this.yourClone                                   = Object.Instantiate(this.View.Adapter.GetItemViewsHolderIfVisible(oldIndex).root.gameObject, this.View.YourRankerParentTransform);
            this.yourClone.GetComponent<CanvasGroup>().alpha = 1;
            var cloneView = this.yourClone.GetComponent<UITemplateLeaderboardItemView>();
            
            this.animationCancelTokenSource = new CancellationTokenSource();
            //Do animation
            //Do scale up
            this.animationTweenList.Clear();
            this.animationTweenList.Add(this.yourClone.transform.DOScale(Vector3.one * 1.1f, scaleTime).SetEase(Ease.InOutBack));
            await UniTask.Delay(TimeSpan.FromSeconds(scaleTime), cancellationToken: this.animationCancelTokenSource.Token);
            //Do move to new rank
            this.animationTweenList.Add(DOTween.To(() => oldRank, setValue => cloneView.SetRank(setValue), newRank, scrollDuration));
            this.View.Adapter.SmoothScrollTo(newIndex - indexPadding, scrollDuration);
            await UniTask.Delay(TimeSpan.FromSeconds(scrollDuration), cancellationToken: this.animationCancelTokenSource.Token);
            //Do scale down
            this.animationTweenList.Add(this.yourClone.transform.DOScale(Vector3.one, scaleTime).SetEase(Ease.InOutBack));
        }

        public override void Dispose()
        {
            base.Dispose();
            this.animationCancelTokenSource.Cancel();
            this.animationCancelTokenSource.Dispose();
            this.View.Adapter.StopScrollingIfAny();
            foreach (var tween in this.animationTweenList)
            {
                tween.Kill();
            }
            Object.Destroy(this.yourClone);
        }
    }
}