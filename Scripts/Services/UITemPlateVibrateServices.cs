namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using TheOneStudio.UITemplate.UITemplate.Interfaces;
    using TheOneStudio.UITemplate.UITemplate.Models;

    public class UITemPlateVibrateServices : IVibrate
    {
        private readonly UITemplateUserSettingData uiTemplateUserSettingData;

        public UITemPlateVibrateServices(UITemplateUserSettingData uiTemplateUserSettingData) { this.uiTemplateUserSettingData = uiTemplateUserSettingData; }

        public void VibratePop()
        {
            if (this.uiTemplateUserSettingData.IsVibrationEnable)
                Vibration.VibratePop();
        }

        public void VibratePeek()
        {
            if (this.uiTemplateUserSettingData.IsVibrationEnable)
                Vibration.VibratePeek();
        }

        public void VibrateNope()
        {
            if (this.uiTemplateUserSettingData.IsVibrationEnable)
                Vibration.VibrateNope();
        }

        public void VibrateAndroid(long milliseconds)
        {
            if (this.uiTemplateUserSettingData.IsVibrationEnable)
                Vibration.VibrateAndroid(milliseconds);
        }

        public void VibrateAndroid(long[] pattern, int repeat)
        {
            if (this.uiTemplateUserSettingData.IsVibrationEnable)
                Vibration.VibrateAndroid(pattern, repeat);
        }

        public void CancelAndroid() { Vibration.CancelAndroid(); }

        public bool HasVibrator() { return Vibration.HasVibrator(); }

        public void Vibrate()
        {
            if (this.uiTemplateUserSettingData.IsVibrationEnable)
                Vibration.Vibrate();
        }
    }
}