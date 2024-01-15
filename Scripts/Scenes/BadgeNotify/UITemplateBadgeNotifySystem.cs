namespace TheOneStudio.UITemplate.UITemplate.Scenes.BadgeNotify
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Signals;
    using GameFoundation.Scripts.Utilities.Extension;
    using Zenject;

    public class UITemplateBadgeNotifySystem : IInitializable

    {
        #region inject

        private readonly IScreenManager screenManager;
        private readonly SignalBus      signalBus;

        #endregion

        private Dictionary<Type, HashSet<string>>                    screenTypeToBadgeTemp    = new();
        private Dictionary<string, Func<bool>>                       badgeToConditionFuncTemp = new();
        private Dictionary<Type, HashSet<UITemplateBadgeNotifyView>> screenTypeToBadges       = new();
        private Dictionary<UITemplateBadgeNotifyView, Type>          badgeToNextScreenType    = new();
        private Dictionary<UITemplateBadgeNotifyView, Func<bool>>    badgeToConditionFunc     = new();
        private IScreenPresenter                                     currentPresenter;

        public UITemplateBadgeNotifySystem(IScreenManager screenManager, SignalBus signalBus)
        {
            this.screenManager = screenManager;
            this.signalBus     = signalBus;
        }

        public void RegisterBadgeNextScreenType(UITemplateBadgeNotifyView badgeNotifyView, IScreenPresenter parentScreenPresenter, Type nextScreenType)
        {
            this.RegisParentScreen(badgeNotifyView, parentScreenPresenter.GetType(),null);
            this.badgeToNextScreenType.Add(badgeNotifyView, nextScreenType);
        }

        private void RegisterBadgeCondition(UITemplateBadgeNotifyView badgeNotifyView, IScreenPresenter parentScreen, Func<bool> condition, string badgeId = null)
        {
            this.RegisParentScreen(badgeNotifyView, parentScreen.GetType(),badgeId);
            if (badgeNotifyView == null && badgeId!=null)
            {
                this.badgeToConditionFuncTemp.Add(badgeId,condition);
                return;
            }
            this.badgeToConditionFunc.Add(badgeNotifyView, condition);
            if (this.badgeToConditionFuncTemp.ContainsKey(badgeNotifyView.badgeId)) this.badgeToConditionFuncTemp.Remove(badgeNotifyView.badgeId);
        }

        private void RegisParentScreen(UITemplateBadgeNotifyView badgeNotifyView, Type parentScreenType, string badgeId)
        {
            if (parentScreenType == null) return;
            if (badgeNotifyView == null)
            {
                var badgeTempSet = this.screenTypeToBadgeTemp.GetOrAdd(parentScreenType, () => new HashSet<string>());
                badgeTempSet.Add(badgeId);

                return;
            }
            var badgeSet = this.screenTypeToBadges.GetOrAdd(parentScreenType, () => new HashSet<UITemplateBadgeNotifyView>());
            badgeSet.Add(badgeNotifyView);
            if (this.screenTypeToBadgeTemp.ContainsKey(parentScreenType)) this.screenTypeToBadgeTemp.Remove(parentScreenType);
        }

        private bool GetBadgeStatus(UITemplateBadgeNotifyView badgeNotifyView)
        {
            if (this.badgeToConditionFunc.TryGetValue(badgeNotifyView, out var conditionFunc))
            {
                return conditionFunc.Invoke();
            }

            return this.screenTypeToBadges[this.badgeToNextScreenType[badgeNotifyView]].Any(this.GetBadgeStatus);
        }

        #region BadgeNotifyFunction

        public void RegisterBadge<TPresenter>(UITemplateBadgeNotifyView badgeView, IScreenPresenter parentScreenPresenter)
            where TPresenter : IScreenPresenter
        {
            this.RegisterBadgeNextScreenType(badgeView, parentScreenPresenter, typeof(TPresenter));
        }

        public void RegisterBadge(UITemplateBadgeNotifyView badgeView, IScreenPresenter parentScreenPresenter, Func<bool> condition)
        {
            this.RegisterBadgeCondition(badgeView, parentScreenPresenter, condition);
        }

        private void SetActiveBadge(UITemplateBadgeNotifyView badgeView) { badgeView.badge.SetActive(this.GetBadgeStatus(badgeView)); }

        #endregion

        public void CheckAllBadgeNotifyStatus(bool force = true)
        {
            var currentScreenPresenter = this.screenManager.CurrentActiveScreen.Value;

            if (!force && currentScreenPresenter.Equals(this.currentPresenter)) return;
            this.currentPresenter = currentScreenPresenter;
            if (!this.screenTypeToBadges.TryGetValue(currentScreenPresenter.GetType(), out var badgeNotifyButtonViews)) return;
            badgeNotifyButtonViews.ForEach(this.SetActiveBadge);
        }

        private void CheckAllBadgeNotifyStatusWhenScreenStatusChange() => this.CheckAllBadgeNotifyStatus(false);

        public void Initialize()
        {
            this.signalBus.Subscribe<ScreenShowSignal>(this.CheckAllBadgeNotifyStatusWhenScreenStatusChange);
            this.signalBus.Subscribe<ScreenCloseSignal>(this.CheckAllBadgeNotifyStatusWhenScreenStatusChange);
            this.signalBus.Subscribe<PopupHiddenSignal>(this.CheckAllBadgeNotifyStatusWhenScreenStatusChange);
        }
    }
}