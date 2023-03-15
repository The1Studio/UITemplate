namespace TheOneStudio.UITemplate.UITemplate.Interfaces
{
    public interface IVibrate
    {
        void VibratePop();
        void VibratePeek();
        void VibrateNope();
#if UNITY_ANDROID
        void VibrateAndroid(long milliseconds);
        void VibrateAndroid ( long[] pattern, int repeat );
        void CancelAndroid();
#endif
        bool HasVibrator();
        void Vibrate();
    }
}