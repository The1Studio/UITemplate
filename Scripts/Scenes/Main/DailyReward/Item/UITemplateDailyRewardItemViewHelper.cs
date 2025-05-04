namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main.DailyReward.Item
{
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.Utilities.LoadImage;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;
    using UnityEngine.Scripting;

    // Rebind this class to your own item view
    public class UITemplateDailyRewardItemViewHelper
    {
        protected readonly IGameAssets                     GameAssets;
        protected readonly UITemplateDailyRewardController DailyRewardController;
        private readonly   LoadImageHelper                 loadImageHelper;

        [Preserve]
        public UITemplateDailyRewardItemViewHelper(IGameAssets gameAssets, UITemplateDailyRewardController dailyRewardController, LoadImageHelper loadImageHelper)
        {
            this.GameAssets            = gameAssets;
            this.DailyRewardController = dailyRewardController;
            this.loadImageHelper       = loadImageHelper;
        }

        public virtual void BindDataItem(UITemplateDailyRewardItemModel model, UITemplateDailyRewardItemView view, UITemplateDailyRewardItemPresenter presenter)
        {
            view.ImgReward.gameObject.SetActive(!string.IsNullOrEmpty(model.RewardRecord.RewardImage));
            if (!string.IsNullOrEmpty(model.RewardRecord.RewardImage))
            {
                this.loadImageHelper.LoadLocalSpriteToUIImage(view.ImgReward, model.RewardRecord.RewardImage)
                    .Forget();
            }

            view.TxtValue.text = $"{model.RewardRecord.RewardValue}";
            view.TxtValue.gameObject.SetActive(model.RewardRecord.ShowValue);
            view.UpdateIconRectTransform(model.RewardRecord.Position, model.RewardRecord.Size);
            view.ObjReward.SetActive(model.RewardStatus != RewardStatus.Locked || model.RewardRecord.SpoilReward);
            view.ObjLock.SetActive(!view.ObjReward.activeSelf);
        }

        public virtual void DisposeItem(UITemplateDailyRewardItemPresenter presenter)
        {
        }
    }
}