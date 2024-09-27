namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main.DailyReward.Item
{
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.AssetLibrary;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;
    using UnityEngine;
    using UnityEngine.Scripting;

    // Rebind this class to your own item view
    public class UITemplateDailyRewardItemViewHelper
    {
        protected readonly IGameAssets                     GameAssets;
        protected readonly UITemplateDailyRewardController DailyRewardController;

        [Preserve]
        public UITemplateDailyRewardItemViewHelper(IGameAssets gameAssets, UITemplateDailyRewardController dailyRewardController)
        {
            this.GameAssets            = gameAssets;
            this.DailyRewardController = dailyRewardController;
        }

        public virtual void BindDataItem(UITemplateDailyRewardItemModel model, UITemplateDailyRewardItemView view, UITemplateDailyRewardItemPresenter presenter)
        {
            view.ImgReward.gameObject.SetActive(!string.IsNullOrEmpty(model.RewardRecord.RewardImage));
            if (!string.IsNullOrEmpty(model.RewardRecord.RewardImage))
            {
                var rewardSprite = this.GameAssets.ForceLoadAsset<Sprite>($"{model.RewardRecord.RewardImage}");
                view.ImgReward.sprite = rewardSprite;
            }

            view.TxtValue.text = $"{model.RewardRecord.RewardValue}";
            view.TxtValue.gameObject.SetActive(model.RewardRecord.ShowValue);
            view.UpdateIconRectTransform(model.RewardRecord.Position, model.RewardRecord.Size);
            view.ObjReward.SetActive(model.RewardStatus != RewardStatus.Locked || model.RewardRecord.SpoilReward);
            view.ObjLock.SetActive(!view.ObjReward.activeSelf);
        }

        public virtual void DisposeItem(UITemplateDailyRewardItemPresenter presenter) { }
    }
}