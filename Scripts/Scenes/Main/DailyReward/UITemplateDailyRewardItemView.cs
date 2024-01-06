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
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main.CollectionNew;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UITemplateDailyRewardItemModel
    {
        public Action<int>                 OnClick           { get; set; }
        public RewardStatus                RewardStatus      { get; set; }
        public UITemplateDailyRewardRecord DailyRewardRecord { get; set; }

        public UITemplateDailyRewardItemModel(UITemplateDailyRewardRecord dailyRewardRecord, RewardStatus rewardStatus, Action<int> click)
        {
            this.DailyRewardRecord = dailyRewardRecord;
            this.RewardStatus      = rewardStatus;
            this.OnClick           = click;
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
        public Sprite          sprBgNormal;
        public Sprite          sprBgCurrentDay;
        public Button          btnClaim;

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
        #region inject

        private readonly ILogService                         logService;
        private readonly UITemplateDailyRewardItemViewHelper dailyRewardItemViewHelper;

        #endregion

        public UITemplateDailyRewardItemPresenter(IGameAssets gameAssets, ILogService logService,
            UITemplateDailyRewardItemViewHelper dailyRewardItemViewHelper) : base(gameAssets)
        {
            this.logService                = logService;
            this.dailyRewardItemViewHelper = dailyRewardItemViewHelper;
        }

        public override void BindData(UITemplateDailyRewardItemModel param) { this.dailyRewardItemViewHelper.BindDataItem(param, this.View); }
    }
}