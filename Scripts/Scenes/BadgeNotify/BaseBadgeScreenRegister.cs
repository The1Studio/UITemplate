namespace TheOneStudio.UITemplate.UITemplate.Scenes.BadgeNotify
{
    using System;
    using System.Collections.Generic;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;

    public abstract class BaseBadgeScreenRegister<TPresenter> : IBadgeScreenRegister where TPresenter :IScreenPresenter
    {
        private readonly Dictionary<string, Func<bool>> badgeIdToCondition = new();

        #region Inject

        private readonly UITemplateBadgeNotifySystem badgeNotifySystem;

        protected BaseBadgeScreenRegister(UITemplateBadgeNotifySystem badgeNotifySystem)
        {
            this.badgeNotifySystem = badgeNotifySystem;
        }

        #endregion

        public abstract void RegisterBadgesOnScreen(TPresenter presenter);

        public abstract void RegisterBadgesConditionOnScreen();

        protected void RegisterBadgeCondition(string badgeId, Func<bool> condition)
        {
            this.badgeIdToCondition.Add(badgeId, condition);
            this.badgeNotifySystem.RegisterBadge(typeof(TPresenter), condition, badgeId);
        }

        private Func<bool> GetConditionFromBadge(UITemplateBadgeNotifyView badgeNotifyView)
        {
            return this.badgeIdToCondition.GetValueOrDefault(badgeNotifyView.badgeId);
        }

        protected void RegisterBadge(UITemplateBadgeNotifyView badgeNotifyView, TPresenter presenter)
        {
            var condition = this.GetConditionFromBadge(badgeNotifyView);
            this.badgeNotifySystem.RegisterBadge(badgeNotifyView, presenter, condition);
        }
    }
}