namespace TheOneStudio.UITemplate.UITemplate.Services.RewardHandle.AllRewards
{
    using Core.AdsServices;
    using UnityEngine;

    public class UITemplateRemoveAdReward : UITemplateBaseReward
    {
        private readonly IAdServices                   adServices;
        public override  string                        RewardId => "remove_ads";

        public UITemplateRemoveAdReward(IAdServices adServices) : base()
        {
            this.adServices                    = adServices;
        }

        public override void ReceiveReward(int value, RectTransform startPosAnimation)
        {
            this.adServices.RemoveAds();
        }
    }
}