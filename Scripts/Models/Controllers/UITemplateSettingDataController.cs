namespace TheOneStudio.UITemplate.UITemplate.Models.Controllers
{
    using GameFoundation.Scripts.Models;

    public class UITemplateSettingDataController:IUITemplateControllerData
    {
        #region Inject

        private readonly UITemplateUserSettingData uiTemplateUserSettingData;
        private readonly SoundSetting              soundSetting;

        #endregion

        public bool IsSoundOn => this.soundSetting.SoundValue.Value > 0;

        public bool IsMusicOn     => this.soundSetting.MusicValue.Value > 0;
        public bool IsVibrationOn => this.uiTemplateUserSettingData.IsVibrationEnable;

        public bool IsFlashLightOn => this.uiTemplateUserSettingData.IsFlashLightEnable;

        public UITemplateSettingDataController(UITemplateUserSettingData uiTemplateUserSettingData, SoundSetting soundSetting)
        {
            this.uiTemplateUserSettingData = uiTemplateUserSettingData;
            this.soundSetting              = soundSetting;
        }

        public void SetSoundOnOff() { this.soundSetting.SoundValue.Value = this.IsSoundOn ? 0 : 1; }

        public void SetMusicOnOff() { this.soundSetting.MusicValue.Value = this.IsMusicOn ? 0 : 1; }
        
        public void SetSound (float value) { this.soundSetting.SoundValue.Value = value; }
        public void SetMusic (float value) { this.soundSetting.MusicValue.Value = value; }

        public void SetVibrationOnOff() { this.uiTemplateUserSettingData.IsVibrationEnable = !this.uiTemplateUserSettingData.IsVibrationEnable; }

        public void SetFlashLightOnOff() { this.uiTemplateUserSettingData.IsFlashLightEnable = !this.uiTemplateUserSettingData.IsFlashLightEnable; }
    }
}