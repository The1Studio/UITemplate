namespace Core.AdsServices.Native
{
    using System;
    using Gadsme;
    using GameFoundation.DI;
    using ServiceImplementation.Configs.Ads;
    using UnityEngine;

    public class GadsmeAdsView : MonoBehaviour
    {
        [SerializeField] protected GameObject nonAdsHolder;
        [SerializeField] protected GameObject adsHolder;

        public bool             isCanClick;
        public int              adsChannelNumber = -1; // -1 means no channel
        public GadsmePlacement  currentAdsPlacement;
        public AdServicesConfig adServicesConfig;
        private INativeAdsService nativeAdsService { get; set; }
        public void OnEnable()
        {
            this.nativeAdsService = this.GetCurrentContainer().Resolve<INativeAdsService>();
            this.adServicesConfig = this.GetCurrentContainer().Resolve<AdServicesConfig>();
            
            this.nativeAdsService.OnRemoveAds += this.RemoveAds;
            if (this.currentAdsPlacement == null)
            {
                this.currentAdsPlacement = this.adsHolder.GetComponentInChildren<GadsmePlacement>();
            }
            this.currentAdsPlacement.clickInteraction = this.isCanClick;
            this.currentAdsPlacement.adChannelNumber  = this.adsChannelNumber;
            this.ShowAds(this.adServicesConfig.EnableGadsme);
        }

        public void   DrawNativeAds(NativeAdsView admobNativeAdsView) {}
        public void RemoveAds()
        {
            Debug.Log("nhh : remove ads ");
            this.ShowAds(false);
        }
        public bool IsRemoveAds()
        {
            return PlayerPrefs.HasKey("GADSME_REMOVE_ADS");
        }
        public void ShowAds(bool isShow)
        {
            var enable = isShow && !this.IsRemoveAds();
            this.nonAdsHolder.SetActive(!enable);
            this.adsHolder.SetActive(enable);
        }
        private void OnDisable()
        {
            this.nativeAdsService.OnRemoveAds -= this.RemoveAds;
        }
        private void OnDestroy()
        {
            this.ShowAds(false);
        }
    }
}