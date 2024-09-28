namespace TheOneStudio.UITemplate.UITemplate.Models
{
    using System;
    using GameFoundation.Scripts.Interfaces;
    using Sirenix.Serialization;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;
    using UnityEngine.Scripting;

    [Preserve]
    public class UITemplateAdsData : ILocalData, IUITemplateLocalData
    {
        [OdinSerialize] public int WatchedInterstitialAds { get; set; }

        [OdinSerialize] public int WatchedRewardedAds { get; set; }

        public void Init() { }

        public Type ControllerType => typeof(UITemplateAdsController);
    }
}