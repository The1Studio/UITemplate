namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using GameFoundation.Scripts.Utilities;

    public class UITemplateSoundServices
    {
        public virtual string KeySoundWin   => "victory";
        public virtual string KeySoundLose  => "lose";
        public virtual string KeySoundClick => "click_button";

        public virtual void PlaySoundWin()   => this.PlaySound(this.KeySoundWin);
        public virtual void PlaySoundLose()  => this.PlaySound(this.KeySoundLose);
        public virtual void PlaySoundClick() => this.PlaySound(this.KeySoundClick);

        public void PlaySound(string key) { AudioManager.Instance.PlaySound(key); }

        public void PlayMusic(string key) { AudioManager.Instance.PlayPlayList(key); }
    }
}