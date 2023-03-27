namespace TheOneStudio.UITemplate.UITemplate.Scenes.Popups
{
    using System.Collections.Generic;
    using DG.Tweening;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateRateGameScreenView : BaseView
    {
        public TMP_Text     TileText;
        public List<Button> StarButtons;
        public List<Image>  StarImages;
        public Button       YesButton;
        public Button       LaterButton;
        public GameObject   Panel;
    }

    [PopupInfo(nameof(UITemplateRateGameScreenView))]
    public class UITemplateRateGameScreenPresenter : UITemplateBasePopupPresenter<UITemplateRateGameScreenView>
    {
        public static string StoreUrl = "https://play.google.com/store/games";
        private int lastStarCount;

        public UITemplateRateGameScreenPresenter(SignalBus signalBus, DiContainer diContainer, IScreenManager screenManager) : base(signalBus)
        {
            this.diContainer   = diContainer;
            this.screenManager = screenManager;
        }

        public override void BindData()
        {
            this.lastStarCount = 0;
            for (var i = 0; i < this.View.StarImages.Count; i++)
            {
                var star = this.View.StarImages[i];
                star.transform.localScale = Vector3.zero;
            }
        }
        protected override void OnViewReady()
        {
            base.OnViewReady();
            
            this.View.YesButton.onClick.AddListener(this.OnClickYes);
            this.View.LaterButton.onClick.AddListener(this.OnClickLater);
            for (var i = 0; i < this.View.StarButtons.Count; i++)
            {
                var closureIndex = i;
                var star         = this.View.StarButtons[closureIndex];
                star.onClick.AddListener(() => this.OnClickStar(closureIndex + 1));
            }

            this.YesButtonAnimation();
        }

        private void OnClickStar(int count)
        {
            this.lastStarCount = count;
            for (var i = 0; i < count; i++)
                this.StarAnimation(i);
            for (var i = count; i < this.View.StarButtons.Count; i++)
                this.StarAnimation(i, false);
        }

        private void YesButtonAnimation()
        {
            this.View.YesButton.transform.localScale = Vector3.one;
            this.View.YesButton.transform.DOScale(Vector3.one * 1.1f, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
        }

        private void StarAnimation(int index, bool WillActive = true)
        {
            if (index >= this.View.StarButtons.Count || index < 0) return;

            var star        = this.View.StarImages[index];
            var targetScale = WillActive ? Vector3.one : Vector3.zero;
            var easeType    = WillActive ? Ease.OutElastic : Ease.OutCirc;
            var duration    = WillActive ? 0.5f : 0.3f;

            star.transform.DOScale(targetScale, duration).SetLoops(1, LoopType.Yoyo).SetEase(easeType);
        }


        protected void OnClickLater()
        {
            this.screenManager.CloseCurrentScreen();
        }

        protected void OnClickYes()
        {
            if (this.lastStarCount == this.View.StarButtons.Count) Application.OpenURL(StoreUrl);
            this.screenManager.CloseCurrentScreen();
        }

        #region inject

        protected readonly DiContainer    diContainer;
        protected readonly IScreenManager screenManager;

        #endregion
    }
}