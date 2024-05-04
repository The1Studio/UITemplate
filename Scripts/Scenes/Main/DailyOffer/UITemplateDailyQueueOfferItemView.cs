#if THEONE_DAILY_QUEUE_REWARD
namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main.DailyOffer
{
    using System;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.MVP;
    using R3;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UITemplateDailyQueueOfferItemView : TViewMono
    {
        [SerializeField] private TMP_Text            buttonText;
        [SerializeField] private TMP_Text            itemText;
        [SerializeField] private Image               itemImage;
        [SerializeField] private GameObject          adsIconObj;
        [SerializeField] private GameObject          disableClaimObj;
        [SerializeField] private Button              claimButton;
        [SerializeField] private UITemplateAdsButton adsClaimButton;

        public TMP_Text            ButtonText      => this.buttonText;
        public TMP_Text            ItemText        => this.itemText;
        public Image               ItemImage       => this.itemImage;
        public GameObject          AdsIconObj      => this.adsIconObj;
        public GameObject          DisableClaimObj => this.disableClaimObj;
        public Button              ClaimButton     => this.claimButton;
        public UITemplateAdsButton AdsClaimButton  => this.adsClaimButton;

        public Action ClickClaim;
        public Action ClickAdsClaim;

        private void Awake()
        {
            this.claimButton.onClick.AddListener(() => this.ClickClaim?.Invoke());
            this.adsClaimButton.onClick.AddListener(() => this.ClickAdsClaim?.Invoke());
        }
    }

    public class UITemplateDailyQueueOfferItemModel
    {
        public string OfferId { get; }

        public UITemplateDailyQueueOfferItemModel(string offerId) { this.OfferId = offerId; }
    }

    public class UITemplateDailyQueueOfferItemPresenter : BaseUIItemPresenter<UITemplateDailyQueueOfferItemView, UITemplateDailyQueueOfferItemModel>
    {
        #region Inject

        private readonly UITemplateAdServiceWrapper              adServiceWrapper;
        private readonly UITemplateDailyQueueOfferDataController dailyQueueOfferDataController;

        #endregion

        private RectTransform                       claimRect;
        private IDisposable                         remainTimeDisposable;
        private UITemplateDailyQueueOfferItemModel  model;
        private UITemplateDailyQueueOfferItemRecord dailyQueueOfferItemRecord;

        private const string AdsPlacement = "Daily_Offer";

        public UITemplateDailyQueueOfferItemPresenter
        (
            IGameAssets                             gameAssets,
            UITemplateAdServiceWrapper              adServiceWrapper,
            UITemplateDailyQueueOfferDataController dailyQueueOfferDataController
        )
            : base(gameAssets)
        {
            this.adServiceWrapper              = adServiceWrapper;
            this.dailyQueueOfferDataController = dailyQueueOfferDataController;
        }

        public override void OnViewReady()
        {
            base.OnViewReady();
            this.claimRect = this.View.ItemImage.GetComponent<RectTransform>();
            this.View.AdsClaimButton.OnViewReady(this.adServiceWrapper);
        }

        public override void BindData(UITemplateDailyQueueOfferItemModel param)
        {
            this.View.ClickClaim    = this.OnClickClaim;
            this.View.ClickAdsClaim = this.OnClickAdsClaim;

            this.model                     = param;
            this.dailyQueueOfferItemRecord = this.dailyQueueOfferDataController.GetCurrentDailyQueueOfferRecord().OfferItems[this.model.OfferId];
            this.View.ItemText.text        = $"x{this.dailyQueueOfferItemRecord.Value}";
            this.View.ItemImage.sprite     = this.GameAssets.ForceLoadAsset<Sprite>(this.dailyQueueOfferItemRecord.ImageId);
            this.OnUpdateOfferItemByStatus();
            this.BindDataToClaimButton();
            this.BindDataToAdsClaimButton();
        }

        private void BindDataToClaimButton()
        {
            if (this.remainTimeDisposable != null) return;

            this.remainTimeDisposable = Observable.EveryUpdate().Subscribe(_ => this.OnUpdateRemainTimeFreeOffer());
        }

        private void BindDataToAdsClaimButton()
        {
            this.dailyQueueOfferDataController.OnUpdateOfferItem += this.OnUpdateOfferItemByStatus;
            this.View.AdsClaimButton.BindData(AdsPlacement);
        }

        public override void Dispose()
        {
            base.Dispose();
            this.dailyQueueOfferDataController.OnUpdateOfferItem -= this.OnUpdateOfferItemByStatus;

            if (this.remainTimeDisposable == null) return;

            this.remainTimeDisposable?.Dispose();
            this.remainTimeDisposable = null;
        }

        private void OnUpdateOfferItemByStatus()
        {
            if (!this.dailyQueueOfferDataController.TryGetOfferStatusDuringDay(this.model.OfferId, out var status))
            {
                return;
            }

            var isRewardedAds = this.dailyQueueOfferItemRecord.IsRewardedAds;
            this.View.ClaimButton.gameObject.SetActive(status == RewardStatus.Unlocked && !isRewardedAds);
            this.View.AdsClaimButton.gameObject.SetActive(status == RewardStatus.Unlocked && isRewardedAds);
            this.View.AdsIconObj.SetActive(status != RewardStatus.Claimed && isRewardedAds);
            this.View.DisableClaimObj.SetActive(status != RewardStatus.Unlocked);
        }

        private void OnUpdateRemainTimeFreeOffer()
        {
            if (!this.dailyQueueOfferDataController.TryGetOfferStatusDuringDay(this.model.OfferId, out var status))
            {
                return;
            }

            var remainTimeToNextDay = this.dailyQueueOfferDataController.GetRemainTimeToNextDay();
            var remainHours         = remainTimeToNextDay.Hours;
            var remainMinutes       = remainTimeToNextDay.Minutes;
            var remainSeconds       = remainTimeToNextDay.Seconds;
            if (status == RewardStatus.Claimed)
            {
                var textRemainHours   = remainHours > 0 ? $"{remainHours}h " : "";
                var textRemainMinutes = $"{remainMinutes}m";
                var textRemainSeconds = remainHours > 0 ? "" : $"{remainSeconds}s";
                this.View.ButtonText.text = $"{textRemainHours}{textRemainMinutes}{textRemainSeconds}";
            }
            else
            {
                this.View.ButtonText.text = "Claim";
            }
        }

        protected void OnClickClaim() { this.OnClaimOfferSucceed(); }

        protected void OnClickAdsClaim() { this.adServiceWrapper.ShowRewardedAd(AdsPlacement, this.OnClaimOfferSucceed, this.OnClaimOfferFailed); }

        protected virtual void OnClaimOfferSucceed()
        {
            var offerData = this.dailyQueueOfferDataController.GetCurrentDailyQueueOfferRecord().OfferItems[this.model.OfferId];
            this.dailyQueueOfferDataController.ClaimOfferItem(offerData, this.claimRect);
        }

        protected virtual void OnClaimOfferFailed() { }
    }
}
#endif