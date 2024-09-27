namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using GameFoundation.Scripts.Utilities;
    using UnityEngine.Scripting;

    [Preserve]
    public class UITemplateSoundServices
    {
        public virtual string KeySoundWin   => "victory";
        public virtual string KeySoundLose  => "lose";
        public virtual string KeySoundClick => "click_button";

        public string CurrentPlayList { get; private set; }

        public virtual void PlaySoundWin() => this.PlaySound(this.KeySoundWin);

        public virtual void PlaySoundLose() => this.PlaySound(this.KeySoundLose);

        public virtual void PlaySoundClick() => this.PlaySound(this.KeySoundClick);

        public void PlaySound(string key) { AudioService.Instance.PlaySound(key); }

        public void StopSound(string key) { AudioService.Instance.StopAllSound(); }

        public void PlayMusic(string playList)
        {
            AudioService.Instance.PlayPlayList(playList);
            this.CurrentPlayList = playList;
        }
    }
}