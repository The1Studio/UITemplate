namespace TheOneStudio.UITemplate.UITemplate.Models.Controllers
{
    using System;
    using GameFoundation.Scripts.Models;
    using UnityEngine.Scripting;

    public class UITemplateSettingDataController : IUITemplateControllerData
    {
        #region Inject

        private readonly UITemplateUserSettingData uiTemplateUserSettingData;
        private readonly SoundSetting              soundSetting;

        #endregion

        public bool IsSoundOn => this.soundSetting.SoundValue.Value > 0;

        public bool IsMusicOn     => this.soundSetting.MusicValue.Value > 0;
        public bool IsVibrationOn => this.uiTemplateUserSettingData.IsVibrationEnable;

        public bool IsFlashLightOn => this.uiTemplateUserSettingData.IsFlashLightEnable;

        public float MusicValue => this.soundSetting.MusicValue.Value;

        public float SoundValue => this.soundSetting.SoundValue.Value;

        [Preserve]
        public UITemplateSettingDataController(UITemplateUserSettingData uiTemplateUserSettingData, SoundSetting soundSetting)
        {
            this.uiTemplateUserSettingData = uiTemplateUserSettingData;
            this.soundSetting              = soundSetting;
        }

        public void SetSoundOnOff() { this.soundSetting.SoundValue.Value = this.IsSoundOn ? 0 : 1; }

        public void SetMusicOnOff() { this.soundSetting.MusicValue.Value = this.IsMusicOn ? 0 : 1; }

        public void SetMusicValue(float value) { this.soundSetting.MusicValue.Value = Math.Clamp(value, 0, 1); }

        public void SetSoundValue(float value) { this.soundSetting.SoundValue.Value = Math.Clamp(value, 0, 1); }

        public void SetVibrationOnOff() { this.uiTemplateUserSettingData.IsVibrationEnable = !this.uiTemplateUserSettingData.IsVibrationEnable; }

        public void SetFlashLightOnOff() { this.uiTemplateUserSettingData.IsFlashLightEnable = !this.uiTemplateUserSettingData.IsFlashLightEnable; }
    }
}