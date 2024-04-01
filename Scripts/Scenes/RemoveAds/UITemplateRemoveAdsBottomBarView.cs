namespace TheOneStudio.UITemplate.UITemplate.Scenes.RemoveAdsBottomBar
{
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateRemoveAdsBottomBarView : MonoBehaviour
    {
        [SerializeField] private GameObject removeAdsObj;
        [SerializeField] private Button     btnRemoveAds;

        #region inject

        private SignalBus                  signalBus;
        private IScreenManager             screenManager;
        private UITemplateAdServiceWrapper adServiceWrapper;

        [Inject]
        private void Init(SignalBus signal, IScreenManager screen, UITemplateAdServiceWrapper adService)
        {
            this.signalBus        = signal;
            this.screenManager    = screen;
            this.adServiceWrapper = adService;
            this.signalBus.Subscribe<OnRemoveAdsSucceedSignal>(this.OnRemoveAdsHandler);
            this.signalBus.Subscribe<UITemplateOnUpdateBannerStateSignal>(this.OnUpdateBannerStateSignal);
            this.btnRemoveAds.onClick.AddListener(this.OpenRemoveAdsPopup);
        } 
        private void OnUpdateBannerStateSignal(UITemplateOnUpdateBannerStateSignal obj)
        {
            this.removeAdsObj.SetActive(obj.IsActive);
        }

        #endregion

        private void OnEnable()
        {
#if THEONE_IAP
            this.removeAdsObj.SetActive(!this.adServiceWrapper.IsRemovedAds);
#else
            this.removeAdsObj.SetActive(false);
#endif
        }

        private void OpenRemoveAdsPopup()
        {
            this.screenManager.OpenScreen<UITemplateRemoveAdPopupPresenter>().Forget();
        }

        private void OnRemoveAdsHandler() { this.removeAdsObj.SetActive(false); }
    }
}