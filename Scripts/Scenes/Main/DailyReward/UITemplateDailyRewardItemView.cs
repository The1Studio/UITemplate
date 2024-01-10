namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main.DailyReward
{
    using System;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.MVP;
    using GameFoundation.Scripts.Utilities.LogService;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UITemplateDailyRewardItemModel
    {
        public RewardStatus                RewardStatus      { get; set; }
        public UITemplateDailyRewardRecord DailyRewardRecord { get; set; }

        public UITemplateDailyRewardItemModel(UITemplateDailyRewardRecord dailyRewardRecord, RewardStatus rewardStatus)
        {
            this.DailyRewardRecord = dailyRewardRecord;
            this.RewardStatus      = rewardStatus;
        }
    }

    public class UITemplateDailyRewardItemView : TViewMono
    {
        public Image           imgBackground;
        public Image           imgReward;
        public GameObject      objLockReward;
        public GameObject      objClaimed;
        public TextMeshProUGUI txtValue;
        public TextMeshProUGUI txtDayLabel;
        public GameObject      objClaimedCheckIcon;
        public Sprite sprBgNormal;
        public Sprite sprBgCurrentDay;

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
        #region inject

        private readonly ILogService                     logService;
        private readonly UITemplateDailyRewardController dailyRewardController;

        #endregion

        private const string TodayLabel  = "TODAY";
        private const string PrefixLabel = "DAY ";

        private UITemplateDailyRewardItemModel model;

        public UITemplateDailyRewardItemPresenter(IGameAssets gameAssets, ILogService logService, UITemplateDailyRewardController dailyRewardController) : base(gameAssets)
        {
            this.logService            = logService;
            this.dailyRewardController = dailyRewardController;
        }

        public override void BindData(UITemplateDailyRewardItemModel param)
        {
            this.model = param;
            this.InitView();
        }

        private async void InitView()
        {
            var rewardSprite = this.GameAssets.ForceLoadAsset<Sprite>($"{this.model.DailyRewardRecord.RewardImage}");
            var rewardValue  = string.Empty;
            if (this.model.DailyRewardRecord.Reward.Count == 1)
                rewardValue = this.model.DailyRewardRecord.Reward.Values.First() == -1
                    ? rewardValue
                    : this.model.DailyRewardRecord.Reward.Values.First().ToString();
            this.View.imgReward.sprite = rewardSprite;
            this.View.txtValue.text    = rewardValue;
            this.View.txtDayLabel.text = this.model.DailyRewardRecord.Day == this.dailyRewardController.GetCurrentDayIndex() + 1
                ? TodayLabel
                : $"{PrefixLabel}{this.model.DailyRewardRecord.Day}";
            if(this.View.imgBackground != null)
                this.View.imgBackground.sprite = this.model.DailyRewardRecord.Day == this.dailyRewardController.GetCurrentDayIndex() + 1
                    ? this.View.sprBgCurrentDay
                    : this.View.sprBgNormal;
            this.View.objLockReward.SetActive(this.model.RewardStatus == RewardStatus.Locked);
            this.View.objClaimed.SetActive(this.model.RewardStatus == RewardStatus.Claimed);

            this.View.UpdateIconRectTransform(this.model.DailyRewardRecord.Position, this.model.DailyRewardRecord.Size);

            //Only play if the items were not claimed
            if (!this.View.objClaimed.activeSelf)
            {
                //Animation
                var duration = 1f;
                this.View.objClaimedCheckIcon.transform.localScale = Vector3.zero;
                this.View.objClaimedCheckIcon.transform.DOScale(Vector3.one, duration).SetEase(Ease.OutBounce);
                await UniTask.Delay(TimeSpan.FromSeconds(duration));
            }
        }
    }
}