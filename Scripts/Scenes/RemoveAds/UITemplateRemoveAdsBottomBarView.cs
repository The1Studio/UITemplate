namespace TheOneStudio.UITemplate.UITemplate.Scenes.RemoveAdsBottomBar
{
    using Cysharp.Threading.Tasks;
    using GameFoundation.DI;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using UnityEngine;
    using UnityEngine.UI;

    public class UITemplateRemoveAdsBottomBarView : MonoBehaviour
    {
        [SerializeField] private GameObject removeAdsObj;
        [SerializeField] private Button     btnRemoveAds;

        #region inject

        protected SignalBus                  signalBus;
        protected IScreenManager             screenManager;
        protected UITemplateAdServiceWrapper adServiceWrapper;

        private void Awake()
        {
            var container = this.GetCurrentContainer();
            this.signalBus        = container.Resolve<SignalBus>();
            this.screenManager    = container.Resolve<IScreenManager>();
            this.adServiceWrapper = container.Resolve<UITemplateAdServiceWrapper>();

            this.signalBus.Subscribe<OnRemoveAdsSucceedSignal>(this.OnRemoveAdsSucceedHandler);
            this.signalBus.Subscribe<UITemplateOnUpdateBannerStateSignal>(this.OnUpdateBannerStateSignal);
            this.btnRemoveAds.onClick.AddListener(this.OnClickRemoveAdsButton);
        }

        private void OnUpdateBannerStateSignal(UITemplateOnUpdateBannerStateSignal obj)
        {
            this.removeAdsObj.SetActive(obj.IsActive);
        }

        #endregion

        private void OnEnable()
        {
            #if THEONE_IAP && !CREATIVE
            this.removeAdsObj.SetActive(!this.adServiceWrapper.IsRemovedAds);
            #else
            this.removeAdsObj.SetActive(false);
            #endif
        }

        protected virtual void OnClickRemoveAdsButton()
        {
            this.screenManager.OpenScreen<UITemplateRemoveAdPopupPresenter>().Forget();
        }

        private void OnRemoveAdsSucceedHandler() { this.removeAdsObj.SetActive(false); }
    }
}