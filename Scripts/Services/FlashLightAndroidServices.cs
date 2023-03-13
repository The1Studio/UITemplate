namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using System;
    using Cysharp.Threading.Tasks;
    using TheOneStudio.UITemplate.UITemplate.Interfaces;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using UnityEngine;
    #if UNITY_ANDROID &&!UNITY_EDITOR

    public class FlashlightPluginAndroid : IFlashLight
    {
        private readonly UITemplateSettingDataController uiTemplateSettingDataController;
        private readonly AndroidJavaClass                javaObject;

        public FlashlightPluginAndroid(UITemplateSettingDataController uiTemplateSettingDataController)
        {
            this.uiTemplateSettingDataController = uiTemplateSettingDataController;
            this.javaObject                      = new AndroidJavaClass("com.myflashlight.flashlightlib.Flashlight");
        }

        public void TurnOn()
        {
            if (this.uiTemplateSettingDataController.IsFlashLightOn)
                this.javaObject.CallStatic("on", this.GetUnityActivity());
        }

        public void TurnOff() { this.javaObject.CallStatic("off", this.GetUnityActivity()); }

        public async void AutoOnOff(float time)
        {
            this.TurnOn();
            await UniTask.Delay(TimeSpan.FromSeconds(time));
            this.TurnOff();
        }

        private AndroidJavaObject GetUnityActivity()
        {
            using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");

            return unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        }
    }
#endif

#if UNITY_IOS
    public class FlashLightPluginIOS : IFlashLight
    {
       public void TurnOn() {  }

        public void TurnOff() {  }

       public async void AutoOnOff(float time = 0.1f)
        {
            this.TurnOn();
            await UniTask.Delay(TimeSpan.FromSeconds(time));
            this.TurnOff();
        }
    }

#endif

#if UNITY_EDITOR

    public class FlashLightEditor : IFlashLight
    {
        public void TurnOn() { }

        public void TurnOff() { }

        public async void AutoOnOff(float time = 0.1f)
        {
            this.TurnOn();
            await UniTask.Delay(TimeSpan.FromSeconds(time));
            this.TurnOff();
        }
    }

#endif
}