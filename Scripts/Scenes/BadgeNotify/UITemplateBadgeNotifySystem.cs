namespace TheOneStudio.UITemplate.UITemplate.Scenes.BadgeNotify
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GameFoundation.DI;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Signals;
    using GameFoundation.Scripts.Utilities.Extension;
    using GameFoundation.Signals;
    using UnityEngine.Scripting;

    public class UITemplateBadgeNotifySystem : IInitializable

    {
        #region inject

        private readonly IScreenManager screenManager;
        private readonly SignalBus      signalBus;

        #endregion

        private readonly Dictionary<string, Func<bool>>                       badgeToAdapterBadgeItem  = new();
        private readonly Dictionary<Type, HashSet<string>>                    screenTypeToBadgeTemp    = new();
        private readonly Dictionary<string, Func<bool>>                       badgeToConditionFuncTemp = new();
        private readonly Dictionary<Type, HashSet<UITemplateBadgeNotifyView>> screenTypeToBadges       = new();
        private readonly Dictionary<UITemplateBadgeNotifyView, Type>          badgeToNextScreenType    = new();
        private readonly Dictionary<UITemplateBadgeNotifyView, Func<bool>>    badgeToConditionFunc     = new();

        private IScreenPresenter currentPresenter;

        [Preserve]
        public UITemplateBadgeNotifySystem(IScreenManager screenManager, SignalBus signalBus)
        {
            this.screenManager = screenManager;
            this.signalBus     = signalBus;
        }

        private void RegisterBadgeNextScreenType(UITemplateBadgeNotifyView badgeNotifyView, IScreenPresenter parentScreenPresenter, Type nextScreenType)
        {
            this.RegisParentScreen(badgeNotifyView, parentScreenPresenter.GetType());
            this.badgeToNextScreenType.Add(badgeNotifyView, nextScreenType);
        }

        private void RegisterBadgeNextScreenType(UITemplateBadgeNotifyView badgeNotifyView, Type parentScreenPresenter, Type nextScreenType)
        {
            this.RegisParentScreen(badgeNotifyView, parentScreenPresenter);
            this.badgeToNextScreenType.Add(badgeNotifyView, nextScreenType);
        }

        private void RegisterBadgeCondition(UITemplateBadgeNotifyView badgeNotifyView, IScreenPresenter parentScreen, Func<bool> condition, string badgeId = null)
        {
            this.RegisParentScreen(badgeNotifyView, parentScreen.GetType());
            this.badgeToConditionFunc.Add(badgeNotifyView, condition);
            if (this.badgeToConditionFuncTemp.ContainsKey(badgeNotifyView.badgeId)) this.badgeToConditionFuncTemp.Remove(badgeNotifyView.badgeId);
        }

        private void RegisterBadgeConditionTemp(Type parentScreen, Func<bool> condition, string badgeId)
        {
            this.RegisterParentScreenTemp(parentScreen, badgeId);
            this.badgeToConditionFuncTemp.Add(badgeId, condition);
        }

        private void RegisterBadgeAdapterCondition(string badgeId, Func<bool> condition) { this.badgeToAdapterBadgeItem.TryAdd(badgeId, condition); }

        private void RegisParentScreen(UITemplateBadgeNotifyView badgeNotifyView, Type parentScreenType)
        {
            if (parentScreenType == null) return;
            var badgeSet = this.screenTypeToBadges.GetOrAdd(parentScreenType, () => new HashSet<UITemplateBadgeNotifyView>());
            badgeSet.Add(badgeNotifyView);
            if (this.screenTypeToBadgeTemp.ContainsKey(parentScreenType)) this.screenTypeToBadgeTemp.Remove(parentScreenType);
        }

        private void RegisterParentScreenTemp(Type parentScreenType, string badgeId)
        {
            if (parentScreenType == null) return;
            var badgeTempSet = this.screenTypeToBadgeTemp.GetOrAdd(parentScreenType, () => new HashSet<string>());
            badgeTempSet.Add(badgeId);
        }

        private bool GetBadgeStatusFromBadgeView(UITemplateBadgeNotifyView badgeNotifyView)
        {
            if (this.badgeToConditionFunc.TryGetValue(badgeNotifyView, out var conditionFunc))
            {
                return conditionFunc.Invoke();
            }

            return this.screenTypeToBadges.TryGetValue(this.badgeToNextScreenType[badgeNotifyView], out var badgeViews)
                ? badgeViews.Any(this.GetBadgeStatusFromBadgeView)
                : this.screenTypeToBadgeTemp[this.badgeToNextScreenType[badgeNotifyView]].Any(this.GetBadgeStatusFromBadgeId);
        }

        private bool GetBadgeStatusFromBadgeId(string badgeId)
        {
            if (this.badgeToConditionFuncTemp.TryGetValue(badgeId, out var conditionFuncTemp))
            {
                return conditionFuncTemp.Invoke();
            }

            return this.badgeToAdapterBadgeItem[badgeId].Invoke();
        }

        #region BadgeNotifyFunction

        public void RegisterBadge<TPresenter>(UITemplateBadgeNotifyView badgeView, IScreenPresenter parentScreenPresenter)
            where TPresenter : IScreenPresenter
        {
            this.RegisterBadgeNextScreenType(badgeView, parentScreenPresenter, typeof(TPresenter));
        }

        public void RegisterBadge<TPresenter>(UITemplateBadgeNotifyView badgeView, Type parentScreenPresenter)
            where TPresenter : IScreenPresenter
        {
            this.RegisterBadgeNextScreenType(badgeView, parentScreenPresenter, typeof(TPresenter));
        }

        public void RegisterBadge(UITemplateBadgeNotifyView badgeView, IScreenPresenter parentScreenPresenter, Func<bool> condition, string badgeId = null)
        {
            this.RegisterBadgeCondition(badgeView, parentScreenPresenter, condition, badgeId);
        }

        public void RegisterBadge(Type parentScreenType, Func<bool> condition, string badgeId = null) { this.RegisterBadgeConditionTemp(parentScreenType, condition, badgeId); }

        public void RegisterBadgeAdapter(string badgeId, Func<bool> condition) { this.RegisterBadgeAdapterCondition(badgeId, condition); }

        private void RefreshBadgeStatus(UITemplateBadgeNotifyView badgeView) { badgeView.badge.SetActive(this.GetBadgeStatusFromBadgeView(badgeView)); }

        public void RefreshBadgeStatusForAdapter(UITemplateBadgeNotifyView badgeView) { badgeView.badge.SetActive(this.GetBadgeStatusFromBadgeId(badgeView.badgeId)); }

        #endregion

        public void CheckAllBadgeNotifyStatus(bool force = true)
        {
            var currentScreenPresenter = this.screenManager.CurrentActiveScreen.Value;

            if (!force && currentScreenPresenter.Equals(this.currentPresenter)) return;
            this.currentPresenter = currentScreenPresenter;
            var badgeToNextScreen = this.badgeToNextScreenType.FirstOrDefault(badge => badge.Value == this.currentPresenter.GetType()).Key;
            if (badgeToNextScreen != null) this.RefreshBadgeStatus(badgeToNextScreen);

            if (!this.screenTypeToBadges.TryGetValue(currentScreenPresenter.GetType(), out var badgeNotifyButtonViews)) return;
            badgeNotifyButtonViews.ForEach(this.RefreshBadgeStatus);
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