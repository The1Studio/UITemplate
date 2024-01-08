namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main.DailyReward
{
    using System;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using GameFoundation.Scripts.AssetLibrary;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;
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

        public virtual async void BindDataItem(UITemplateDailyRewardItemModel model, UITemplateDailyRewardItemView view, UITemplateDailyRewardItemPresenter presenter)
        {
            var rewardSprite = this.GameAssets.ForceLoadAsset<Sprite>($"{model.DailyRewardRecord.RewardImage}");
            var rewardValue  = string.Empty;
            if (model.DailyRewardRecord.Reward.Count == 1 && model.DailyRewardRecord.Reward.Values.First() != -1)
            {
                rewardValue = $"{model.DailyRewardRecord.Reward.Values.First()}";
            }

            view.imgReward.sprite     = rewardSprite;
            view.txtValue.text        = rewardValue;
            view.txtDayLabel.text     = model.DailyRewardRecord.Day == this.DailyRewardController.GetCurrentDayIndex() + 1 ? TodayLabel : $"{PrefixLabel}{model.DailyRewardRecord.Day}";
            view.imgBackground.sprite = model.DailyRewardRecord.Day == this.DailyRewardController.GetCurrentDayIndex() + 1 ? view.sprBgCurrentDay : view.sprBgNormal;

            view.objClaimed.SetActive(model.RewardStatus == RewardStatus.Claimed);
            view.txtValue.gameObject.SetActive(model.DailyRewardRecord.ShowValue);

            view.UpdateIconRectTransform(model.DailyRewardRecord.Position, model.DailyRewardRecord.Size);

            view.OnClickClaimButton = () => model.OnClick?.Invoke(presenter);

            if (view.objClaimByAds != null)
            {
                view.objClaimByAds.SetActive(model.RewardStatus == RewardStatus.Locked && model.IsGetWithAds);
            }

            view.objLockReward.SetActive(model.RewardStatus == RewardStatus.Locked && !model.DailyRewardRecord.SpoilReward);

            //Only play if the items were not claimed
            if (!view.objClaimed.activeSelf)
            {
                //Animation
                var duration = 1f;
                view.objClaimedCheckIcon.transform.localScale = Vector3.zero;
                view.objClaimedCheckIcon.transform.DOScale(Vector3.one, duration).SetEase(Ease.OutBounce);
                await UniTask.Delay(TimeSpan.FromSeconds(duration));
            }
        }

        public virtual void DisposeItem(UITemplateDailyRewardItemPresenter presenter) { }

        public virtual void OnClaimReward(UITemplateDailyRewardItemPresenter presenter) { }
    }
}