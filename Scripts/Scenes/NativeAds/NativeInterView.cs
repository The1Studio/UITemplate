#if ADMOB_NATIVE_ADS
namespace TheOneStudio.UITemplate.UITemplate.Scenes.NativeAds
{
    using Core.AdsServices.Native;
    using DG.Tweening;
    using GameFoundation.DI;
    using GameFoundation.Signals;
    using ServiceImplementation.Configs.Ads;
    using UnityEngine;
    using TheOneStudio.UITemplate.Scripts.Signals;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using TMPro;
    using UnityEngine.UI;

    public class NativeInterView : MonoBehaviour
    {
        [SerializeField] private GameObject    view;
        [SerializeField] private NativeAdsView nativeAdsView;
        [SerializeField] private Button        btnClose;
        [SerializeField] private GameObject    objTimer;
        [SerializeField] private TMP_Text      txtTimer;
        [SerializeField] private BoxCollider   boxCollider;

        private SignalBus                  signalBus;
        private UITemplateAdServiceWrapper adServiceWrapper;
        private AdServicesConfig           adServicesConfig;
        private INativeAdsService          nativeAdsService;

        private void Awake()
        {
            var container = this.GetCurrentContainer();
            this.signalBus        = container.Resolve<SignalBus>();
            this.adServiceWrapper = container.Resolve<UITemplateAdServiceWrapper>();
            this.adServicesConfig = container.Resolve<AdServicesConfig>();
            this.nativeAdsService = container.Resolve<INativeAdsService>();

            this.view.SetActive(false);
            this.btnClose.gameObject.SetActive(false);
            this.objTimer.SetActive(false);
            this.btnClose.onClick.AddListener(this.OnCloseView);

            this.signalBus.Subscribe<ShowNativeInterAdsSignal>(this.OnShowNativeInterAdsSignal);
        }

        private void OnDestroy()
        {
            this.signalBus.Unsubscribe<ShowNativeInterAdsSignal>(this.OnShowNativeInterAdsSignal);
        }

        private ShowNativeInterAdsSignal signal;

        private void OnShowNativeInterAdsSignal(ShowNativeInterAdsSignal signal)
        {
            this.signal = signal;
            this.view.SetActive(true);
            this.ChangeButtonState(false);
            this.StartCountDown();
            this.nativeAdsView.Init(this.nativeAdsService);
        }

        private void StartCountDown()
        {
            this.txtTimer.DOCounter(this.adServicesConfig.NativeInterCountdown, 1, this.adServicesConfig.NativeInterCountdown).SetUpdate(true).SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    this.ChangeButtonState(true);
                });
        }

        private void OnCloseView()
        {
            this.view.SetActive(false);
            this.nativeAdsView.Release();
            this.signal.OnComplete?.Invoke(false);
            this.adServiceWrapper.LastTimeShowNativeInterAd = Time.time;
        }

        private void ChangeButtonState(bool isActive)
        {
            this.btnClose.gameObject.SetActive(isActive);
            this.objTimer.SetActive(!isActive);
            this.boxCollider.enabled = isActive;
        }
    }
}
#endif