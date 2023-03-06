namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using GameFoundation.Scripts.Utilities;

    public class UITemplateSoundService
    {
        protected virtual string KeySoundWin   => "victory";
        protected virtual string KeySoundLose  => "lose";
        protected virtual string KeySoundClick => "click_button";

        public virtual void PlaySoundWin()   => PlaySound(this.KeySoundWin);
        public virtual void PlaySoundLose()  => PlaySound(this.KeySoundLose);
        public virtual void PlaySoundClick() => PlaySound(this.KeySoundClick);

        public static void PlaySound(string key) => AudioManager.Instance.PlaySound(key);
    }
}