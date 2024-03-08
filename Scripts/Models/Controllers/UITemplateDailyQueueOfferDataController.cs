namespace TheOneStudio.UITemplate.UITemplate.Models.Controllers
{
    using System;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using UnityEngine;

    public class UITemplateDailyQueueOfferDataController : IUITemplateControllerData
    {
        #region Inject

        private readonly IInternetService                    internetService;
        private readonly UITemplateDailyQueueOfferData       dailyQueueOfferData;
        private readonly UITemplateDailyQueueOfferBlueprint  dailyQueueOfferBlueprint;
        private readonly UITemplateInventoryDataController   inventoryDataController;
        private readonly UITemplateFlyingAnimationController flyingAnimationController;
        private readonly UITemplateDailyRewardController     dailyRewardController;

        #endregion

        public UITemplateDailyQueueOfferDataController
        (
            IInternetService                    internetService,
            UITemplateDailyQueueOfferData       dailyQueueOfferData,
            UITemplateDailyQueueOfferBlueprint  dailyQueueOfferBlueprint,
            UITemplateInventoryDataController   inventoryDataController,
            UITemplateFlyingAnimationController flyingAnimationController,
            UITemplateDailyRewardController     dailyRewardController
        )
        {
            this.internetService           = internetService;
            this.dailyQueueOfferData       = dailyQueueOfferData;
            this.dailyQueueOfferBlueprint  = dailyQueueOfferBlueprint;
            this.inventoryDataController   = inventoryDataController;
            this.flyingAnimationController = flyingAnimationController;
            this.dailyRewardController     = dailyRewardController;
        }

        public void CheckOfferStatus()
        {
            var isDifferentDay = this.GetRemainTimeToNextDay().Days > 0;
            if (isDifferentDay)
            {
                this.InitAllOfferStatus();
            }
        }

        private int GetCurrentDayIndex()
        {
            var firstTimeOpenedDate = this.dailyRewardController.GetFirstTimeOpenedDate;
            return (DateTime.Now - firstTimeOpenedDate).Days;
        }

        public UITemplateDailyQueueOfferRecord GetCurrentDailyQueueOfferRecord() { return this.dailyQueueOfferBlueprint.GetDataById(this.GetCurrentDayIndex()); }

        private TimeSpan GetRemainTimeToNextDay() { return DateTime.Now - this.dailyQueueOfferData.LastOfferDate; }

        public DateTime GetFirstTimeOpenedDate => this.dailyRewardController.GetFirstTimeOpenedDate;

        private void InitAllOfferStatus()
        {
            foreach (var (_, offerItemRecord) in this.GetCurrentDailyQueueOfferRecord().OfferItems)
            {
                var offerStatus = offerItemRecord.IsRewardedAds ? RewardStatus.Locked : RewardStatus.Unlocked;
                this.dailyQueueOfferData.OfferToStatusDuringDay.Add(offerItemRecord.OfferId, offerStatus);
            }

            this.dailyQueueOfferData.LastOfferDate = DateTime.Now;
        }

        public async UniTask ClaimOfferItem(UITemplateDailyQueueOfferItemRecord offerItemRecord, RectTransform claimRect = null, string claimSoundKey = null)
        {
            if (!this.dailyQueueOfferData.OfferToStatusDuringDay.TryGetValue(offerItemRecord.OfferId, out var status))
                return;

            if (status == RewardStatus.Claimed)
                return;

            this.dailyQueueOfferData.OfferToStatusDuringDay[offerItemRecord.OfferId] = RewardStatus.Claimed;
            this.UnlockNextOffer(offerItemRecord.OfferId);
            await this.inventoryDataController.AddGenericReward(offerItemRecord.ItemId, offerItemRecord.Value, claimRect, claimSoundKey);
        }

        private void UnlockNextOffer(string currentOfferId)
        {
            if (!this.GetCurrentDailyQueueOfferRecord().OfferItems[currentOfferId].IsRewardedAds)
                return;

            var nextOfferId = this.dailyQueueOfferData.OfferToStatusDuringDay
                .FirstOrDefault(keyPair => keyPair.Value == RewardStatus.Locked).Key;
            if (string.IsNullOrEmpty(nextOfferId))
                return;

            this.dailyQueueOfferData.OfferToStatusDuringDay[nextOfferId] = RewardStatus.Unlocked;
        }
    }
}