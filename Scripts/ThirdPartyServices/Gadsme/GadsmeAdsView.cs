﻿#if GADSME
namespace Core.AdsServices.Native
{
    using Gadsme;
    using GameFoundation.DI;
    using GameFoundation.Signals;
    using ServiceImplementation.Configs.Ads;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using UnityEngine;

    public class GadsmeAdsView : MonoBehaviour
    {
        [SerializeField] protected GameObject nonAdsHolder;
        [SerializeField] protected GameObject adsHolder;

        public  bool              isCanClick;
        public  int               adsChannelNumber = -1; // -1 means no channel
        public  GadsmePlacement   currentAdsPlacement;
        public  AdServicesConfig  adServicesConfig;
        private INativeAdsService NativeAdsService { get; set; }
        private SignalBus         SignalBus        { get; set; }
        public void OnEnable()
        {
            var container = this.GetCurrentContainer();
            this.NativeAdsService = container.Resolve<INativeAdsService>();
            this.adServicesConfig = container.Resolve<AdServicesConfig>();
            this.SignalBus        = container.Resolve<SignalBus>();

            this.SignalBus.Subscribe<OnRemoveAdsSucceedSignal>(this.OnRemoveAds );
            if (this.currentAdsPlacement == null)
            {
                this.currentAdsPlacement = this.adsHolder.GetComponentInChildren<GadsmePlacement>();
            }
            this.currentAdsPlacement.clickInteraction = this.isCanClick;
            this.currentAdsPlacement.adChannelNumber  = this.adsChannelNumber;
            this.ShowAds(this.adServicesConfig.EnableGadsme);
        }
        public void OnRemoveAds ()
        {
            this.ShowAds(false);
        }
        public void ShowAds(bool isShow)
        {
            this.nonAdsHolder.SetActive(!isShow);
            this.adsHolder.SetActive(isShow);
        }
        private void OnDisable()
        {
            this.ShowAds(false);
            this.SignalBus.Unsubscribe<OnRemoveAdsSucceedSignal>(this.OnRemoveAds );
        }
    }
}
#endif