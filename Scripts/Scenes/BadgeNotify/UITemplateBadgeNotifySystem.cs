namespace TheOneStudio.UITemplate.UITemplate.Scenes.BadgeNotify
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Signals;
    using GameFoundation.Scripts.Utilities.Extension;
    using TheOneStudio.UITemplate.UITemplate.Extension;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using UnityEngine.Events;
    using Zenject;

    public class UITemplateBadgeNotifySystem : IInitializable

    {
        #region inject

        private readonly IScreenManager             screenManager;
        private readonly UITemplateAdServiceWrapper uiTemplateAdServiceWrapper;
        private readonly SignalBus                  signalBus;

        #endregion

        private Dictionary<Type, HashSet<UITemplateBadgeNotifyButtonView>> screenTypeToBadgeButtons    = new();
        private Dictionary<UITemplateBadgeNotifyButtonView, Type>          badgeButtonToNextScreenType = new();
        private Dictionary<UITemplateBadgeNotifyButtonView, Func<bool>>    badgeButtonToConditionFunc  = new();

        public UITemplateBadgeNotifySystem(IScreenManager screenManager, UITemplateAdServiceWrapper uiTemplateAdServiceWrapper, SignalBus signalBus)
        {
            this.screenManager              = screenManager;
            this.uiTemplateAdServiceWrapper = uiTemplateAdServiceWrapper;
            this.signalBus                  = signalBus;
        }

        public void RegisterBadge(UITemplateBadgeNotifyButtonView badgeNotifyButtonView, IScreenPresenter parentScreen, Type nextScreenType)
        {
            this.RegisParentScreen(badgeNotifyButtonView, parentScreen);

            this.badgeButtonToNextScreenType.Add(badgeNotifyButtonView, nextScreenType);
        }

        private void RegisParentScreen(UITemplateBadgeNotifyButtonView badgeNotifyButtonView, IScreenPresenter parentScreen)
        {
            if (parentScreen == null) return;

            var badgeSet = this.screenTypeToBadgeButtons.GetOrAdd(parentScreen.GetType(), () => new HashSet<UITemplateBadgeNotifyButtonView>());
            badgeSet.Add(badgeNotifyButtonView);
        }

        public void RegisterBadge(UITemplateBadgeNotifyButtonView badgeNotifyButtonView, IScreenPresenter parentScreen, Func<bool> condition)
        {
            this.RegisParentScreen(badgeNotifyButtonView, parentScreen);

            this.badgeButtonToConditionFunc.Add(badgeNotifyButtonView, condition);
        }

        public bool GetBadgeStatus(UITemplateBadgeNotifyButtonView badgeNotifyButtonView)
        {
            if (this.badgeButtonToConditionFunc.TryGetValue(badgeNotifyButtonView, out var conditionFunc))
            {
                return conditionFunc.Invoke();
            }

            return this.screenTypeToBadgeButtons[this.badgeButtonToNextScreenType[badgeNotifyButtonView]].Any(this.GetBadgeStatus);
        }

        #region BadgeNotifyFunction

        public void RegisterButton<TPresenter>(UITemplateBadgeNotifyButtonView badgeButtonView, IScreenPresenter parentScreenPresenter, string interPlacement = null) where TPresenter : IScreenPresenter
        {
            if (interPlacement.IsNullOrEmpty())
            {
                badgeButtonView.badgeButton.onClick.AddListener(() => this.screenManager.OpenScreen<TPresenter>());
            }
            else
            {
                badgeButtonView.badgeButton.onClick.AddListener(() => this.uiTemplateAdServiceWrapper.ShowInterstitialAd(interPlacement, _ => this.screenManager.OpenScreen<TPresenter>()));
            }

            this.RegisterBadge(badgeButtonView, parentScreenPresenter, typeof(TPresenter));
        }

        public void RegisterButton(UITemplateBadgeNotifyButtonView badgeButtonView, IScreenPresenter parentScreenPresenter, UnityAction onClick, Func<bool> condition, string interPlacement = null)
        {
            if (interPlacement.IsNullOrEmpty() && onClick != null)
            {
                badgeButtonView.badgeButton.onClick.AddListener(onClick);
            }
            else
            {
                badgeButtonView.badgeButton.onClick.AddListener(() => this.uiTemplateAdServiceWrapper.ShowInterstitialAd(interPlacement, _ => onClick?.Invoke()));
            }
            this.RegisterBadge(badgeButtonView, parentScreenPresenter, condition);
        }

        private void SetActiveBadge(UITemplateBadgeNotifyButtonView badgeButtonView) { badgeButtonView.badge.SetActive(this.GetBadgeStatus(badgeButtonView)); }

        #endregion

        public void CheckAllBadgeNotifyStatus()
        {
            var currentScreenPresenter = this.screenManager.CurrentActiveScreen.Value;
            if (this.screenTypeToBadgeButtons.TryGetValue(currentScreenPresenter.GetType(), out var badgeNotifyButtonViews))
            {
                badgeNotifyButtonViews.ForEach(this.SetActiveBadge);
            }
        }

        public void Initialize()
        {
            this.signalBus.Subscribe<ScreenShowSignal>(this.CheckAllBadgeNotifyStatus);
            this.signalBus.Subscribe<ScreenHideSignal>(this.CheckAllBadgeNotifyStatus);
            this.signalBus.Subscribe<ScreenCloseSignal>(this.CheckAllBadgeNotifyStatus);
        }
    }
}