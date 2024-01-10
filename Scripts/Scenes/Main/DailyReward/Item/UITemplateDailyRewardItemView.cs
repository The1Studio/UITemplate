namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main.DailyReward.Item
{
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.MVP;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UITemplateDailyRewardItemView : TViewMono
    {
        [Header("Reward")] public GameObject      objReward;
        public                    Image           imgReward;
        public                    TextMeshProUGUI txtValue;

        [Header("Lock")] public GameObject objLockReward;

        public void UpdateIconRectTransform(Vector2? position, Vector2? size)
        {
            var rectTransform = this.objReward.GetComponent<RectTransform>();

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

    public class UITemplateDailyRewardItemPresenter : BaseUIItemPresenter<UITemplateDailyRewardItemView, RewardRecord>
    {
        public RewardRecord Model { get; set; }

        #region inject

        private readonly UITemplateDailyRewardItemViewHelper dailyRewardItemViewHelper;

        #endregion

        public UITemplateDailyRewardItemPresenter(IGameAssets gameAssets, UITemplateDailyRewardItemViewHelper dailyRewardItemViewHelper) : base(gameAssets)
        {
            this.dailyRewardItemViewHelper = dailyRewardItemViewHelper;
        }

        public override void BindData(RewardRecord param)
        {
            this.Model = param;
            this.dailyRewardItemViewHelper.BindDataItem(param, this.View, this);
        }

        public override void Dispose() { this.dailyRewardItemViewHelper.DisposeItem(this); }

        public void ClaimReward() { this.dailyRewardItemViewHelper.OnClaimReward(this); }
    }
}