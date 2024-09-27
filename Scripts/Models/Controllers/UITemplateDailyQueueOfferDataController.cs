#if THEONE_DAILY_QUEUE_REWARD
namespace TheOneStudio.UITemplate.UITemplate.Models.Controllers
{
    using System;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using UnityEngine;
    using UnityEngine.Scripting;

    public class UITemplateDailyQueueOfferDataController : IUITemplateControllerData
    {
        #region Inject

        private readonly IInternetService                   internetService;
        private readonly UITemplateDailyQueueOfferData      dailyQueueOfferData;
        private readonly UITemplateDailyQueueOfferBlueprint dailyQueueOfferBlueprint;
        private readonly UITemplateInventoryDataController  inventoryDataController;

#endregion

        [Preserve]
        public UITemplateDailyQueueOfferDataController
        (
            IInternetService internetService,
            UITemplateDailyQueueOfferData dailyQueueOfferData,
            UITemplateDailyQueueOfferBlueprint dailyQueueOfferBlueprint,
            UITemplateInventoryDataController inventoryDataController
        )
        {
            this.internetService           = internetService;
            this.dailyQueueOfferData       = dailyQueueOfferData;
            this.dailyQueueOfferBlueprint  = dailyQueueOfferBlueprint;
            this.inventoryDataController   = inventoryDataController;
        }

        public event Action OnUpdateOfferItem;

        public void CheckOfferStatus()
        {
            var isFirstTimeOpen = this.dailyQueueOfferData.OfferToStatusDuringDay.Count == 0;
            var isDifferentDay  = this.GetRemainTimeToNextDay().Days < 0;
            if (isDifferentDay || isFirstTimeOpen)
            {
                this.InitAllOfferStatus();
            }
        }

        public bool TryGetOfferStatusDuringDay(string id, out RewardStatus rewardStatus)
        {
            if (this.dailyQueueOfferData.OfferToStatusDuringDay.TryGetValue(id, out var status))
            {
                rewardStatus = status;
                return true;
            }

            rewardStatus = RewardStatus.Locked;
            return false;
        }

        private int GetCurrentDayIndex()
        {
            var lastOfferDate       = this.dailyQueueOfferData.LastOfferDate;
            var firstTimeOpenedDate = this.dailyQueueOfferData.FirstTimeOpen;
            var loginTime           = (int)(lastOfferDate - firstTimeOpenedDate).TotalDays;
            var lastTimeLoginToNow  = (int)(DateTime.Now  - lastOfferDate).TotalDays;
            if (lastTimeLoginToNow > 0)
            {
                loginTime++;
            }
            else
            {
                loginTime = 0;
            }

            return loginTime;
        }

        public UITemplateDailyQueueOfferRecord GetCurrentDailyQueueOfferRecord()
        {
            var dayIndex = this.GetCurrentDayIndex();
            if (dayIndex > this.dailyQueueOfferBlueprint.Count)
            {
                dayIndex %= this.dailyQueueOfferBlueprint.Count;
            }

            return this.dailyQueueOfferBlueprint.GetDataById(dayIndex <= 0 ? 1 : dayIndex);
        }

        public TimeSpan GetRemainTimeToNextDay() { return this.dailyQueueOfferData.LastOfferDate + TimeSpan.FromDays(1) - DateTime.Now; }

        public DateTime GetFirstTimeOpenedDate => this.dailyQueueOfferData.FirstTimeOpen;

        private void InitAllOfferStatus()
        {
            this.dailyQueueOfferData.OfferToStatusDuringDay.Clear();
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
            this.UnlockNextOffer();
            this.OnUpdateOfferItem?.Invoke();
            await this.inventoryDataController.AddGenericReward(offerItemRecord.ItemId, offerItemRecord.Value, claimRect, claimSoundKey);
        }

        private void UnlockNextOffer()
        {
            var nextOfferId = this.dailyQueueOfferData.OfferToStatusDuringDay
                .FirstOrDefault(keyPair => keyPair.Value == RewardStatus.Locked).Key;
            if (string.IsNullOrEmpty(nextOfferId))
                return;

            this.dailyQueueOfferData.OfferToStatusDuringDay[nextOfferId] = RewardStatus.Unlocked;
        }
    }
}
#endif