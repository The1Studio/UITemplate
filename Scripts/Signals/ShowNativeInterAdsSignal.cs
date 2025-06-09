namespace TheOneStudio.UITemplate.Scripts.Signals
{
    using System;

    public class ShowNativeInterAdsSignal
    {
        public Action<bool> OnComplete;

        public ShowNativeInterAdsSignal(Action<bool> onComplete)
        {
            this.OnComplete = onComplete;
        }
    }

    public class ShowNativeCollapseSignal
    {
        public bool IsShow;

        public ShowNativeCollapseSignal(bool isShow)
        {
            this.IsShow = isShow;
        }
    }
}