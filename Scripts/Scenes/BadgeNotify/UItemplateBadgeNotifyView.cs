namespace TheOneStudio.UITemplate.UITemplate.Scenes.BadgeNotify
{
    using UnityEngine;
    using Zenject;

    public class UItemplateBadgeNotifyView : MonoBehaviour
    {
        #region inject

        private UITemplateBadgeNotifySystem uiTemplateBadgeNotifySystem;

        #endregion

        [SerializeField] private GameObject badge;
        [SerializeField] private string     key;

        [Inject]
        private void Construct(UITemplateBadgeNotifySystem uiTemplateBadgeNotifySystem)
        {
            if (this.uiTemplateBadgeNotifySystem != null) return;

            this.uiTemplateBadgeNotifySystem = uiTemplateBadgeNotifySystem;
        }

        public void SetActive(bool isActive) { this.badge.SetActive(isActive); }

        public void Register(string parentKey) { this.uiTemplateBadgeNotifySystem.RegisterBadge(this, this.key, null); }
        
        public bool GetStatus() => this.badge.activeSelf;
    }
}