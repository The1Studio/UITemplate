namespace UITemplate.Scripts.Scenes.Utils
{
    using System;
    using System.Linq;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.MVP;
    using GameFoundation.Scripts.Utilities.LogService;
    using LocalData;
    using TMPro;
    using UITemplate.Scripts.Blueprints;
    using UITemplate.Scripts.Models;
    using UnityEngine;
    using UnityEngine.UI;

    public class UITemplateDailyRewardItemModel
    {
        public UITemplateDailyRewardRecord DailyRewardRecord { get; set; }

        public UITemplateDailyRewardItemModel(UITemplateDailyRewardRecord dailyRewardRecord) { this.DailyRewardRecord = dailyRewardRecord; }
    }

    public class UITemplateDailyRewardItemView : TViewMono
    {
        [SerializeField] private Image           imgReward;
        [SerializeField] private GameObject      objLockReward;
        [SerializeField] private GameObject      objClaimed;
        [SerializeField] private TextMeshProUGUI txtValue;
        [SerializeField] private TextMeshProUGUI txtDayLabel;
        [SerializeField] private Button          btnClaim;

        public Button BtnClaim => this.btnClaim;

        public void SetStatus(RewardStatus rewardStatus, Sprite rewardSprite, string value, string label)
        {
            this.txtDayLabel.text = label;
            this.txtValue.text    = value;
            this.imgReward.sprite = rewardSprite;
            switch (rewardStatus)
            {
                case RewardStatus.Locked:
                    this.objLockReward.SetActive(true);
                    this.objClaimed.SetActive(false);
                    break;
                case RewardStatus.Unlocked:
                    this.objLockReward.SetActive(false);
                    this.objClaimed.SetActive(false);
                    break;
                case RewardStatus.Claimed:
                    this.objLockReward.SetActive(false);
                    this.objClaimed.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(rewardStatus), rewardStatus, null);
            }
        }
    }

    public class UITemplateDailyRewardItemPresenter : BaseUIItemPresenter<UITemplateDailyRewardItemView, UITemplateDailyRewardItemModel>
    {
        private readonly ILogService                    logService;
        private readonly UserLocalData                  localData;
        private          UITemplateDailyRewardItemModel model;

        private int userLoginDay;
        public UITemplateDailyRewardItemPresenter(IGameAssets gameAssets, ILogService logService, UserLocalData localData) : base(gameAssets)
        {
            this.logService = logService;
            this.localData  = localData;
        }

        public override async void BindData(UITemplateDailyRewardItemModel param)
        {
            this.View.BtnClaim.onClick.RemoveAllListeners();
            this.View.BtnClaim.onClick.AddListener(this.OnClickClaimReward);
            this.userLoginDay               = await this.localData.RewardData.GetUserLoginDay();
            this.model                      = param;
            this.View.BtnClaim.interactable = this.localData.RewardData.RewardStatus[this.model.DailyRewardRecord.Day - 1] != RewardStatus.Locked;
            this.InitView();
        }

        private void InitView()
        {
            var rewardSprite = this.GameAssets.ForceLoadAsset<Sprite>($"{this.model.DailyRewardRecord.RewardImage}");
            var rewardValue  = "";
            if (this.model.DailyRewardRecord.Reward.Count == 1)
            {
                rewardValue = this.model.DailyRewardRecord.Reward.First().Values.First() == 1 ? "" : this.model.DailyRewardRecord.Reward.First().Values.First().ToString();
            }

            var rewardLabel = this.model.DailyRewardRecord.Day == this.userLoginDay ? "TODAY" : "DAY " + this.model.DailyRewardRecord.Day;
            this.View.SetStatus(this.localData.RewardData.RewardStatus[this.model.DailyRewardRecord.Day - 1], rewardSprite, rewardValue, rewardLabel);
        }

        private void OnClickClaimReward()
        {
            this.localData.RewardData.RewardStatus[this.model.DailyRewardRecord.Day - 1] = RewardStatus.Claimed;
            this.logService.LogWithColor("Add reward to local here! ", Color.yellow);
            this.InitView();
        }
    }
}