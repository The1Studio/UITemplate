namespace TheOneStudio.UITemplate.UITemplate.Services.RewardHandle.AllRewards
{
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    public class UITemplateRemoveAdRewardExecutorBase : UITemplateRewardExecutorBase
    {
        public const string REWARD_ID = "remove_ads";

        #region inject

        private readonly UITemplateAdServiceWrapper uiTemplateAdServiceWrapper;

        #endregion

        public override string RewardId => REWARD_ID;

        [Preserve]
        public UITemplateRemoveAdRewardExecutorBase(UITemplateAdServiceWrapper uiTemplateAdServiceWrapper) { this.uiTemplateAdServiceWrapper = uiTemplateAdServiceWrapper; }

        public override void ReceiveReward(int value, RectTransform startPosAnimation) { this.uiTemplateAdServiceWrapper.RemoveAds(); }
    }
}