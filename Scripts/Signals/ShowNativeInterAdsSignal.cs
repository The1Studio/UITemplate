namespace TheOneStudio.UITemplate.Scripts.Signals
{
    using System;

    public class ShowNativeInterAdsSignal
    {
        public string       InterPlacement;
        public Action<bool> OnComplete;

        public ShowNativeInterAdsSignal(string interPlacement, Action<bool> onComplete)
        {
            this.InterPlacement = interPlacement;
            this.OnComplete     = onComplete;
        }
    }
}