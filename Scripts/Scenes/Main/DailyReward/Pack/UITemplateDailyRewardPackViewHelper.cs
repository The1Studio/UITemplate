namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main.DailyReward.Pack
{
    using System;
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using GameFoundation.Scripts.AssetLibrary;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;
    using UnityEngine;
    using UnityEngine.Scripting;

    // Rebind this class to your own pack view
    public class UITemplateDailyRewardPackViewHelper
    {
        private const string TodayLabel  = "TODAY";
        private const string PrefixLabel = "DAY ";

        protected readonly IGameAssets                     GameAssets;
        protected readonly UITemplateDailyRewardController DailyRewardController;

        [Preserve]
        public UITemplateDailyRewardPackViewHelper(IGameAssets gameAssets, UITemplateDailyRewardController dailyRewardController)
        {
            this.GameAssets            = gameAssets;
            this.DailyRewardController = dailyRewardController;
        }

        public virtual async void BindDataItem(UITemplateDailyRewardPackModel model, UITemplateDailyRewardPackView view, UITemplateDailyRewardPackPresenter presenter)
        {
            view.TxtDayLabel.text = model.DailyRewardRecord.Day == this.DailyRewardController.GetCurrentDayIndex() + 1 ? TodayLabel : $"{PrefixLabel}{model.DailyRewardRecord.Day}";
            if (view.ImgBackground != null) view.ImgBackground.sprite = model.DailyRewardRecord.Day == this.DailyRewardController.GetCurrentDayIndex() + 1 ? view.SprBgCurrentDay : view.SprBgNormal;

            view.ObjClaimedCheckIcon.SetActive(model.RewardStatus == RewardStatus.Claimed);
            view.ObjClaimed.SetActive(model.RewardStatus == RewardStatus.Claimed);
            view.OnClickClaimButton = () => model.OnClick?.Invoke(presenter);

            if (view.ObjClaimByAds != null)
            {
                view.ObjClaimByAds.SetActive(model.RewardStatus == RewardStatus.Locked && model.IsGetWithAds);
            }

            //Only play if the items were not claimed
            if (!view.ObjClaimed.activeSelf)
            {
                //Animation
                var duration = 1f;
                view.ObjClaimedCheckIcon.transform.localScale = Vector3.zero;
                view.ObjClaimedCheckIcon.transform.DOScale(Vector3.one, duration).SetEase(Ease.OutBounce);
                await UniTask.Delay(TimeSpan.FromSeconds(duration));
            }

            if (view.PackImg != null)
            {
                view.PackImg.sprite = this.GameAssets.ForceLoadAsset<Sprite>(model.DailyRewardRecord.PackImage);
            }
        }

        public virtual void DisposeItem(UITemplateDailyRewardPackPresenter presenter) { }

        public virtual void OnClaimReward(UITemplateDailyRewardPackPresenter presenter) { }
    }
}