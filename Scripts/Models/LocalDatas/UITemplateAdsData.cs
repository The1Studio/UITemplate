namespace TheOneStudio.UITemplate.UITemplate.Models
{
    using System;
    using System.Collections.Generic;
    using GameFoundation.Scripts.Interfaces;
    using Sirenix.Serialization;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;
    using UnityEngine.Scripting;

    [Preserve]
    public class UITemplateAdsData : ILocalData, IUITemplateLocalData
    {
        [OdinSerialize] internal int          WatchedInterstitialAds         { get; set; }
        [OdinSerialize] internal int          WatchedRewardedAds             { get; set; }
        [OdinSerialize] internal List<double> InterstitialAndRewardedRevenue { get; set; } = new();

        public void Init() { }

        public Type ControllerType => typeof(UITemplateAdsController);
    }
}