namespace TheOneStudio.UITemplate.UITemplate.Services.RewardHandle.AllRewards
{
    using Core.AdsServices;
    using GameFoundation.Scripts.Utilities.LogService;
    using UnityEngine;

    public class UITemplateRemoveAdReward : UITemplateBaseReward
    {
        private readonly IAdServices                   adServices;
        private readonly UITemplateGetRealRewardHelper uiTemplateGetRealRewardHelper;
        public override  string                        RewardId => "remove_ads";

        public UITemplateRemoveAdReward(ILogService logger, IAdServices adServices, UITemplateGetRealRewardHelper uiTemplateGetRealRewardHelper) : base(logger, uiTemplateGetRealRewardHelper)
        {
            this.adServices                    = adServices;
            this.uiTemplateGetRealRewardHelper = uiTemplateGetRealRewardHelper;
        }

        public override void ReceiveReward(string value, RectTransform startPosAnimation)
        {
            this.adServices.RemoveAds();
            this.AfterReceiveReward(this.RewardId);
        }
    }
}