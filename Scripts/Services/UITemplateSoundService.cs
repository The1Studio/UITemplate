namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using System;
    using GameFoundation.Scripts.Utilities;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using UnityEngine;
    using Zenject;

    public class UITemplateSoundService
    {
        private readonly UITemplateSettingDataController settingDataController;

        public UITemplateSoundService(UITemplateSettingDataController settingDataController)
        {
            this.settingDataController = settingDataController;
        }

        protected virtual string KeySoundWin   => "victory";
        protected virtual string KeySoundLose  => "lose";
        protected virtual string KeySoundClick => "click_button";

        public virtual void PlaySoundWin()   => this.PlaySound(this.KeySoundWin);
        public virtual void PlaySoundLose()  => this.PlaySound(this.KeySoundLose);
        public virtual void PlaySoundClick() => this.PlaySound(this.KeySoundClick);

        public void PlaySound(string key)
        {
            if (this.settingDataController.IsSoundOn)
            {
                AudioManager.Instance.PlaySound(key);
            }
        }

        public void PlayMusic(string key)
        {
            if (this.settingDataController.IsMusicOn)
            {
                // TODO: play BGM playlist instead of single BGM
                AudioManager.Instance.PlaySound(key, true);
            }
        }

        public void Vibrate()
        {
            if (this.settingDataController.IsVibrationOn)
            {
                // TODO: I don't know handheld vibration API is work with any device or not, so I just comment it out
                Handheld.Vibrate();
            }
        }

        public static void BindTo<TSoundService>(DiContainer diContainer) where TSoundService : UITemplateSoundService
        {
            diContainer.Bind<TSoundService>().AsCached();
            diContainer.Rebind<UITemplateSoundService>().FromResolveGetter<TSoundService>(e => e).AsCached();
        }
    }
}