namespace TheOneStudio.UITemplate.UITemplate.Scenes.Play.End
{
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateLoseScreenView : BaseView
    {
        public Button                 HomeButton;
        public Button                 ReplayButton;
        public UITemplateOnOffButton  NextEndgameButton;
        public UITemplateCurrencyView CurrencyView;
        public Image                  AdsNotification;
        public GameObject             Character;
    }

    [ScreenInfo(nameof(UITemplateLoseScreenView))]
    public class UITemplateLoseScreenPresenter : UITemplateBaseScreenPresenter<UITemplateLoseScreenView>
    {
        private static bool isAdsAvailable = true;

        public UITemplateLoseScreenPresenter(SignalBus signalBus, DiContainer diContainer, IScreenManager screenManager) : base(signalBus)
        {
            this.diContainer   = diContainer;
            this.screenManager = screenManager;
        }

        public override void BindData()
        {
            this.View.AdsNotification.gameObject.transform.localScale = Vector3.zero;
            this.adsNotificationAnimation(isAdsAvailable = this.checkAdsAvailable());
            this.View.NextEndgameButton.SetOnOff(false);
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        protected override async void OnViewReady()
        {
            base.OnViewReady();
            await this.OpenViewAsync();

            this.View.HomeButton.onClick.AddListener(this.OnClickHome);
            this.View.ReplayButton.onClick.AddListener(this.OnClickReplay);
            this.View.NextEndgameButton.Button.onClick.AddListener(this.OnClickNextEndgame);
        }

        protected virtual void OnClickHome()
        {
            this.screenManager.OpenScreen<UITemplateHomeSimpleScreenPresenter>();
        }

        protected virtual void OnClickReplay()
        {
            // Reuse WinScreen
        }

        private async void OnClickNextEndgame()
        {
            if (isAdsAvailable)
                //show ads 
                if (this.hasWatchedAds())
                {
                    this.adsNotificationAnimation(false);
                    await UniTask.WaitUntil(() => !isAdsAvailable);
                    this.View.NextEndgameButton.SetOnOff(true);
                    // Reuse WinScreen
                }
        }

        private bool checkAdsAvailable()
        {
            // Do something
            return true;
        }

        private bool hasWatchedAds()
        {
            // Do something
            return true;
        }

        private void adsNotificationAnimation(bool willActive)
        {
            var adsNotification = this.View.AdsNotification.gameObject;
            var targetScale     = willActive ? Vector3.one : Vector3.zero;
            var easeType        = willActive ? Ease.OutElastic : Ease.InElastic;
            var duration        = willActive ? 0.5f : 0.3f;

            adsNotification.transform.DOScale(targetScale, duration).SetEase(easeType).OnComplete(() => isAdsAvailable = willActive);
        }

        #region inject

        private readonly DiContainer    diContainer;
        private readonly IScreenManager screenManager;

        #endregion
    }
}