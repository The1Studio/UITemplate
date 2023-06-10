namespace TheOneStudio.UITemplate.UITemplate.Models.Controllers
{
    using GameFoundation.Scripts.Models;

    public class UITemplateSettingDataController:IUITemplateControllerData
    {
        #region Inject

        private readonly UITemplateUserSettingData uiTemplateUserSettingData;
        private readonly SoundSetting              soundSetting;

        #endregion

        public bool IsSoundOn => this.soundSetting.SoundVolume > 0;

        public bool IsMusicOn     => this.soundSetting.MusicVolume > 0;
        public bool IsVibrationOn => this.uiTemplateUserSettingData.IsVibrationEnable;

        public bool IsFlashLightOn => this.uiTemplateUserSettingData.IsFlashLightEnable;

        public UITemplateSettingDataController(UITemplateUserSettingData uiTemplateUserSettingData, SoundSetting soundSetting)
        {
            this.uiTemplateUserSettingData = uiTemplateUserSettingData;
            this.soundSetting              = soundSetting;
        }

        public void SetSoundOnOff() { this.soundSetting.SoundVolume = this.IsSoundOn ? 0 : 1; }

        public void SetMusicOnOff() { this.soundSetting.MusicVolume = this.IsMusicOn ? 0 : 1; }

        public void SetVibrationOnOff() { this.uiTemplateUserSettingData.IsVibrationEnable = !this.uiTemplateUserSettingData.IsVibrationEnable; }

        public void SetFlashLightOnOff() { this.uiTemplateUserSettingData.IsFlashLightEnable = !this.uiTemplateUserSettingData.IsFlashLightEnable; }
    }
}