namespace TheOneStudio.UITemplate.UITemplate.Interfaces
{
    public interface IVibrate
    {
        void VibratePop();
        void VibratePeek();
        void VibrateNope();
        void VibrateAndroid(long milliseconds);
        void VibrateAndroid ( long[] pattern, int repeat );
        void CancelAndroid();
        bool HasVibrator();
        void Vibrate();
    }
}