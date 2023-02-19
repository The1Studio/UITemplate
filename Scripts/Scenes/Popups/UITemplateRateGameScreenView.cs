namespace TheOneStudio.UITemplate.UITemplate.Scenes.Popups
{
    using System.Collections.Generic;
    using DG.Tweening;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
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
    public class UITemplateRateGameScreenPresenter : BasePopupPresenter<UITemplateRateGameScreenView>
    {
        #region inject

        private readonly DiContainer    diContainer;
        private readonly IScreenManager screenManager;

        #endregion

        private static string storeUrl      = "https://play.google.com/store/apps/details?id=com.unity3d.player";
        private static int    lastStarCount = 0;
        public UITemplateRateGameScreenPresenter(SignalBus signalBus, DiContainer diContainer, IScreenManager screenManager) : base(signalBus)
        {
            this.diContainer   = diContainer;
            this.screenManager = screenManager;
        }
        public override void BindData()
        {
            lastStarCount = 0;
            for (int i = 0; i < this.View.StarImages.Count; i++)
            {
                var star = this.View.StarImages[i];
                star.transform.localScale = Vector3.zero;
            }
        }

        public override void Dispose() { base.Dispose(); }

        protected override async void OnViewReady()
        {
            base.OnViewReady();
            await this.OpenViewAsync();
            this.View.YesButton.onClick.AddListener(this.OnClickYes);
            this.View.LaterButton.onClick.AddListener(this.OnClickLater);
            for (int i = 0; i < this.View.StarButtons.Count; i++)
            {
                int closureIndex = i;
                var star         = this.View.StarButtons[closureIndex];
                star.onClick.AddListener(() => this.OnClickStar(closureIndex + 1));
            }
            this.playAnimation();
        }

        private void playAnimation() { this.yesButtonAnimation(); }

        private void OnClickStar(int count)
        {
            lastStarCount = count;
            for (int i = 0; i < count; i++)
                this.starAnimation(i, true);
            for (int i = count; i < this.View.StarButtons.Count; i++)
                this.starAnimation(i, false);
        }
        private void yesButtonAnimation()
        {
            this.View.YesButton.transform.localScale = Vector3.one;
            this.View.YesButton.transform.DOScale(Vector3.one * 1.1f, 1f).SetLoops(-1, loopType: LoopType.Yoyo).SetEase(Ease.Linear);
        }

        private async void starAnimation(int index, bool WillActive = true)
        {
            if (index >= this.View.StarButtons.Count || index < 0) return;

            var star        = this.View.StarImages[index];
            var targetScale = WillActive ? Vector3.one : Vector3.zero;
            var easeType    = WillActive ? Ease.OutElastic : Ease.OutCirc;
            var duration    = WillActive ? 0.5f : 0.3f;

            star.transform.DOScale(targetScale, duration).SetLoops(1, loopType: LoopType.Yoyo).SetEase(easeType);
        }


        private void OnClickLater() { this.screenManager.CloseCurrentScreen(); }

        private void OnClickYes()
        {
            if (lastStarCount == this.View.StarButtons.Count) Application.OpenURL(storeUrl);
            this.screenManager.CloseCurrentScreen();
        }
    }
}