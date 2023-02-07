namespace UITemplate.Scripts.Models
{
    public class UITemplateSettingData
    {
        public float SoundEffectVolume { get; private set; } = 1;
        public float MusicVolume       { get; private set; } = 1;
        public bool  IsVibrationEnable = true;

        public void SetSoundOnOff() { this.SoundEffectVolume = this.IsSoundOn ? 0 : 1; }

        public void SetMusicOnOff() { this.MusicVolume = this.IsMusicOn ? 0 : 1; }

        public void SetVibrationOnOff() { this.IsVibrationEnable = !this.IsVibrationEnable; }

        public bool IsSoundOn => this.SoundEffectVolume > 0;

        public bool IsMusicOn => this.MusicVolume > 0;
    }
}