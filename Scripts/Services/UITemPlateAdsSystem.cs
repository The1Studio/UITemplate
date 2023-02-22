namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using System;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.Utilities.LogService;
    using TheOneStudio.UITemplate.UITemplate.Interfaces;

    public class UITemPlateAdsSystem : IAdsSystem
    {
        private readonly ILogService logger;

        public UITemPlateAdsSystem(ILogService logger) { this.logger = logger; }

        public void ShowBanner() { }

        public void HideBanner() { }

        public void DestroyBanner() { }

        public async void ShowInterstitial(Action onComplete = null)
        {
            this.logger.Log($"UITemplate ads system: ShowInterstitial Auto complete after 1 second");
            await UniTask.Delay(1000);
            onComplete?.Invoke();
        }

        public async void ShowRewardedVideo(Action onComplete = null)
        {
            this.logger.Log($"UITemplate ads system: ShowRewardedVideo Auto complete after 1 second");
            await UniTask.Delay(1000);
            onComplete?.Invoke();
        }
    }
}