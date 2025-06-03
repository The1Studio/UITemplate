namespace TheOneStudio.UITemplate.Scripts.Signals
{
    using System;

    public class ShowNativeInterAdsSignal
    {
        public Action<bool> OnComplete;

        public ShowNativeInterAdsSignal(Action<bool> onComplete)
        {
            this.OnComplete     = onComplete;
        }
    }
}