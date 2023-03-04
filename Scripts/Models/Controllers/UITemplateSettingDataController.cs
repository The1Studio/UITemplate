namespace TheOneStudio.UITemplate.UITemplate.Models.Controllers
{
    using GameFoundation.Scripts.Utilities;

    public class UITemplateSettingDataController
    {
        #region Inject

        private readonly  UITemplateUserSettingData uiTemplateUserSettingData;
        private readonly IAudioManager             audioManager;

        #endregion

        public UITemplateSettingDataController(UITemplateUserSettingData uiTemplateUserSettingData, IAudioManager audioManager)
        {
            this.uiTemplateUserSettingData = uiTemplateUserSettingData;
            this.audioManager              = audioManager;
        }

        public void SetSoundOnOff()
        {
            this.uiTemplateUserSettingData.SoundValue.Value = this.IsSoundOn ? 0 : 1; 
            this.audioManager.CheckToMuteSound(!this.IsSoundOn);
        }

        public void SetMusicOnOff()
        {
            this.uiTemplateUserSettingData.MusicValue.Value = this.IsMusicOn ? 0 : 1; 
            this.audioManager.CheckToMuteMusic(!this.IsMusicOn);
        }

        public void SetVibrationOnOff() { this.uiTemplateUserSettingData.IsVibrationEnable = !this.uiTemplateUserSettingData.IsVibrationEnable; }
        
        public bool IsSoundOn => this.uiTemplateUserSettingData.SoundValue.Value > 0;

        public bool IsMusicOn     => this.uiTemplateUserSettingData.MusicValue.Value > 0;
        public bool IsVibrationOn => this.uiTemplateUserSettingData.IsVibrationEnable;
    }
}