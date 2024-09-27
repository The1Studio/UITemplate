namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main.DailyReward
{
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main.DailyReward.Item;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main.DailyReward.Pack;
    using UnityEngine.Scripting;

    public interface IDailyRewardAnimationHelper
    {
        public UniTask PlayPreClaimRewardAnimation(UITemplateDailyRewardPopupPresenter  dailyRewardPopupPresenter);
        public UniTask PlayPostClaimRewardAnimation(UITemplateDailyRewardPopupPresenter dailyRewardPopupPresenter);
    }

    [Preserve]
    public class DailyRewardAnimationHelper : IDailyRewardAnimationHelper
    {
        public virtual UniTask PlayPreClaimRewardAnimation(UITemplateDailyRewardPopupPresenter dailyRewardPopupPresenter) { return UniTask.CompletedTask; }

        public virtual UniTask PlayPostClaimRewardAnimation(UITemplateDailyRewardPopupPresenter dailyRewardPopupPresenter) { return UniTask.CompletedTask; }

        protected List<UITemplateDailyRewardPackPresenter> GetPackPresenters(UITemplateDailyRewardPopupPresenter dailyRewardPopupPresenter) { return dailyRewardPopupPresenter.View.dailyRewardPackAdapter.GetPresenters(); }

        protected List<UITemplateDailyRewardItemPresenter> GetItemPresenters(UITemplateDailyRewardPackPresenter packPresenter) { return packPresenter.View.DailyRewardItemAdapter.GetPresenters(); }
    }
}