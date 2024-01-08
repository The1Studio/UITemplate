namespace TheOneStudio.UITemplate.UITemplate.Scenes.BadgeNotify
{
    using System;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using TheOneStudio.UITemplate.UITemplate.Extension;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateBadgeNotifyButtonView : MonoBehaviour
    {
        #region inject

        private UITemplateBadgeNotifySystem uiTemplateBadgeNotifySystem;
        private IScreenManager              screenManager;
        private UITemplateAdServiceWrapper  uitemplateAdServiceWrapper;

        #endregion

        [SerializeField] private Button     badgeButton;
        [SerializeField] private GameObject badge;

        [Inject]
        private void Construct(UITemplateBadgeNotifySystem uiTemplateBadgeNotifySystem, IScreenManager screenManager, UITemplateAdServiceWrapper uiTemplateAdServiceWrapper)
        {
            if (this.uiTemplateBadgeNotifySystem != null) return;

            this.uiTemplateBadgeNotifySystem = uiTemplateBadgeNotifySystem;
            this.screenManager               = screenManager;
            this.uitemplateAdServiceWrapper  = uiTemplateAdServiceWrapper;
        }

        public void BindData()
        {
            this.badge.SetActive(this.uiTemplateBadgeNotifySystem.GetBadgeStatus(this));
        }

        public void Register<T>(IScreenPresenter parentScreenPresenter, string interPlacement = null) where T : IScreenPresenter
        {
            if (interPlacement.IsNullOrEmpty())
            {
                this.badgeButton.onClick.AddListener(() => this.screenManager.OpenScreen<T>());
            }
            else
            {
                this.badgeButton.onClick.AddListener(() => this.uitemplateAdServiceWrapper.ShowInterstitialAd(interPlacement, _ => this.screenManager.OpenScreen<T>()));
            }

            this.uiTemplateBadgeNotifySystem.RegisterBadge(this, parentScreenPresenter, typeof(T));
        }

        public void Register(IScreenPresenter parentScreenPresenter, UnityAction onClick, Func<bool> condition, string interPlacement = null)
        {
            if (interPlacement.IsNullOrEmpty())
            {
                this.badgeButton.onClick.AddListener(onClick);
            }
            else
            {
                this.badgeButton.onClick.AddListener(() => this.uitemplateAdServiceWrapper.ShowInterstitialAd(interPlacement, _ => onClick.Invoke()));
            }
            this.uiTemplateBadgeNotifySystem.RegisterBadge(this, parentScreenPresenter, condition);
        }

        public void SetActive(bool isActive) { this.badge.SetActive(isActive); }
        
    }
}