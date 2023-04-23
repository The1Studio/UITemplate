namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using TheOneStudio.UITemplate.UITemplate.Interfaces;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;

    public class UITemPlateVibrateServices : IVibrate
    {
        private readonly UITemplateSettingDataController uiTemplateSettingDataController;

        public UITemPlateVibrateServices(UITemplateSettingDataController uiTemplateSettingDataController)
        {
            this.uiTemplateSettingDataController = uiTemplateSettingDataController;
            Vibration.Init();
        }

        public void VibratePop()
        {
            if (this.uiTemplateSettingDataController.IsVibrationOn)
                Vibration.VibratePop();
        }

        public void VibratePeek()
        {
            if (this.uiTemplateSettingDataController.IsVibrationOn)
                Vibration.VibratePeek();
        }

        public void VibrateNope()
        {
            if (this.uiTemplateSettingDataController.IsVibrationOn)
                Vibration.VibrateNope();
        }

#if UNITY_ANDROID
        public void VibrateAndroid(long milliseconds)
        {
            if (this.uiTemplateSettingDataController.IsVibrationOn)
                Vibration.VibrateAndroid(milliseconds);
        }

        public void VibrateAndroid(long[] pattern, int repeat)
        {
            if (this.uiTemplateSettingDataController.IsVibrationOn)
                Vibration.VibrateAndroid(pattern, repeat);
        }

        public void CancelAndroid() { Vibration.CancelAndroid(); }
#endif

        public bool HasVibrator() { return Vibration.HasVibrator(); }

        public void Vibrate()
        {
            if (this.uiTemplateSettingDataController.IsVibrationOn)
                Vibration.Vibrate();
        }
    }
}