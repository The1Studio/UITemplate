namespace TheOneStudio.UITemplate.UITemplate.Scenes.BadgeNotify
{
    using System.Collections.Generic;

    public class UITemplateBadgeNotifySystem
    {
        private Dictionary<string, UItemplateBadgeNotifyView> badgeKeyToView      = new();
        private Dictionary<string, string>                    badgeKeyToParentKey = new();

        public void RegisterBadge(UItemplateBadgeNotifyView badgeNotifyView, string key, string parentBadgeNotify)
        {
            this.badgeKeyToView.Add(key, badgeNotifyView);
            this.badgeKeyToParentKey.Add(key, parentBadgeNotify);
        }

        public void UpdateBadgeNotifyStatus()
        {
        }
        
        
    }
}