namespace TheOneStudio.UITemplate.UITemplate.Scenes.Leaderboard
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.Utilities.Extension;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;
    using Object = UnityEngine.Object;
    using Random = UnityEngine.Random;

    [Serializable]
    public class CountryCodeToFlagSprite : UnitySerializedDictionary<string, Sprite>
    {
    }

    public class UITemplateLeaderboardPopupView : BaseView
    {
        public UITemplateLeaderboardAdapter Adapter;
        public Button                       CloseButton;
        public Transform                    YourRankerParentTransform;

        [SerializeField]
        public CountryCodeToFlagSprite CountryCodeToFlagSprite;
    }

    [PopupInfo(nameof(UITemplateLeaderboardPopupView))]
    public class UITemplateLeaderBoardPopupPresenter : BasePopupPresenter<UITemplateLeaderboardPopupView>
    {
        #region inject

        private readonly DiContainer diContainer;

        #endregion

        private GameObject yourClone;

        public UITemplateLeaderBoardPopupPresenter(SignalBus signalBus, DiContainer diContainer) : base(signalBus) { this.diContainer = diContainer; }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.View.CloseButton.onClick.AddListener(() => this.CloseView());
        }

        public override async void BindData()
        {
            var TestList       = new List<UITemplateLeaderboardItemModel>();
            var rankerAmount   = 100;
            var indexPadding   = 4;
            var newIndex       = 6;
            var oldIndex       = rankerAmount - 10 - indexPadding;
            var scrollDuration = 3;
            var scaleTime      = 1f;

            for (var i = 0; i < rankerAmount; i++)
            {
                TestList.Add(new UITemplateLeaderboardItemModel(i, "VN", NVJOBNameGen.GiveAName(Random.Range(1, 8)), false));
            }

            var currentRegion = RegionInfo.CurrentRegion.ThreeLetterISORegionName;
            Debug.Log(currentRegion);

            TestList[newIndex].IsYou = true;
            TestList[oldIndex].IsYou = true;

            await this.View.Adapter.InitItemAdapter(TestList, this.diContainer);
            this.View.Adapter.ScrollTo(oldIndex - indexPadding);
            this.yourClone                                   = Object.Instantiate(this.View.Adapter.GetItemViewsHolderIfVisible(oldIndex).root.gameObject, this.View.YourRankerParentTransform);
            this.yourClone.GetComponent<CanvasGroup>().alpha = 1;
            var cloneView = this.yourClone.GetComponent<UITemplateLeaderboardItemView>();

            this.yourClone.transform.DOScale(Vector3.one * 1.1f, scaleTime).SetEase(Ease.InOutBack);
            await UniTask.Delay(TimeSpan.FromSeconds(scaleTime));
            DOTween.To(() => oldIndex, setValue => cloneView.SetRank(setValue), newIndex, scrollDuration);
            this.View.Adapter.SmoothScrollTo(newIndex - indexPadding, scrollDuration);
            await UniTask.Delay(TimeSpan.FromSeconds(scrollDuration));
            this.yourClone.transform.DOScale(Vector3.one, scaleTime).SetEase(Ease.InOutBack);
        }

        public override void Dispose()
        {
            base.Dispose();
            Object.Destroy(this.yourClone);
        }
    }
}