namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main.DailyReward
{
    using System;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.MVP;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UITemplateDailyRewardItemModel
    {
        public Action<UITemplateDailyRewardItemPresenter> OnClick           { get; set; }
        public RewardStatus                               RewardStatus      { get; set; }
        public UITemplateDailyRewardRecord                DailyRewardRecord { get; set; }
        public bool                                       IsGetWithAds      { get; set; }

        public UITemplateDailyRewardItemModel(UITemplateDailyRewardRecord dailyRewardRecord, RewardStatus rewardStatus, Action<UITemplateDailyRewardItemPresenter> click)
        {
            this.DailyRewardRecord = dailyRewardRecord;
            this.RewardStatus      = rewardStatus;
            this.OnClick           = click;
        }
    }

    public class UITemplateDailyRewardItemView : TViewMono
    {
        public Button          btnClaim;
        public Image           imgBackground;
        public Sprite          sprBgNormal;
        public Sprite          sprBgCurrentDay;
        public TextMeshProUGUI txtDayLabel;

        [Header("Reward")] public Image           imgReward;
        public                    TextMeshProUGUI txtValue;

        [Header("Lock")] public GameObject objLockReward;

        [Header("Claimed")] public GameObject objClaimed;
        public                     GameObject objClaimedCheckIcon;

        [Header("Ads")] public GameObject objClaimByAds;

        public Action OnClickClaimButton { get; set; }

        private void Awake()
        {
            if (this.btnClaim != null)
            {
                this.btnClaim.onClick.AddListener(() => { this.OnClickClaimButton?.Invoke(); });
            }
        }

        public void UpdateIconRectTransform(Vector2? position, Vector2? size)
        {
            var rectTransform = this.imgReward.GetComponent<RectTransform>();

            if (position is not null)
            {
                rectTransform.anchoredPosition = position.Value;
            }

            if (size is not null)
            {
                rectTransform.sizeDelta = size.Value;
            }
        }
    }

    public class UITemplateDailyRewardItemPresenter : BaseUIItemPresenter<UITemplateDailyRewardItemView, UITemplateDailyRewardItemModel>
    {
        public UITemplateDailyRewardItemModel Model { get; set; }

        #region inject

        private readonly UITemplateDailyRewardItemViewHelper dailyRewardItemViewHelper;

        #endregion

        public UITemplateDailyRewardItemPresenter(IGameAssets gameAssets, UITemplateDailyRewardItemViewHelper dailyRewardItemViewHelper) : base(gameAssets)
        {
            this.dailyRewardItemViewHelper = dailyRewardItemViewHelper;
        }

        public override void BindData(UITemplateDailyRewardItemModel param)
        {
            this.Model = param;
            this.dailyRewardItemViewHelper.BindDataItem(param, this.View, this);
        }

        public override void Dispose() { this.dailyRewardItemViewHelper.DisposeItem(this.View); }

        public void ClaimReward() { this.dailyRewardItemViewHelper.OnClaimReward(); }
    }
}