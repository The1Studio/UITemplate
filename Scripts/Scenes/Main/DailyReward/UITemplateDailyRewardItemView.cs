namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main.DailyReward
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.MVP;
    using GameFoundation.Scripts.Utilities.LogService;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UITemplateDailyRewardItemModel
    {
        public UITemplateDailyRewardItemModel(UITemplateDailyRewardRecord dailyRewardRecord, Action onClickClaim)
        {
            this.DailyRewardRecord = dailyRewardRecord;
            this.OnClickClaim      = onClickClaim;
        }

        public UITemplateDailyRewardRecord DailyRewardRecord { get; set; }
        public Action                      OnClickClaim      { get; set; }
    }

    public class UITemplateDailyRewardItemView : TViewMono
    {
        public Image           imgReward;
        public GameObject      objLockReward;
        public GameObject      objClaimed;
        public TextMeshProUGUI txtValue;
        public TextMeshProUGUI txtDayLabel;
        public Button          btnClaim;
    }

    public class UITemplateDailyRewardItemPresenter : BaseUIItemPresenter<UITemplateDailyRewardItemView, UITemplateDailyRewardItemModel>
    {
        #region inject

        private readonly ILogService                     logService;
        private readonly UITemplateDailyRewardData       uiTemplateDailyRewardData;
        private readonly UITemplateDailyRewardController uiTemplateDailyRewardController;

        #endregion

        private const string TodayLabel  = "TODAY";
        private const string PrefixLabel = "DAY ";

        private UITemplateDailyRewardItemModel model;

        public UITemplateDailyRewardItemPresenter(IGameAssets gameAssets, ILogService logService, UITemplateDailyRewardData uiTemplateDailyRewardData,
            UITemplateDailyRewardController uiTemplateDailyRewardController) : base(gameAssets)
        {
            this.logService                      = logService;
            this.uiTemplateDailyRewardData       = uiTemplateDailyRewardData;
            this.uiTemplateDailyRewardController = uiTemplateDailyRewardController;
        }

        public override async void BindData(UITemplateDailyRewardItemModel param)
        {
            this.View.btnClaim.onClick.AddListener(this.OnClickClaimReward);
            this.model                      = param;
            this.View.btnClaim.interactable = this.uiTemplateDailyRewardData.RewardStatus[this.model.DailyRewardRecord.Day - 1] != RewardStatus.Locked;
            this.InitView();
        }

        private void InitView()
        {
            var rewardSprite = this.GameAssets.ForceLoadAsset<Sprite>($"{this.model.DailyRewardRecord.RewardImage}");
            var rewardValue  = string.Empty;
            if (this.model.DailyRewardRecord.Reward.Count == 1)
                rewardValue = this.model.DailyRewardRecord.Reward.First().Values.First() == 1
                    ? rewardValue
                    : this.model.DailyRewardRecord.Reward.First().Values.First().ToString();
            this.View.imgReward.sprite = rewardSprite;
            this.View.txtValue.text    = rewardValue;
            this.View.txtDayLabel.text = this.model.DailyRewardRecord.Day == this.uiTemplateDailyRewardData.RewardStatus.Count
                ? TodayLabel
                : $"{PrefixLabel}{this.model.DailyRewardRecord.Day}";
            this.View.objLockReward.SetActive(this.uiTemplateDailyRewardData.RewardStatus[this.model.DailyRewardRecord.Day - 1] == RewardStatus.Locked);
            this.View.objClaimed.SetActive(this.uiTemplateDailyRewardData.RewardStatus[this.model.DailyRewardRecord.Day - 1] == RewardStatus.Claimed);
        }

        private void OnClickClaimReward()
        {
            this.uiTemplateDailyRewardData.RewardStatus[this.model.DailyRewardRecord.Day - 1] = RewardStatus.Claimed;
            this.logService.LogWithColor("Add reward to local here! ", Color.yellow);
            this.InitView();
            this.model.OnClickClaim?.Invoke();
        }

        public override void Dispose()
        {
            base.Dispose();
            this.View.btnClaim.onClick.RemoveAllListeners();
        }
    }
}