namespace TheOneStudio.UITemplate.UITemplate.Scenes.Leaderboard
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.Utilities.LogService;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using TheOneStudio.UITemplate.UITemplate.Services.CountryFlags.CountryFlags.Scripts;
    using TMPro;
    using UnityEngine;
    using UnityEngine.Scripting;
    using UnityEngine.UI;
    using Object = UnityEngine.Object;
    using Random = UnityEngine.Random;

    public class UITemplateLeaderboardPopupView : BaseView
    {
        public UITemplateLeaderboardAdapter Adapter;
        public Button                       CloseButton;
        public Transform                    YourRankerParentTransform;
        public CountryFlags                 CountryFlags;
        public TMP_Text                     BetterThanText;
        public int                          MaxLevel    = 200;
        public int                          LowestRank  = 68365;
        public int                          HighestRank = 156;
        public int                          RankRange => this.LowestRank - this.HighestRank;
    }

    [PopupInfo(nameof(UITemplateLeaderboardPopupView), false)]
    public class UITemplateLeaderBoardPopupPresenter : UITemplateBasePopupPresenter<UITemplateLeaderboardPopupView>
    {
        private const string SFXLeaderboard = "sfx_leaderboard";

        #region inject

        private readonly UITemplateLevelDataController uiTemplateLevelDataController;
        private readonly UITemplateSoundServices       uiTemplateSoundServices;

        #endregion

        private GameObject              yourClone;
        private CancellationTokenSource animationCancelTokenSource;
        private List<Tween>             animationTweenList = new();

        [Preserve]
        public UITemplateLeaderBoardPopupPresenter(
            SignalBus                     signalBus,
            ILogService                   logger,
            UITemplateLevelDataController uiTemplateLevelDataController,
            UITemplateSoundServices       uiTemplateSoundServices
        ) : base(signalBus, logger)
        {
            this.uiTemplateLevelDataController = uiTemplateLevelDataController;
            this.uiTemplateSoundServices       = uiTemplateSoundServices;
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.View.CloseButton.onClick.AddListener(this.CloseView);
        }

        private int GetRankWithLevel(int level) => (int)(this.View.LowestRank - Mathf.Sqrt(Mathf.Sqrt(level * 1f / this.View.MaxLevel)) * this.View.RankRange);

        public override UniTask BindData()
        {
            this.DoAnimation().Forget();
            return UniTask.CompletedTask;
        }

        private async UniTaskVoid DoAnimation()
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

            this.uiTemplateSoundServices.PlaySound(SFXLeaderboard);

            //Setup view
            await this.View.Adapter.InitItemAdapter(TestList);
            this.View.Adapter.ScrollTo(oldIndex - indexPadding);

            //Create your clone
            this.yourClone                                   = Object.Instantiate(this.View.Adapter.GetItemViewsHolderIfVisible(oldIndex).root.gameObject, this.View.YourRankerParentTransform);
            this.yourClone.GetComponent<CanvasGroup>().alpha = 1;
            var cloneView = this.yourClone.GetComponent<UITemplateLeaderboardItemView>();
            this.View.BetterThanText.text = this.GetBetterThanText(oldRank);

            this.animationCancelTokenSource = new CancellationTokenSource();
            //Do animation
            //Do scale up
            this.animationTweenList.Clear();
            this.animationTweenList.Add(this.yourClone.transform.DOScale(Vector3.one * 1.1f, scaleTime).SetEase(Ease.InOutBack).SetUpdate(isIndependentUpdate: true));
            await UniTask.Delay(TimeSpan.FromSeconds(scaleTime), cancellationToken: this.animationCancelTokenSource.Token, ignoreTimeScale: true);
            //Do move to new rank
            cloneView.ShowRankUP();
            this.animationTweenList.Add(DOTween.To(() => 0, setValue => cloneView.SetRankUp(setValue), oldRank - newRank, scrollDuration).SetUpdate(isIndependentUpdate: true));
            this.animationTweenList.Add(DOTween.To(() => oldRank, setValue =>
            {
                cloneView.SetRank(setValue);
                this.View.BetterThanText.text = this.GetBetterThanText(setValue);
            }, newRank, scrollDuration).SetUpdate(isIndependentUpdate: true));
            this.View.Adapter.SmoothScrollTo(newIndex - indexPadding, scrollDuration);
            await UniTask.Delay(TimeSpan.FromSeconds(scrollDuration), cancellationToken: this.animationCancelTokenSource.Token, ignoreTimeScale: true);
            //Do scale down
            this.animationTweenList.Add(this.yourClone.transform.DOScale(Vector3.one, scaleTime).SetEase(Ease.InOutBack).SetUpdate(isIndependentUpdate: true));
            await UniTask.Delay(TimeSpan.FromSeconds(scaleTime + 2), cancellationToken: this.animationCancelTokenSource.Token, ignoreTimeScale: true);
            this.CloseView();
        }

        private string GetBetterThanText(int currentRank) =>
            $"you are better than <color=#2DF2FF><size=120%>{(this.View.LowestRank * 1.5f - currentRank) / (this.View.LowestRank * 1.5f) * 100:F2}%</size ></color > of people";

        public override void Dispose()
        {
            base.Dispose();
            this.uiTemplateSoundServices.StopSound(SFXLeaderboard);
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