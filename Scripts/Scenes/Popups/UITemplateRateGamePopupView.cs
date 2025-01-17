namespace TheOneStudio.UITemplate.UITemplate.Scenes.Popups
{
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.Utilities.LogService;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using TheOneStudio.UITemplate.UITemplate.Services.StoreRating;
    using UnityEngine;
    using UnityEngine.Scripting;
    using UnityEngine.UI;

    public class UITemplateRateGamePopupView : BaseView
    {
        public List<Button> starButtons;
        public List<Image>  starImages;
        public Button       yesButton;
        public Button       laterButton;
    }

    [PopupInfo(nameof(UITemplateRateGamePopupView))]
    public class UITemplateRateGamePopupPresenter : UITemplateBasePopupPresenter<UITemplateRateGamePopupView>
    {
        #region inject

        private readonly UITemplateStoreRatingHandler storeRatingHandler;

        [Preserve]
        public UITemplateRateGamePopupPresenter(
            SignalBus                    signalBus,
            ILogService                  logger,
            UITemplateStoreRatingHandler storeRatingHandler
        ) : base(signalBus, logger)
        {
            this.storeRatingHandler = storeRatingHandler;
        }

        #endregion

        private int lastStarCount;

        public override UniTask BindData()
        {
            this.lastStarCount = 0;
            this.View.yesButton.gameObject.SetActive(false);
            this.View.starImages.ForEach(star => star.transform.localScale = Vector3.zero);

            return UniTask.CompletedTask;
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();

            this.View.yesButton.onClick.AddListener(this.OnClickYes);
            this.View.laterButton.onClick.AddListener(this.OnClickLater);
            for (var i = 0; i < this.View.starButtons.Count; i++)
            {
                var closureIndex = i;
                var star         = this.View.starButtons[closureIndex];
                star.onClick.AddListener(() =>
                {
                    this.View.yesButton.gameObject.SetActive(true);
                    this.OnClickStar(closureIndex + 1);
                });
            }

            this.YesButtonAnimation();
        }

        private void OnClickStar(int count)
        {
            this.lastStarCount = count;
            for (var i = 0; i < count; i++) this.StarAnimation(i);
            for (var i = count; i < this.View.starButtons.Count; i++) this.StarAnimation(i, false);
        }

        private void YesButtonAnimation()
        {
            this.View.yesButton.transform.localScale = Vector3.one;
            this.View.yesButton.transform.DOScale(Vector3.one * 1.1f, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
        }

        private void StarAnimation(int index, bool willActive = true)
        {
            if (index >= this.View.starButtons.Count || index < 0) return;

            var star        = this.View.starImages[index];
            var targetScale = willActive ? Vector3.one : Vector3.zero;
            var easeType    = willActive ? Ease.OutElastic : Ease.OutCirc;
            var duration    = willActive ? 0.5f : 0.3f;

            star.transform.DOScale(targetScale, duration).SetLoops(1, LoopType.Yoyo).SetEase(easeType);
        }

        protected virtual void OnClickLater() { this.CloseView(); }

        protected virtual void OnClickYes()
        {
            if (this.lastStarCount == this.View.starButtons.Count) // max rating
                this.storeRatingHandler.LaunchStoreRating();
            this.CloseView();
        }
    }
}