namespace UITemplate.Scripts.Scenes.Play.End
{
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using UnityEngine.UI;
    using Zenject;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using UITemplate.Scripts.Scenes.Main;
    using UITemplate.Scripts.Scenes.Popups;
    using UnityEngine;

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
    public class UITemplateLoseScreenPresenter : BaseScreenPresenter<UITemplateLoseScreenView>
    {
        #region inject

        private readonly DiContainer    diContainer;
        private readonly IScreenManager screenManager;

        #endregion

        public UITemplateLoseScreenPresenter(SignalBus signalBus, DiContainer diContainer, IScreenManager screenManager) : base(signalBus)
        {
            this.diContainer   = diContainer;
            this.screenManager = screenManager;
        }

        private static bool isAdsAvailable = true;
        public override void BindData()
        {
            this.View.AdsNotification.gameObject.transform.localScale = Vector3.zero;
            this.adsNotificationAnimation(isAdsAvailable = this.checkAdsAvailable());
            this.View.NextEndgameButton.SetOnOff(isOn: false);
        }

        public override void Dispose() { base.Dispose(); }

        protected override async void OnViewReady()
        {
            base.OnViewReady();
            await this.OpenViewAsync();

            this.View.HomeButton.onClick.AddListener(this.OnClickHome);
            this.View.ReplayButton.onClick.AddListener(this.OnClickReplay);
            this.View.NextEndgameButton.Button.onClick.AddListener(this.OnClickNextEndgame);
        }

        private void OnClickHome() { this.screenManager.OpenScreen<UITemplateHomeSimpleScreenPresenter>(); }

        private void OnClickReplay()
        {
            // Reuse WinScreen
        }

        private async void OnClickNextEndgame()
        {
            if (isAdsAvailable)
            {
                //show ads 
                if (this.hasWatchedAds())
                {
                    this.adsNotificationAnimation(false);
                    await UniTask.WaitUntil(() => !isAdsAvailable);
                    this.View.NextEndgameButton.SetOnOff(isOn: true);
                    // Reuse WinScreen
                }
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

        private async void adsNotificationAnimation(bool willActive)
        {
            var adsNotification = this.View.AdsNotification.gameObject;
            var targetScale     = willActive ? Vector3.one : Vector3.zero;
            var easeType        = willActive ? Ease.OutElastic : Ease.InElastic;
            var duration        = willActive ? 0.5f : 0.3f;
            
            await adsNotification.transform.DOScale(targetScale, duration).SetEase(easeType).AsyncWaitForCompletion();
            isAdsAvailable = willActive;
        }
    }
}