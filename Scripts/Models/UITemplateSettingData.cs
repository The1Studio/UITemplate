namespace TheOneStudio.UITemplate.UITemplate.Models
{
    using GameFoundation.Scripts.Models;

    public class UITemplateUserSettingData : SoundSetting
    {
        public bool IsVibrationEnable = true;

        public void SetSoundOnOff() { this.SoundValue.Value = this.IsSoundOn ? 0 : 1; }

        public void SetMusicOnOff() { this.MusicValue.Value = this.IsMusicOn ? 0 : 1; }

        public void SetVibrationOnOff() { this.IsVibrationEnable = !this.IsVibrationEnable; }

        public bool IsSoundOn => this.SoundValue.Value > 0;

        public bool IsMusicOn => this.MusicValue.Value > 0;
    }
}