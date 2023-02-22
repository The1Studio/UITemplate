namespace TheOneStudio.UITemplate.UITemplate.Scenes.Utils
{
    using System.Collections.Generic;
    using System.Linq;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.MVP;
    using GameFoundation.Scripts.Utilities.LogService;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TMPro;
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

        private Dictionary<RewardStatus, List<GameObject>> statusToGameObjectsMap;

        private void Start()
        {
            this.statusToGameObjectsMap = new()
                                          {
                                              { RewardStatus.Locked, new List<GameObject> { this.objLockReward } },
                                              { RewardStatus.Unlocked, new List<GameObject> { } },
                                              { RewardStatus.Claimed, new List<GameObject> { this.objClaimed } }
                                          };
        }

        public Button BtnClaim => this.btnClaim;

        public void SetStatus(RewardStatus rewardStatus, Sprite rewardSprite, string value, string label)
        {
            this.txtDayLabel.text = label;
            this.txtValue.text    = value;
            this.imgReward.sprite = rewardSprite;

            foreach (var kvp in this.statusToGameObjectsMap)
            {
                foreach (var go in kvp.Value)
                {
                    go.SetActive(kvp.Key == rewardStatus);
                }
            }
        }
    }

    public class UITemplateDailyRewardItemPresenter : BaseUIItemPresenter<UITemplateDailyRewardItemView, UITemplateDailyRewardItemModel>
    {
        #region inject

        private readonly ILogService               logService;
        private readonly UITemplateUserDailyRewardData userDailyRewardData;

        #endregion

        private const string TodayLabel  = "TODAY";
        private const string PrefixLabel = "DAY ";

        private UITemplateDailyRewardItemModel model;
        private int                            userLoginDay;

        public UITemplateDailyRewardItemPresenter(IGameAssets gameAssets, ILogService logService,UITemplateUserDailyRewardData userDailyRewardData) : base(gameAssets)
        {
            this.logService      = logService;
            this.userDailyRewardData = userDailyRewardData;
        }

        public override async void BindData(UITemplateDailyRewardItemModel param)
        {
            this.View.BtnClaim.onClick.RemoveAllListeners();
            this.View.BtnClaim.onClick.AddListener(this.OnClickClaimReward);
            this.userLoginDay               = await this.userDailyRewardData.GetUserLoginDay();
            this.model                      = param;
            this.View.BtnClaim.interactable = this.userDailyRewardData.RewardStatus[this.model.DailyRewardRecord.Day - 1] != RewardStatus.Locked;
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

            var rewardLabel = this.model.DailyRewardRecord.Day == this.userLoginDay ? TodayLabel : PrefixLabel + this.model.DailyRewardRecord.Day;
            this.View.SetStatus(this.userDailyRewardData.RewardStatus[this.model.DailyRewardRecord.Day - 1], rewardSprite, rewardValue, rewardLabel);
        }

        private void OnClickClaimReward()
        {
            this.userDailyRewardData.RewardStatus[this.model.DailyRewardRecord.Day - 1] = RewardStatus.Claimed;
            this.logService.LogWithColor("Add reward to local here! ", Color.yellow);
            this.InitView();
        }
    }
}