namespace TheOneStudio.UITemplate.UITemplate.Models.Controllers
{
    using GameFoundation.Scripts.Utilities;

    public class UITemplateSettingDataController
    {
        #region Inject

        private readonly  UITemplateUserSettingData uiTemplateUserSettingData;

        #endregion

        public UITemplateSettingDataController(UITemplateUserSettingData uiTemplateUserSettingData)
        {
            this.uiTemplateUserSettingData = uiTemplateUserSettingData;
        }

        public void SetSoundOnOff()
        {
            this.uiTemplateUserSettingData.SoundValue.Value = this.IsSoundOn ? 0 : 1; 
        }

        public void SetMusicOnOff()
        {
            this.uiTemplateUserSettingData.MusicValue.Value = this.IsMusicOn ? 0 : 1; 
        }

        public void SetVibrationOnOff() { this.uiTemplateUserSettingData.IsVibrationEnable = !this.uiTemplateUserSettingData.IsVibrationEnable; }
        
        public bool IsSoundOn => this.uiTemplateUserSettingData.SoundValue.Value > 0;

        public bool IsMusicOn     => this.uiTemplateUserSettingData.MusicValue.Value > 0;
        public bool IsVibrationOn => this.uiTemplateUserSettingData.IsVibrationEnable;
    }
}