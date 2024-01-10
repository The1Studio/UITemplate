namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main.DailyReward.Item
{
    using GameFoundation.Scripts.AssetLibrary;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using UnityEngine;

    // Rebind this class to your own item view
    public class UITemplateDailyRewardItemViewHelper
    {
        private const string TodayLabel  = "TODAY";
        private const string PrefixLabel = "DAY ";

        protected readonly IGameAssets                     GameAssets;
        protected readonly UITemplateDailyRewardController DailyRewardController;

        public UITemplateDailyRewardItemViewHelper(IGameAssets gameAssets, UITemplateDailyRewardController dailyRewardController)
        {
            this.GameAssets            = gameAssets;
            this.DailyRewardController = dailyRewardController;
        }

        public virtual async void BindDataItem(RewardRecord model, UITemplateDailyRewardItemView view, UITemplateDailyRewardItemPresenter presenter)
        {
            var rewardSprite = this.GameAssets.ForceLoadAsset<Sprite>($"{model.RewardImage}");
            view.imgReward.sprite = rewardSprite;
            view.txtValue.text    = $"{model.RewardValue}";

            view.txtValue.gameObject.SetActive(model.ShowValue);

            view.UpdateIconRectTransform(model.Position, model.Size);

            view.objReward.gameObject.SetActive(!model.SpoilReward);
            view.objLockReward.SetActive(!model.SpoilReward);
        }

        public virtual void DisposeItem(UITemplateDailyRewardItemPresenter presenter) { }

        public virtual void OnClaimReward(UITemplateDailyRewardItemPresenter presenter) { }
    }
}