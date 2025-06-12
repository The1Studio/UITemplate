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
        public bool   IsShow;
        public Action OnHide;

        public ShowNativeCollapseSignal(bool isShow, Action onHide = null)
        {
            this.IsShow           = isShow;
            this.OnHide = onHide;
        }
    }
}