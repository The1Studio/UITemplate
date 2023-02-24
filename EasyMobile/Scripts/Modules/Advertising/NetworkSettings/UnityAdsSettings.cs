using System;
using System.Collections.Generic;
using UnityEngine;
using EasyMobile.Internal;

namespace EasyMobile
{
    [Serializable]
    public class UnityAdsSettings : AdNetworkSettings
    {
        /// <summary>
        /// Gets or sets the Unity Monetization app identifier.
        /// </summary>
        public AdId AppId
        {
            get { return mAppId; }
            set { mAppId = value; }
        }

        /// <summary>
        /// Gets or sets the default banner ad identifier.
        /// </summary>
        public AdId DefaultBannerAdId
        {
            get { return mDefaultBannerAdId; }
            set { mDefaultBannerAdId = value; }
        }

        /// <summary>
        /// Gets or sets the default interstitial ad identifier.
        /// </summary>
        public AdId DefaultInterstitialAdId
        {
            get { return mDefaultInterstitialAdId; }
            set { mDefaultInterstitialAdId = value; }
        }

        /// <summary>
        /// Gets or sets the default rewarded ad identifier.
        /// </summary>
        public AdId DefaultRewardedAdId
        {
            get { return mDefaultRewardedAdId; }
            set { mDefaultRewardedAdId = value; }
        }

        /// <summary>
        /// Gets or sets the list of custom banner identifiers.
        /// Each identifier is associated with an ad placement.
        /// </summary>
        public Dictionary<AdPlacement, AdId> CustomBannerAdIds
        {
            get { return mCustomBannerAdIds; }
            set { mCustomBannerAdIds = value as Dictionary_AdPlacement_AdId; }
        }

        /// <summary>
        /// Gets or sets the list of custom interstitial ad identifiers.
        /// Each identifier is associated with an ad placement.
        /// </summary>
        public Dictionary<AdPlacement, AdId> CustomInterstitialAdIds
        {
            get { return mCustomInterstitialAdIds; }
            set { mCustomInterstitialAdIds = value as Dictionary_AdPlacement_AdId; }
        }

        /// <summary>
        /// Gets or sets the list of custom rewarded ad identifiers.
        /// Each identifier is associated with an ad placement.
        /// </summary>
        public Dictionary<AdPlacement, AdId> CustomRewardedAdIds
        {
            get { return mCustomRewardedAdIds; }
            set { mCustomRewardedAdIds = value as Dictionary_AdPlacement_AdId; }
        }

        /// <summary>
        /// Enables or disables test mode.
        /// </summary>
        public bool EnableTestMode
        {
            get { return mEnableTestMode; }
            set { mEnableTestMode = value; }
        }

        /// <summary>
        /// Enables unity ad 4 client implementation or legacy implementation
        /// </summary>
        public bool UseUnityAd4Client
        {
            get {return mUseUnityAd4Client; }
            set {mUseUnityAd4Client = value;}
        }
        
        public const string DEFAULT_INTERSTITIAL_ID_ANDROID = "Interstitial_Android";
        public const string DEFAULT_INTERSTITIAL_ID_IOS = "Interstitial_iOS";
        public const string DEFAULT_REWARDED_ID_ANDROID = "Rewarded_Android";
        public const string DEFAULT_REWARDED_ID_IOS = "Rewarded_iOS";
        public const string DEFAULT_BANNER_ID_ANDROID = "Banner_Android";
        public const string DEFAULT_BANNER_ID_IOS = "Banner_iOS";

        [SerializeField]
        private AdId mAppId;
        [SerializeField]
        private AdId mDefaultBannerAdId;
        [SerializeField]
        private AdId mDefaultInterstitialAdId;
        [SerializeField]
        private AdId mDefaultRewardedAdId;
        [SerializeField]
        private Dictionary_AdPlacement_AdId mCustomBannerAdIds;
        [SerializeField]
        private Dictionary_AdPlacement_AdId mCustomInterstitialAdIds;
        [SerializeField]
        private Dictionary_AdPlacement_AdId mCustomRewardedAdIds;
        [SerializeField]
        private bool mEnableTestMode;
        [SerializeField]
        private bool mUseUnityAd4Client;

        public UnityAdsSettings()
        {
            mDefaultBannerAdId = new AdId(DEFAULT_BANNER_ID_IOS, DEFAULT_BANNER_ID_ANDROID);
            mDefaultInterstitialAdId = new AdId(DEFAULT_INTERSTITIAL_ID_IOS, DEFAULT_INTERSTITIAL_ID_ANDROID);
            mDefaultRewardedAdId = new AdId(DEFAULT_REWARDED_ID_IOS, DEFAULT_REWARDED_ID_ANDROID);
        }
    }
}
