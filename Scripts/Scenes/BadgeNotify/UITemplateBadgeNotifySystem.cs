namespace TheOneStudio.UITemplate.UITemplate.Scenes.BadgeNotify
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.Utilities.Extension;

    public class UITemplateBadgeNotifySystem
    {
        private Dictionary<Type, HashSet<UITemplateBadgeNotifyButtonView>> screenTypeToBadgeButtons    = new();
        private Dictionary<UITemplateBadgeNotifyButtonView, Type>          badgeButtonToNextScreenType = new();
        private Dictionary<UITemplateBadgeNotifyButtonView, Func<bool>>    badgeButtonToConditionFunc  = new();

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
    }
}