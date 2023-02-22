namespace TheOneStudio.UITemplate.UITemplate.Interfaces
{
    using System;

    public interface IAdsSystem
    {
        void ShowBanner();
        void HideBanner();
        void DestroyBanner();
        void ShowInterstitial(Action onComplete = null);
        void ShowRewardedVideo(Action onComplete = null);
    }
}