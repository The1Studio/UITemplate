namespace TheOneStudio.UITemplate.UITemplate.Services.RewardHandle.AllRewards
{
    using Core.AdsServices;
    using GameFoundation.Scripts.Utilities.LogService;

    public class UITemplateRemoveAdReward : UITemplateBaseReward
    {
        private readonly IAdServices adServices;
        public override  string      RewardId => "remove_ads";

        public UITemplateRemoveAdReward(ILogService logger, IAdServices adServices) : base(logger) { this.adServices = adServices; }

        public override void ReceiveReward(string value,string addressableFlyingItem) { this.adServices.RemoveAds(); }
    }
}