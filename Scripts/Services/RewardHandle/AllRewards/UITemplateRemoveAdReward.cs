namespace TheOneStudio.UITemplate.UITemplate.Services.RewardHandle.AllRewards
{
    using System.Collections.Generic;
    using Core.AdsServices;
    using UnityEngine;

    public class UITemplateRemoveAdReward : UITemplateBaseReward
    {
        public const string REWARD_ID = "remove_ads";
        
        private readonly List<IAdServices> adServices;
        public override  string            RewardId => REWARD_ID;

        public UITemplateRemoveAdReward(List<IAdServices> adServices) { this.adServices = adServices; }

        public override void ReceiveReward(int value, RectTransform startPosAnimation)
        {
            this.adServices.ForEach(adService => adService.RemoveAds());
        }
    }
}