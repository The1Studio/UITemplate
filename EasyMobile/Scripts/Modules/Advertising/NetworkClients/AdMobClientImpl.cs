using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace EasyMobile
{
#if EM_ADMOB
    using GoogleMobileAds;
    using GoogleMobileAds.Api;
    using EasyMobile.Internal;
#endif
    using System.Text;
    public class AdMobClientImpl : AdClientImpl
    {
        private const string NO_SDK_MESSAGE = "SDK missing. Please import the AdMob (Google Mobile Ads) plugin.";

        #region Singleton

        private static AdMobClientImpl sInstance;

        private AdMobClientImpl()
        {
        }
        /// <summary>
        /// Returns the singleton client.
        /// </summary>
        /// <returns>The client.</returns>
        public static AdMobClientImpl CreateClient()
        {
            if (sInstance == null)
            {
                sInstance = new AdMobClientImpl();
            }
            return sInstance;
        }
        #endregion  // Singleton

        private Banner banner = new Banner(mAdSettings);
        private Rewarded rewarded = new Rewarded(mAdSettings);
        private Interstitial interstitial = new Interstitial(mAdSettings);
        private RewardedInterstitial rewardedInterstitial = new RewardedInterstitial(mAdSettings);

        public Banner BannerAdProvider {get {return banner;}}
        public Rewarded RewardedAdProvider {get {return rewarded;}}
        public Interstitial InterstitialAdProvider {get {return interstitial;}}
        public RewardedInterstitial RewardedInterstitialAdProvider {get {return rewardedInterstitial;}}

        #region AdMob Events

#if EM_ADMOB

        /// <summary>
        /// Called when a banner ad request has successfully loaded.
        /// </summary>
		public event EventHandler<EventArgs> OnBannerAdLoaded;
		private void InvokeBannerAdLoaded(AdInstance instance, object sender, EventArgs args)
		{if(OnBannerAdLoaded != null)OnBannerAdLoaded.Invoke(sender, args);}

        /// <summary>
        /// Called when a banner ad request failed to load.
        /// </summary>
		public event EventHandler<AdFailedToLoadEventArgs> OnBannerAdFailedToLoad;
		private void InvokeBannerAdFailedToLoad(AdInstance instance, object sender, AdFailedToLoadEventArgs args)
		{if(OnBannerAdFailedToLoad != null)OnBannerAdFailedToLoad.Invoke(sender, args);}

        /// <summary>
        /// Called when a banner ad is clicked.
        /// </summary>
		public event EventHandler<EventArgs> OnBannerAdOpening;
		private void InvokeBannerAdOpening(AdInstance instance, object sender, EventArgs args)
		{if(OnBannerAdOpening != null)OnBannerAdOpening.Invoke(sender, args);}

        /// <summary>
        /// Called when the user returned from the app after a banner ad click.
        /// </summary>
		public event EventHandler<EventArgs> OnBannerAdClosed;
		private void InvokeBannerAdClosed(AdInstance instance, object sender, EventArgs args)
		{if(OnBannerAdClosed != null)OnBannerAdClosed.Invoke(sender, args);}

        /// <summary>
        /// Called when a banner ad click caused the user to leave the application.
        /// </summary>
        [Obsolete("Admob has removed all AdLeavingApplication events")]
		public event EventHandler<EventArgs> OnBannerAdLeavingApplication;
		private void InvokeBannerAdLeavingApplication(AdInstance instance, object sender, EventArgs args)
		{if(OnBannerAdLeavingApplication != null)OnBannerAdLeavingApplication.Invoke(sender, args);}

        /// <summary>
        /// Called when an interstitial ad request has successfully loaded.
        /// </summary>
		public event EventHandler<EventArgs> OnInterstitialAdLoaded;
		private void InvokeInterstitialAdLoaded(AdInstance instance, object sender, EventArgs args)
		{if(OnInterstitialAdLoaded != null)OnInterstitialAdLoaded.Invoke(sender, args);}

        /// <summary>
        /// Called when an interstitial ad request failed to load.
        /// </summary>
		public event EventHandler<AdFailedToLoadEventArgs> OnInterstitialAdFailedToLoad;
		private void InvokeInterstitialAdFailedToLoad(AdInstance instance, object sender, AdFailedToLoadEventArgs args)
		{if(OnInterstitialAdFailedToLoad != null)OnInterstitialAdFailedToLoad.Invoke(sender, args);}

        /// <summary>
        /// Called when an interstitial ad is shown.
        /// </summary>
		public event EventHandler<EventArgs> OnInterstititalAdOpening;
		private void InvokeInterstititalAdOpening(AdInstance instance, object sender, EventArgs args)
		{if(OnInterstititalAdOpening != null)OnInterstititalAdOpening.Invoke(sender, args);}

        /// <summary>
        /// Called when an interstitital ad is closed.
        /// </summary>
		public event EventHandler<EventArgs> OnInterstitialAdClosed;
		private void InvokeInterstitialAdClosed(AdInstance instance, object sender, EventArgs args)
		{if(OnInterstitialAdClosed != null)OnInterstitialAdClosed.Invoke(sender, args);}

        /// <summary>
        /// Called when an interstitial ad click caused the user to leave the application.
        /// </summary>
        [Obsolete("Admob has removed all AdLeavingApplication events")]
		public event EventHandler<EventArgs> OnInterstitialAdLeavingApplication;
		private void InvokeInterstitialAdLeavingApplication(AdInstance instance, object sender, EventArgs args)
		{if(OnInterstitialAdLeavingApplication != null)OnInterstitialAdLeavingApplication.Invoke(sender, args);}

        /// <summary>
        /// Called when a rewarded video ad request has successfully loaded.
        /// </summary>
		public event EventHandler<EventArgs> OnRewardedAdLoaded;
		private void InvokeRewardedAdLoaded(AdInstance instance, object sender, EventArgs args)
		{if(OnRewardedAdLoaded != null)OnRewardedAdLoaded.Invoke(sender, args);}

        /// <summary>
        /// Called when a rewarded video ad request failed to load.
        /// </summary>
		public event EventHandler<AdFailedToLoadEventArgs> OnRewardedAdFailedToLoad;
		private void InvokeRewardedAdFailedToLoad(AdInstance instance, object sender, AdFailedToLoadEventArgs args)
		{if(OnRewardedAdFailedToLoad != null)OnRewardedAdFailedToLoad.Invoke(sender, args);}

        /// <summary>
        /// Called when a rewarded video ad request failed to show.
        /// </summary>
		public event EventHandler<AdErrorEventArgs> OnRewardedAdFailedToShow;
		private void InvokeRewardedAdFailedToShow(AdInstance instance, object sender, AdErrorEventArgs args)
		{if(OnRewardedAdFailedToShow != null)OnRewardedAdFailedToShow.Invoke(sender, args);}

        /// <summary>
        /// Called when a rewarded video ad is shown.
        /// </summary>
		public event EventHandler<EventArgs> OnRewardedAdOpening;
		private void InvokeRewardedAdOpening(AdInstance instance, object sender, EventArgs args)
		{if(OnRewardedAdOpening != null)OnRewardedAdOpening.Invoke(sender, args);}

        /// <summary>
        /// Called when a rewarded video ad starts to play.
        /// </summary>
        [Obsolete("Admob has removed this event")]
		public event EventHandler<EventArgs> OnRewardedAdStarted;
		private void InvokeRewardedAdStarted(AdInstance instance, object sender, EventArgs args)
		{if(OnRewardedAdStarted != null)OnRewardedAdStarted.Invoke(sender, args);}

        /// <summary>
        /// Called when the user should be rewarded for watching a video.
        /// </summary>
		public event EventHandler<Reward> OnRewardedAdRewarded;
		private void InvokeRewardedAdRewarded(AdInstance instance, object sender, Reward reward)
		{if(OnRewardedAdRewarded != null)OnRewardedAdRewarded.Invoke(sender, reward);}

        /// <summary>
        /// Called when a rewarded video ad is closed.
        /// </summary>
		public event EventHandler<EventArgs> OnRewardedAdClosed;
		private void InvokeRewardedAdClosed(AdInstance instance, object sender, EventArgs args)
		{if(OnRewardedAdClosed != null)OnRewardedAdClosed.Invoke(sender, args);}

        /// <summary>
        /// Called when a rewarded video ad click caused the user to leave the application.
        /// </summary>
        [Obsolete("Admob has removed all AdLeavingApplication events")]
		public event EventHandler<EventArgs> OnRewardedAdLeavingApplication;
		private void InvokeRewardedAdLeavingApplication(AdInstance instance, object sender, EventArgs args)
		{if(OnRewardedAdLeavingApplication != null)OnRewardedAdLeavingApplication.Invoke(sender, args);}

        /// <summary>
        /// Called when a rewarded video ad request has successfully loaded.
        /// </summary>
		public event EventHandler<EventArgs> OnInterstitialRewardedAdLoaded;
		private void InvokeInterstitialRewardedAdLoaded(AdInstance instance)
		{if(OnInterstitialRewardedAdLoaded != null)OnInterstitialRewardedAdLoaded.Invoke(null, null);}

        /// <summary>
        /// Called when a rewarded video ad request failed to load.
        /// </summary>
		public event EventHandler<AdFailedToLoadEventArgs> OnInterstitialRewardedAdFailedToLoad;
		private void InvokeInterstitialRewardedAdFailedToLoad(AdInstance instance, AdFailedToLoadEventArgs args)
		{if(OnInterstitialRewardedAdFailedToLoad != null)OnInterstitialRewardedAdFailedToLoad.Invoke(null, args);}

        /// <summary>
        /// Called when a rewarded video ad request failed to show.
        /// </summary>
		public event EventHandler<AdErrorEventArgs> OnInterstitialRewardedAdFailedToShow;
		private void InvokeInterstitialRewardedAdFailedToShow(AdInstance instance, object sender, AdErrorEventArgs args)
		{if(OnInterstitialRewardedAdFailedToShow != null)OnInterstitialRewardedAdFailedToShow.Invoke(sender, args);}

        /// <summary>
        /// Called when a rewarded video ad is shown.
        /// </summary>
		public event EventHandler<EventArgs> OnInterstitialRewardedAdOpening;
		private void InvokeInterstitialRewardedAdOpening(AdInstance instance, object sender, EventArgs args)
		{if(OnInterstitialRewardedAdOpening != null)OnInterstitialRewardedAdOpening.Invoke(sender, args);}

        /// <summary>
        /// Called when a rewarded video ad starts to play.
        /// </summary>
        [Obsolete("Admob has removed this event")]
		public event EventHandler<EventArgs> OnInterstitialRewardedAdStarted;
		private void InvokeInterstitialRewardedAdStarted(AdInstance instance, object sender, EventArgs args)
		{if(OnInterstitialRewardedAdStarted != null)OnInterstitialRewardedAdStarted.Invoke(sender, args);}

        /// <summary>
        /// Called when the user should be rewarded for watching a video.
        /// </summary>
		public event EventHandler<Reward> OnInterstitialRewardedAdRewarded;
		private void InvokeInterstitialRewardedAdRewarded(AdInstance instance, Reward reward)
		{if(OnInterstitialRewardedAdRewarded != null)OnInterstitialRewardedAdRewarded.Invoke(null, reward);}

        /// <summary>
        /// Called when a rewarded video ad is closed.
        /// </summary>
		public event EventHandler<EventArgs> OnInterstitialRewardedAdClosed;
		private void InvokeInterstitialRewardedAdClosed(AdInstance instance, object sender, EventArgs args)
		{if(OnInterstitialRewardedAdClosed != null)OnInterstitialRewardedAdClosed.Invoke(sender, args);}

#endif

        #endregion  // AdMob Events

        private static AdMobSettings mAdSettings {
            get{
                #if EM_ADMOB
                return EM_Settings.Advertising.AdMob;
                #else
                return null;
                #endif
            }
        }

        public override bool IsSdkAvail
        {
            get{
                #if EM_ADMOB
                    return true;
                #else
                    return false;
                #endif
            }
        }

        public override AdNetwork Network {get {return AdNetwork.AdMob;}}

        public override bool IsBannerAdSupported {get {return true;}}

        public override bool IsInterstitialAdSupported {get {return true;}}

        public override bool IsRewardedAdSupported {get {return true;}}

        protected override string NoSdkMessage { get { return NO_SDK_MESSAGE; } }

        protected override Dictionary<AdPlacement, AdId> CustomInterstitialAdsDict
        {
            get
            {
                if(!IsSdkAvail || mAdSettings == null)
                    return null;
                return mAdSettings.CustomInterstitialAdIds;
            }
        } 

        protected override Dictionary<AdPlacement, AdId> CustomRewardedAdsDict
        {
            get
            {
                if(!IsSdkAvail || mAdSettings == null)
                    return null;
                return mAdSettings.CustomRewardedAdIds;
            }
        }

        protected override Dictionary<AdPlacement, AdId> CustomRewardedInterstitialAdsDict
        {
            get
            {
                if(!IsSdkAvail || mAdSettings == null)
                    return null;
                return mAdSettings.CustomRewardedInterstitialAdIds;
            }
        }

        private const string DATA_PRIVACY_CONSENT_KEY = "EM_Ads_AdMob_DataPrivacyConsent";
        protected override string DataPrivacyConsentSaveKey { get { return DATA_PRIVACY_CONSENT_KEY; } }

        public override bool IsValidPlacement(AdPlacement placement, AdType type)
        {
            string id = "";
            switch (type)
            {
                case AdType.Banner:
                    id = banner.PlacementToId(placement);
                    break;
                case AdType.Interstitial:
                    id = interstitial.PlacementToId(placement);
                    break;
                case AdType.Rewarded:
                    id = rewarded.PlacementToId(placement);
                    break;
                case AdType.RewardedInterstitial:
                    id = rewardedInterstitial.PlacementToId(placement);
                    break;
            }
            return string.IsNullOrEmpty(id) == false;
        }

        protected override void ApplyDataPrivacyConsent(ConsentStatus consent)
        {
            if(!IsSdkAvail)
                return;
            banner.SetConsent(consent);
            interstitial.SetConsent(consent);
            rewarded.SetConsent(consent);
        }

        protected override void InternalInit()
        {
            #if EM_ADMOB
            // Set GDPR consent if any.
            ConsentStatus consent = GetApplicableDataPrivacyConsent();
            ApplyDataPrivacyConsent(consent);
            MobileAds.Initialize(status =>{
                RuntimeHelper.RunOnMainThread(()=>{
                    RequestConfiguration.Builder configurationBuilder = new RequestConfiguration.Builder();
                    // Child-directed.
                    configurationBuilder.SetTagForChildDirectedTreatment(Utils.Convert(mAdSettings.TargetingSettings.TagForChildDirectedTreatment));
                    // Test mode.
                    if (mAdSettings.EnableTestMode)
                    {
                        // Add all emulators
                        List<String> testDevicesIds = new List<string>();
                        testDevicesIds.Add(AdRequest.TestDeviceSimulator);

                        // Add user-specified test devices
                        string testDeviceId = Utils.GetTestDeviceId();
                        if (!String.IsNullOrEmpty(testDeviceId))
                            testDevicesIds.Add(Util.AutoTrimId(testDeviceId));

                        configurationBuilder.SetTestDeviceIds(testDevicesIds);
                    }
                    MobileAds.SetRequestConfiguration(configurationBuilder.build());
                    mIsInitialized = true;
                    Debug.Log("AdMob client has been initialized.");
                });
            });
            #endif
            interstitial.Completed += HandleInterstitialCompleted;
            rewarded.Completed += HandleRewardedCompleted;
            rewarded.Skipped += HandleRewardedSkipped;

#if EM_ADMOB
#region EventForwarding
            banner.LoadedEvent += InvokeBannerAdLoaded;
            banner.FailToLoadEvent += InvokeBannerAdFailedToLoad;
            banner.OpeningEvent += InvokeBannerAdOpening;
            banner.ClosedEvent += InvokeBannerAdClosed;

            interstitial.AdLoadedEvent += InvokeInterstitialAdLoaded;
            interstitial.FailToLoadEvent += InvokeInterstitialAdFailedToLoad;
            interstitial.AdOpeningEvent += InvokeInterstititalAdOpening;
            interstitial.ClosedEvent += InvokeInterstitialAdClosed;
            
            rewarded.AdLoadedEvent += InvokeRewardedAdLoaded;
            rewarded.FailToLoadEvent += InvokeRewardedAdFailedToLoad;
            rewarded.FailToShowEvent += InvokeRewardedAdFailedToShow;
            rewarded.AdOpeningEvent += InvokeRewardedAdOpening;
            rewarded.RewardEvent += InvokeRewardedAdRewarded;
            rewarded.ClosedEvent += InvokeRewardedAdClosed;

            rewardedInterstitial.AdLoadedEvent += InvokeInterstitialRewardedAdLoaded;
            rewardedInterstitial.FailToLoadEvent += InvokeInterstitialRewardedAdFailedToLoad;
            rewardedInterstitial.FailToShowEvent += InvokeInterstitialRewardedAdFailedToShow;
            rewardedInterstitial.ShownEvent += InvokeInterstitialRewardedAdOpening;
            rewardedInterstitial.RewardEvent += InvokeInterstitialRewardedAdRewarded;
            rewardedInterstitial.ClosedEvent += InvokeInterstitialRewardedAdClosed;
#endregion
#endif

            rewardedInterstitial.Completed += HandleRewardedInterstitialCompleted;
            rewardedInterstitial.Skipped += HandleRewardedInterstitialSkipped;
        }

        private void HandleRewardedInterstitialSkipped(RewardedInterstitial.Instance instance)
        {
            OnRewardedInterstitialAdSkipped(instance.Placement);
        }

        private void HandleRewardedInterstitialCompleted(RewardedInterstitial.Instance instance)
        {
            OnRewardedInterstitialAdCompleted(instance.Placement);
        }

        private void HandleRewardedSkipped(Rewarded.Instance instance)
        {
            OnRewardedAdSkipped(instance.Placement);
        }

        private void HandleRewardedCompleted(Rewarded.Instance instance)
        {
            OnRewardedAdCompleted(instance.Placement);
        }

        private void HandleInterstitialCompleted(Interstitial.Instance instance)
        {
            OnInterstitialAdCompleted(instance.Placement);
        }

        protected override bool InternalIsInterstitialAdReady(AdPlacement placement)
        {
            Interstitial.Instance ad = interstitial.GetAdInstance(placement);
            if(ad == null) return false;
            return ad.IsReady();
        }

        protected override bool InternalIsRewardedAdReady(AdPlacement placement)
        {
            Rewarded.Instance ad = rewarded.GetAdInstance(placement);
            if(ad == null) return false;
            return ad.IsReady();
        }

        protected override void InternalLoadInterstitialAd(AdPlacement placement)
        {
            interstitial.Load(placement);
        }

        protected override void InternalLoadRewardedAd(AdPlacement placement)
        {
            rewarded.Load(placement);
        }

        protected override void InternalShowBannerAd(AdPlacement placement, BannerAdPosition position, BannerAdSize size)
        {
            Banner.Instance ad = banner.Load(placement, position, size);
            if(ad != null)
                ad.Show();
        }

        protected override void InternalDestroyBannerAd(AdPlacement placement)
        {
            Banner.Instance ad = banner.GetAdInstance(placement);
            if(ad != null)
                banner.Destroy(ad);
        }

        protected override void InternalHideBannerAd(AdPlacement placement)
        {
            Banner.Instance ad = banner.GetAdInstance(placement);
            if(ad != null)
                ad.Hide();
        }

        protected override void InternalShowInterstitialAd(AdPlacement placement)
        {
            Interstitial.Instance ad = interstitial.GetAdInstance(placement);
            if(ad != null)
                ad.Show();
        }

        protected override void InternalShowRewardedAd(AdPlacement placement)
        {
            Rewarded.Instance ad = rewarded.GetAdInstance(placement);
            if(ad != null)
                ad.Show();
        }

        protected override void InternalLoadRewardedInterstitialAd(AdPlacement placement)
        {
            rewardedInterstitial.Load(placement);
        }

        protected override bool InternalIsRewardedInterstitialAdReady(AdPlacement placement)
        {
            RewardedInterstitial.Instance ad = rewardedInterstitial.GetAdInstance(placement);
            if(ad == null) return false;
            return ad.IsReady();
        }

        protected override void InternalShowRewardedInterstitialAd(AdPlacement placement)
        {
            RewardedInterstitial.Instance ad = rewardedInterstitial.GetAdInstance(placement);
            if(ad != null)
                ad.Show();
        }

        public class AdInstance
        {
            internal virtual bool IsReady() {return false;}
            internal virtual void Show() {}
            internal virtual void Load() {}
            internal virtual void Destroy() {}
        }
        public class Ad<T> where T:AdInstance
        {
            protected Dictionary<AdPlacement, T> instances = new Dictionary<AdPlacement, T>();
            protected ConsentStatus consentStatus;
            internal virtual string PlacementToId(AdPlacement placement){return "";}
            internal T GetAdInstance(AdPlacement placement)
            {
                if(instances.ContainsKey(placement))
                    return instances[placement];
                return null;
            }
            internal virtual T Load(AdPlacement placement){return null;}
            internal virtual void Destroy(T ad){}
            public void SetConsent(ConsentStatus consent)
            {
                this.consentStatus = consent;
            }
            protected PlacementResult FindInstancePlacement(T instance)
            {
                PlacementResult result = new PlacementResult();
                foreach (KeyValuePair<AdPlacement,T> item in instances)
                {
                    if(item.Value == instance)
                    {
                        result.Placement = item.Key;
                        result.Found = true;
                    }
                }
                return result;
            }
            public struct PlacementResult
            {
                public bool Found;
                public AdPlacement Placement;
            }
        }
        public class Banner: Ad<Banner.Instance>{
            private AdMobSettings mAdSettings;

            public Banner(AdMobSettings mAdSettings)
            {
                this.mAdSettings = mAdSettings;
            }

            internal override string PlacementToId(AdPlacement placement)
            {
                AdId id;
                if(placement == AdPlacement.Default)
                    id = mAdSettings.DefaultBannerAdId;
                else
                    id = Utils.FindIdForPlacement(mAdSettings.CustomBannerAdIds, placement);
                return Utils.GetIdString(id);
            }

            internal override Instance Load(AdPlacement placement)
            {
                return Load(placement, BannerAdPosition.Bottom, BannerAdSize.Banner);
            }
#if EM_ADMOB
            internal event Action<Instance, object, EventArgs> ClosedEvent = delegate {};
            private void InternalInvokeClosedEvent(Instance instance, object obj, EventArgs args){if(ClosedEvent != null) ClosedEvent.Invoke(instance, obj, args);}
            internal event Action<Instance, object, AdFailedToLoadEventArgs> FailToLoadEvent = delegate {};
            private void InternalInvokeFailToLoadEvent(Instance instance, object obj, AdFailedToLoadEventArgs args){if(FailToLoadEvent != null) FailToLoadEvent.Invoke(instance, obj, args); }
            internal event Action<Instance, object, EventArgs> LoadedEvent = delegate {};
            private void InternalInvokeLoadedEvent(Instance instance, object obj, EventArgs args){if(LoadedEvent != null) LoadedEvent.Invoke(instance, obj, args);}
            internal event Action<Instance, object, EventArgs> OpeningEvent = delegate {};
            private void InternalInvokeOpeningEvent(Instance instance, object obj, EventArgs args){if(OpeningEvent != null) OpeningEvent.Invoke(instance, obj, args);}
            internal event Action<Instance, object, AdValueEventArgs> PaidEvent = delegate {};
            private void InternalInvokePaidEvent(Instance instance, object obj, AdValueEventArgs args){if(PaidEvent != null) PaidEvent.Invoke(instance, obj, args);}
#endif

            internal Instance Load(AdPlacement placement, BannerAdPosition position, BannerAdSize size)
            {
                if(instances.ContainsKey(placement))
                    return instances[placement];
                Instance instance = new Instance(
                    PlacementToId(placement), 
                    placement,
                    size, 
                    position,
                    consentStatus);
#if EM_ADMOB
                instance.ClosedEvent        += InternalInvokeClosedEvent       ;
                instance.FailToLoadEvent    += InternalInvokeFailToLoadEvent   ;
                instance.LoadedEvent        += InternalInvokeLoadedEvent       ;
                instance.OpeningEvent       += InternalInvokeOpeningEvent      ;
                instance.PaidEvent          += InternalInvokePaidEvent         ;
#endif
                instance.Load();
                instances.Add(placement, instance);
                return instance;
            }

            internal override void Destroy(Instance ad)
            {
                PlacementResult pResult = FindInstancePlacement(ad);
                if(pResult.Found)
                {
                    instances.Remove(pResult.Placement);
#if EM_ADMOB
                    ad.ClosedEvent        -= InternalInvokeClosedEvent       ;
                    ad.FailToLoadEvent    -= InternalInvokeFailToLoadEvent   ;
                    ad.LoadedEvent        -= InternalInvokeLoadedEvent       ;
                    ad.OpeningEvent       -= InternalInvokeOpeningEvent      ;
                    ad.PaidEvent          -= InternalInvokePaidEvent         ;
#endif
                    ad.Destroy();
                }
            }
        
            public class Instance : AdInstance
            {
                internal AdPlacement Placement {get; private set;}
#if EM_ADMOB
                private ConsentStatus consentStatus;
                private BannerAdPosition position;
                private BannerAdSize size;
                private string id;
#endif

                public Instance(string id, AdPlacement placement, BannerAdSize size, BannerAdPosition position, ConsentStatus consentStatus)
                {
                    Placement = placement;
#if EM_ADMOB
                    this.id = id;
                    this.consentStatus = consentStatus;
                    this.position = position;
                    this.size = size;
#endif
                }
#if !EM_ADMOB
                internal void Hide(){}
#endif
#if EM_ADMOB
                private bool loaded = false;

                internal event Action<Instance, object, EventArgs> ClosedEvent = delegate {};
                internal event Action<Instance, object, AdFailedToLoadEventArgs> FailToLoadEvent = delegate {};
                internal event Action<Instance, object, EventArgs> LoadedEvent = delegate {};
                internal event Action<Instance, object, EventArgs> OpeningEvent = delegate {};
                internal event Action<Instance, object, AdValueEventArgs> PaidEvent = delegate {};
                private Coroutine reloadRoutine;

                private BannerView banner;
                private bool showingState = false;
                internal void Hide()
                {
                    if(banner != null)
                        this.banner.Hide();
                    showingState = false;
                }

                internal override bool IsReady()
                {
                    return loaded;
                }

                internal override void Destroy()
                {
                    if(banner != null)
                    {
                        banner.Destroy();
                        banner = null;
                        if(reloadRoutine != null)
                            RuntimeHelper.StopCoroutineOnMainThread(reloadRoutine);
                    }
                }

                internal override void Load()
                {
                    Destroy();
                    banner = new BannerView(
                        id,
                        Utils.Convert(size),
                        Utils.Convert(position)
                    );
                    this.banner.LoadAd(Utils.CreateAdMobAdRequest(consentStatus));
                    banner.OnAdClosed += HandleClosed;
                    banner.OnAdFailedToLoad += HandleFailToLoad;
                    banner.OnAdLoaded += HandleLoaded;
                    banner.OnAdOpening += HandleOpening;
                    banner.OnPaidEvent += HandlePaidEvent;
                }

                internal override void Show()
                {
                    if(this.banner != null)
                        this.banner.Show();
                    showingState = true;
                }

                private void HandleClosed(object sender, EventArgs e)
                {
                    ClosedEvent.Invoke(this, sender, e);
                }

                private void HandleFailToLoad(object sender, AdFailedToLoadEventArgs e)
                {
                    FailToLoadEvent.Invoke(this, sender, e);

                    // Reload with some cool-down to avoid possible
                    // stackoverflowing issues that may lead to a crash.
                    RuntimeHelper.RunCoroutineOnMainThread(
                        CRLoad(10f),
                        (rt)=> reloadRoutine = rt
                    );// reload after 10s
                }

                private IEnumerator CRLoad(float delay)
                {
                    yield return new WaitForSeconds(delay);
                    Load();
                }

                private void HandleLoaded(object sender, EventArgs e)
                {   
                    if(showingState)
                        Show();
                    else
                        Hide();
                    loaded = true;
                    LoadedEvent.Invoke(this, sender, e);
                }

                private void HandleOpening(object sender, EventArgs e)
                {
                    OpeningEvent.Invoke(this, sender, e);
                }

                private void HandlePaidEvent(object sender, AdValueEventArgs e)
                {
                    PaidEvent.Invoke(this, sender, e);
                }
#endif
            }
        }
        public class Interstitial: Ad<Interstitial.Instance>{
            internal event Action<Instance> Completed = delegate {};
            private void InternalInvokeCompleted(Instance instance){if(Completed != null) Completed.Invoke(instance);}

            private AdMobSettings mAdSettings;

            public Interstitial(AdMobSettings mAdSettings)
            {
                this.mAdSettings = mAdSettings;
            }

            internal override string PlacementToId(AdPlacement placement)
            {
                AdId id;
                if(placement == AdPlacement.Default)
                    id = mAdSettings.DefaultInterstitialAdId;
                else
                    id = Utils.FindIdForPlacement(mAdSettings.CustomInterstitialAdIds, placement);
                return Utils.GetIdString(id);
            }

#if EM_ADMOB
            public event Action<Instance,object, AdValueEventArgs> PaidEvent = delegate {};
            private void InternalInvokePaidEvent(Instance instance, object obj, AdValueEventArgs args){if(PaidEvent != null) PaidEvent.Invoke(instance, obj, args);}
            public event Action<Instance,object, EventArgs> AdOpeningEvent = delegate {};
            private void InternalInvokeAdOpeningEvent(Instance instance, object obj, EventArgs args){if(AdOpeningEvent != null) AdOpeningEvent.Invoke(instance, obj, args);}
            public event Action<Instance,object, EventArgs> AdLoadedEvent = delegate {};
            private void InternalInvokeAdLoadedEvent(Instance instance, object obj, EventArgs args){if(AdLoadedEvent != null) AdLoadedEvent.Invoke(instance, obj, args);}
            public event Action<Instance,object, AdErrorEventArgs> FailToShowEvent = delegate {};
            private void InternalInvokeFailToShowEvent(Instance instance, object obj, AdErrorEventArgs args){if(FailToShowEvent != null) FailToShowEvent.Invoke(instance, obj, args); Destroy(instance);}
            public event Action<Instance,object, AdFailedToLoadEventArgs> FailToLoadEvent = delegate {};
            private void InternalInvokeFailToLoadEvent(Instance instance, object obj, AdFailedToLoadEventArgs args){if(FailToLoadEvent != null) FailToLoadEvent.Invoke(instance, obj, args); Destroy(instance);}
            public event Action<Instance,object, EventArgs> RecordImpressionEvent = delegate {};
            private void InternalInvokeRecordImpressionEvent(Instance instance, object obj, EventArgs args){if(RecordImpressionEvent != null) RecordImpressionEvent.Invoke(instance, obj, args);}
            public event Action<Instance,object, EventArgs> ClosedEvent = delegate {};
            private void InternalInvokeClosedEvent(Instance instance, object obj, EventArgs args){if(ClosedEvent != null) ClosedEvent.Invoke(instance, obj, args);}
#endif
            
            internal override Instance Load(AdPlacement placement)
            {
                if(instances.ContainsKey(placement))
                {
                    Instance ad = instances[placement];
                    if(!ad.Shown)
                        return ad;
                    else
                        instances.Remove(placement);
                }
                Instance instance = new Instance(
                    PlacementToId(placement),
                    placement,
                    consentStatus
                );
#if EM_ADMOB
                instance.PaidEvent              += InternalInvokePaidEvent             ;
                instance.AdOpeningEvent         += InternalInvokeAdOpeningEvent        ;
                instance.AdLoadedEvent          += InternalInvokeAdLoadedEvent         ;
                instance.FailToShowEvent        += InternalInvokeFailToShowEvent       ;
                instance.FailToLoadEvent        += InternalInvokeFailToLoadEvent       ;
                instance.RecordImpressionEvent  += InternalInvokeRecordImpressionEvent ;
                instance.ClosedEvent            += InternalInvokeClosedEvent           ; 
#endif
                instance.Completed += InternalInvokeCompleted;
                instance.Load();
                instances.Add(placement, instance);
                return instance;
            }

            internal override void Destroy(Instance ad)
            {
                PlacementResult pResult = FindInstancePlacement(ad);
                if(pResult.Found)
                {
                    instances.Remove(pResult.Placement);
                    ad.Completed -=InternalInvokeCompleted;
#if EM_ADMOB
                    ad.PaidEvent              -= InternalInvokePaidEvent             ;
                    ad.AdOpeningEvent         -= InternalInvokeAdOpeningEvent        ;
                    ad.AdLoadedEvent          -= InternalInvokeAdLoadedEvent         ;
                    ad.FailToShowEvent        -= InternalInvokeFailToShowEvent       ;
                    ad.FailToLoadEvent        -= InternalInvokeFailToLoadEvent       ;
                    ad.RecordImpressionEvent  -= InternalInvokeRecordImpressionEvent ;
                    ad.ClosedEvent            -= InternalInvokeClosedEvent           ; 
#endif
                    ad.Destroy();
                }
            }

            public class Instance : AdInstance
            {
                internal event Action<Instance> Completed = delegate {};
                internal AdPlacement Placement {get; private set;}
                public bool Shown { get; private set; }
#if EM_ADMOB
                private ConsentStatus consentStatus;
                private string id;
#endif

                public Instance(string id, AdPlacement placement, ConsentStatus consentStatus)
                {
                    this.Placement = placement;
#if EM_ADMOB
                    this.id = id;
                    this.consentStatus = consentStatus;
#endif
                }
#if EM_ADMOB
                private InterstitialAd interstitialAd;

                public event Action<Instance,object, AdValueEventArgs> PaidEvent = delegate {};
                public event Action<Instance,object, EventArgs> AdOpeningEvent = delegate {};
                public event Action<Instance,object, EventArgs> AdLoadedEvent = delegate {};
                public event Action<Instance,object, AdErrorEventArgs> FailToShowEvent = delegate {};
                public event Action<Instance,object, AdFailedToLoadEventArgs> FailToLoadEvent = delegate {};
                public event Action<Instance,object, EventArgs> RecordImpressionEvent = delegate {};
                public event Action<Instance,object, EventArgs> ClosedEvent = delegate {};

                internal override bool IsReady()
                {
                    if(interstitialAd == null || Shown)
                        return false;
                    return interstitialAd.IsLoaded();
                }

                internal override void Destroy()
                {
                    Shown = true;
                    if(interstitialAd != null)
                    {
                        interstitialAd.Destroy();
                        interstitialAd = null;
                    }
                }

                internal override void Show()
                {
                    if(interstitialAd != null)
                        interstitialAd.Show();
                    Shown = true;
                }

                internal override void Load()
                {
                    interstitialAd = new InterstitialAd(id);
                    // Register for interstitial ad events.
                    interstitialAd.OnAdClosed += HandleClosed;
                    interstitialAd.OnAdDidRecordImpression += HandleRecordImpression;
                    interstitialAd.OnAdFailedToLoad += HandleFailToLoad;
                    interstitialAd.OnAdFailedToShow += HandleFailToShow;
                    interstitialAd.OnAdLoaded += HandleAdLoaded;
                    interstitialAd.OnAdOpening += HandleAdOpening;
                    interstitialAd.OnPaidEvent += HandlePaidEvent;
                    interstitialAd.LoadAd(Utils.CreateAdMobAdRequest(consentStatus));
                }

                private void HandlePaidEvent(object sender, AdValueEventArgs e)
                {
                    PaidEvent.Invoke(this, sender, e);
                }

                private void HandleAdOpening(object sender, EventArgs e)
                {
                    AdOpeningEvent.Invoke(this, sender, e);
                }

                private void HandleAdLoaded(object sender, EventArgs e)
                {
                    AdLoadedEvent.Invoke(this, sender, e);
                }

                private void HandleFailToShow(object sender, AdErrorEventArgs e)
                {
                    FailToShowEvent.Invoke(this, sender, e);
                }

                private void HandleFailToLoad(object sender, AdFailedToLoadEventArgs e)
                {
                    FailToLoadEvent.Invoke(this, sender, e);
                    Destroy();
                }

                private void HandleRecordImpression(object sender, EventArgs e)
                {
                    RecordImpressionEvent.Invoke(this, sender, e);
                }

                private void HandleClosed(object sender, EventArgs e)
                {
                    ClosedEvent.Invoke(this, sender, e);
                    Completed.Invoke(this);
                    Destroy();
                }
#endif
            }
        }
        public class Rewarded: Ad<Rewarded.Instance>{
            private AdMobSettings mAdSettings;

            internal event Action<Instance> Completed = delegate {};
            private void InternalInvokeCompleted(Instance instance){if(Completed != null) Completed.Invoke(instance);}
            internal event Action<Instance> Skipped = delegate {};
            private void InternalInvokeSkipped(Instance instance){if(Skipped != null) Skipped.Invoke(instance);}

            public Rewarded(AdMobSettings mAdSettings)
            {
                this.mAdSettings = mAdSettings;
            }

            internal override string PlacementToId(AdPlacement placement)
            {
                AdId id;
                if(placement == AdPlacement.Default)
                    id = mAdSettings.DefaultRewardedAdId;
                else
                    id = Utils.FindIdForPlacement(mAdSettings.CustomRewardedAdIds, placement);
                return Utils.GetIdString(id);
            }

#if EM_ADMOB
            public event Action<Instance,object, AdValueEventArgs> PaidEvent = delegate {};
            private void InternalInvokePaidEvent(Instance instance, object obj, AdValueEventArgs args){if(PaidEvent != null) PaidEvent.Invoke(instance, obj, args);}
            public event Action<Instance,object, EventArgs> AdOpeningEvent = delegate {};
            private void InternalInvokeAdOpeningEvent(Instance instance, object obj, EventArgs args){if(AdOpeningEvent != null) AdOpeningEvent.Invoke(instance, obj, args);}
            public event Action<Instance,object, EventArgs> AdLoadedEvent = delegate {};
            private void InternalInvokeAdLoadedEvent(Instance instance, object obj, EventArgs args){if(AdLoadedEvent != null) AdLoadedEvent.Invoke(instance, obj, args);}
            public event Action<Instance,object, AdErrorEventArgs> FailToShowEvent = delegate {};
            private void InternalInvokeFailToShowEvent(Instance instance, object obj, AdErrorEventArgs args){if(FailToShowEvent != null) FailToShowEvent.Invoke(instance, obj, args); Destroy(instance);}
            public event Action<Instance,object, AdFailedToLoadEventArgs> FailToLoadEvent = delegate {};
            private void InternalInvokeFailToLoadEvent(Instance instance, object obj, AdFailedToLoadEventArgs args){if(FailToLoadEvent != null) FailToLoadEvent.Invoke(instance, obj, args); Destroy(instance);}
            public event Action<Instance,object, EventArgs> RecordImpressionEvent = delegate {};
            private void InternalInvokeRecordImpressionEvent(Instance instance, object obj, EventArgs args){if(RecordImpressionEvent != null) RecordImpressionEvent.Invoke(instance, obj, args);}
            public event Action<Instance,object, EventArgs> ClosedEvent = delegate {};
            private void InternalInvokeClosedEvent(Instance instance, object obj, EventArgs args){if(ClosedEvent != null) ClosedEvent.Invoke(instance, obj, args);}
            public event Action<Instance,object, Reward> RewardEvent = delegate {};
            private void InternalInvokeRewardEvent(Instance instance, object obj, Reward reward){if(RewardEvent != null) RewardEvent.Invoke(instance, obj, reward);}
#endif
            internal override Instance Load(AdPlacement placement)
            {
                if(instances.ContainsKey(placement))
                {
                    Instance ad = instances[placement];
                    if(!ad.Shown)
                        return ad;
                    else
                        instances.Remove(placement);
                }
                Instance instance = new Instance(
                    PlacementToId(placement),
                    placement,
                    consentStatus
                );
#if EM_ADMOB
                instance.PaidEvent              += InternalInvokePaidEvent             ;
                instance.AdOpeningEvent         += InternalInvokeAdOpeningEvent        ;      
                instance.AdLoadedEvent          += InternalInvokeAdLoadedEvent         ;      
                instance.FailToShowEvent        += InternalInvokeFailToShowEvent       ;      
                instance.FailToLoadEvent        += InternalInvokeFailToLoadEvent       ;      
                instance.RecordImpressionEvent  += InternalInvokeRecordImpressionEvent ;      
                instance.ClosedEvent            += InternalInvokeClosedEvent           ;      
                instance.RewardEvent            += InternalInvokeRewardEvent           ;      
#endif
                instance.Completed += InternalInvokeCompleted;
                instance.Skipped += InternalInvokeSkipped;
                instance.Load();
                instances.Add(placement, instance);
                return instance;
            }

            internal override void Destroy(Instance ad)
            {
                PlacementResult pResult = FindInstancePlacement(ad);
                if(pResult.Found)
                {
                    instances.Remove(pResult.Placement);
                    ad.Completed += Completed;
                    ad.Skipped += Skipped;
#if EM_ADMOB
                    ad.PaidEvent              -= InternalInvokePaidEvent             ;
                    ad.AdOpeningEvent         -= InternalInvokeAdOpeningEvent        ;      
                    ad.AdLoadedEvent          -= InternalInvokeAdLoadedEvent         ;      
                    ad.FailToShowEvent        -= InternalInvokeFailToShowEvent       ;      
                    ad.FailToLoadEvent        -= InternalInvokeFailToLoadEvent       ;      
                    ad.RecordImpressionEvent  -= InternalInvokeRecordImpressionEvent ;      
                    ad.ClosedEvent            -= InternalInvokeClosedEvent           ;      
                    ad.RewardEvent            -= InternalInvokeRewardEvent           ;     
#endif
                    ad.Destroy();
                }
            }

            public class Instance : AdInstance
            {
                internal event Action<Instance> Completed = delegate {};
                internal event Action<Instance> Skipped = delegate {};
                internal AdPlacement Placement {get; private set;}
                public bool Rewarded {get; private set;}
                public bool Shown {get; private set;}
#if EM_ADMOB
                private string id;
                private ConsentStatus consentStatus;
#endif

                public Instance(string id, AdPlacement placement, ConsentStatus consentStatus)
                {
                    Placement = placement;
#if EM_ADMOB
                    this.id = id;
                    this.consentStatus = consentStatus;
#endif
                }

#if EM_ADMOB
                public event Action<Instance,object, AdValueEventArgs> PaidEvent = delegate {};
                public event Action<Instance,object, EventArgs> AdOpeningEvent = delegate {};
                public event Action<Instance,object, EventArgs> AdLoadedEvent = delegate {};
                public event Action<Instance,object, AdErrorEventArgs> FailToShowEvent = delegate {};
                public event Action<Instance,object, AdFailedToLoadEventArgs> FailToLoadEvent = delegate {};
                public event Action<Instance,object, EventArgs> RecordImpressionEvent = delegate {};
                public event Action<Instance,object, EventArgs> ClosedEvent = delegate {};
                public event Action<Instance,object, Reward> RewardEvent = delegate {};

                private RewardedAd rewardedAd;

                internal override bool IsReady()
                {
                    if(rewardedAd == null || Shown)
                        return false;
                    return rewardedAd.IsLoaded();
                }

                internal override void Show()
                {
                    if(rewardedAd != null)
                        rewardedAd.Show();
                    Shown = true;
                }

                internal override void Destroy()
                {
                    Shown = true;
                    if(rewardedAd != null)
                    {
                        rewardedAd.Destroy();
                        rewardedAd = null;
                    }
                }

                internal override void Load()
                {
                    rewardedAd = new RewardedAd(id);

                    rewardedAd.OnAdClosed += HandleClosed;
                    rewardedAd.OnAdDidRecordImpression += HandleRecordImpression;
                    rewardedAd.OnAdFailedToLoad += HandleFailToLoad;
                    rewardedAd.OnAdFailedToShow += HandleFailToShow;
                    rewardedAd.OnAdLoaded += HandleAdLoaded;
                    rewardedAd.OnAdOpening += HandleAdOpening;
                    rewardedAd.OnPaidEvent += HandlePaidEvent;
                    rewardedAd.OnUserEarnedReward += HandleRewardEvent;

                    rewardedAd.LoadAd(Utils.CreateAdMobAdRequest(consentStatus));
                }

                private void HandlePaidEvent(object sender, AdValueEventArgs e)
                {
                    PaidEvent.Invoke(this, sender, e);
                }

                private void HandleAdOpening(object sender, EventArgs e)
                {
                    AdOpeningEvent.Invoke(this, sender, e);
                }

                private void HandleAdLoaded(object sender, EventArgs e)
                {
                    AdLoadedEvent.Invoke(this, sender, e);
                }

                private void HandleFailToShow(object sender, AdErrorEventArgs e)
                {
                    FailToShowEvent.Invoke(this, sender, e);
                }

                private void HandleFailToLoad(object sender, AdFailedToLoadEventArgs e)
                {
                    FailToLoadEvent.Invoke(this, sender, e);
                    Destroy();
                }

                private void HandleRecordImpression(object sender, EventArgs e)
                {
                    RecordImpressionEvent.Invoke(this, sender, e);
                }

                private void HandleClosed(object sender, EventArgs e)
                {
                    #if UNITY_EDITOR
                    HandleRewardEvent(sender, new Reward());
                    #endif
                    ClosedEvent.Invoke(this, sender, e);
                    if(Rewarded)
                        Completed.Invoke(this);
                    else
                        Skipped.Invoke(this);
                    Destroy();
                }
                
                private void HandleRewardEvent(object sender, Reward e)
                {
                    Rewarded = true;
                    RewardEvent.Invoke(this, sender, e);
                }
#endif
            }
        }
        public class RewardedInterstitial: Ad<RewardedInterstitial.Instance>{
            private AdMobSettings mAdSettings;

            internal event Action<Instance> Completed = delegate {};
            private void InternalInvokeCompleted(Instance instance){if(Completed != null) Completed.Invoke(instance);}
            internal event Action<Instance> Skipped = delegate {};
            private void InternalInvokeSkipped(Instance instance){if(Skipped != null) Skipped.Invoke(instance);}

            public RewardedInterstitial(AdMobSettings mAdSettings)
            {
                this.mAdSettings = mAdSettings;
            }

            internal override string PlacementToId(AdPlacement placement)
            {
                AdId id;
                if(placement == AdPlacement.Default)
                    id = mAdSettings.DefaultRewardedInterstitialAdId;
                else
                    id = Utils.FindIdForPlacement(mAdSettings.CustomRewardedInterstitialAdIds, placement);
                return Utils.GetIdString(id);
            }
#if EM_ADMOB
            internal event Action<Instance,object, AdValueEventArgs> PaidEvent = delegate {};
            private void InternalInvokePaidEvent(Instance instance, object obj, AdValueEventArgs args){if(PaidEvent != null) PaidEvent.Invoke(instance, obj, args);}
            internal event Action<Instance> AdLoadedEvent = delegate {};
            private void InternalInvokeAdLoadedEvent(Instance instance){if(AdLoadedEvent != null) AdLoadedEvent.Invoke(instance);}
            internal event Action<Instance,object, AdErrorEventArgs> FailToShowEvent = delegate {};
            private void InternalInvokeFailToShowEvent(Instance instance, object obj, AdErrorEventArgs args){if(FailToShowEvent != null) FailToShowEvent.Invoke(instance, obj, args); Destroy(instance);}
            internal event Action<Instance, AdFailedToLoadEventArgs> FailToLoadEvent = delegate {};
            private void InternalInvokeFailToLoadEvent(Instance instance, AdFailedToLoadEventArgs args){if(FailToLoadEvent != null) FailToLoadEvent.Invoke(instance, args); Destroy(instance);}
            internal event Action<Instance,object, EventArgs> RecordImpressionEvent = delegate {};
            private void InternalInvokeRecordImpressionEvent(Instance instance, object obj, EventArgs args){if(RecordImpressionEvent != null) RecordImpressionEvent.Invoke(instance, obj, args);}
            internal event Action<Instance,object, EventArgs> ClosedEvent = delegate {};
            private void InternalInvokeClosedEvent(Instance instance, object obj, EventArgs args){if(ClosedEvent != null) ClosedEvent.Invoke(instance, obj, args);}
            internal event Action<Instance,Reward> RewardEvent = delegate {};
            private void InternalInvokeRewardEvent(Instance instance,Reward reward){if(RewardEvent != null) RewardEvent.Invoke(instance, reward);}
            internal event Action<Instance,object, EventArgs> ShownEvent = delegate {};
            private void InternalInvokeShownEvent(Instance instance, object obj, EventArgs args){if(ShownEvent != null) ShownEvent.Invoke(instance, obj, args);}
#endif
            internal override Instance Load(AdPlacement placement)
            {
                if(instances.ContainsKey(placement))
                {
                    Instance ad = instances[placement];
                    if(!ad.Shown)
                        return ad;
                    else
                        instances.Remove(placement);
                }
                Instance instance = new Instance(
                    PlacementToId(placement),
                    placement,
                    consentStatus
                );
                instance.Completed += InternalInvokeCompleted;
                instance.Skipped += InternalInvokeSkipped;
#if EM_ADMOB
                instance.PaidEvent              += InternalInvokePaidEvent             ;
                instance.AdLoadedEvent          += InternalInvokeAdLoadedEvent         ;
                instance.FailToShowEvent        += InternalInvokeFailToShowEvent       ;
                instance.FailToLoadEvent        += InternalInvokeFailToLoadEvent       ;
                instance.RecordImpressionEvent  += InternalInvokeRecordImpressionEvent ;
                instance.ClosedEvent            += InternalInvokeClosedEvent           ;
                instance.RewardEvent            += InternalInvokeRewardEvent           ;
                instance.ShownEvent             += InternalInvokeShownEvent            ;
#endif
                instance.Load();
                instances.Add(placement, instance);
                return instance;
            }

            internal override void Destroy(Instance ad)
            {
                PlacementResult pResult = FindInstancePlacement(ad);
                if(pResult.Found)
                {
                    instances.Remove(pResult.Placement);
                    ad.Completed += Completed;
                    ad.Skipped += Skipped;
#if EM_ADMOB
                    ad.PaidEvent              -= InternalInvokePaidEvent             ;
                    ad.AdLoadedEvent          -= InternalInvokeAdLoadedEvent         ;
                    ad.FailToShowEvent        -= InternalInvokeFailToShowEvent       ;
                    ad.FailToLoadEvent        -= InternalInvokeFailToLoadEvent       ;
                    ad.RecordImpressionEvent  -= InternalInvokeRecordImpressionEvent ;
                    ad.ClosedEvent            -= InternalInvokeClosedEvent           ;
                    ad.RewardEvent            -= InternalInvokeRewardEvent           ;
                    ad.ShownEvent             -= InternalInvokeShownEvent            ;
#endif
                    ad.Destroy();
                }
            }

            public class Instance : AdInstance
            {
                internal event Action<Instance> Completed = delegate {};
                internal event Action<Instance> Skipped = delegate {};
                internal AdPlacement Placement {get; private set;}
                
                public bool Rewarded {get; private set;}
                public bool Shown {get; private set;}
#if EM_ADMOB
                private string id;
                private ConsentStatus consentStatus;
#endif

                public Instance(string id, AdPlacement placement, ConsentStatus consentStatus)
                {
                    Placement = placement;
#if EM_ADMOB
                    this.id = id;
                    this.consentStatus = consentStatus;
#endif
                }

#if EM_ADMOB
                internal event Action<Instance,object, AdValueEventArgs> PaidEvent = delegate {};
                internal event Action<Instance> AdLoadedEvent = delegate {};
                internal event Action<Instance,object, AdErrorEventArgs> FailToShowEvent = delegate {};
                internal event Action<Instance, AdFailedToLoadEventArgs> FailToLoadEvent = delegate {};
                internal event Action<Instance,object, EventArgs> RecordImpressionEvent = delegate {};
                internal event Action<Instance,object, EventArgs> ClosedEvent = delegate {};
                internal event Action<Instance,Reward> RewardEvent = delegate {};
                internal event Action<Instance,object, EventArgs> ShownEvent = delegate {};

                private RewardedInterstitialAd rewardedAd;

                internal override bool IsReady()
                {
                    if(rewardedAd == null || Shown)
                        return false;
                    return true;
                }

                internal override void Show()
                {
                    if(rewardedAd != null)
                        rewardedAd.Show(HandleRewardEvent);
                    Shown = true;
                }

                internal override void Destroy()
                {
                    Shown = true;
                    if(rewardedAd != null)
                    {
                        rewardedAd.Destroy();
                        rewardedAd = null;
                    }
                }

                internal override void Load()
                {
                    AdRequest request = Utils.CreateAdMobAdRequest(consentStatus);
                    RewardedInterstitialAd.LoadAd(id, request, HandleAdLoaded);
                }

                private void HandlePaidEvent(object sender, AdValueEventArgs e)
                {
                    PaidEvent.Invoke(this, sender, e);
                }

                private void HandleAdLoaded(RewardedInterstitialAd rewardedAd, AdFailedToLoadEventArgs failedToLoad)
                {
                    if(failedToLoad != null)
                    {
                        HandleFailToLoad(failedToLoad);
                        return;
                    }
                    this.rewardedAd = rewardedAd;
                    AdLoadedEvent.Invoke(this);

                    rewardedAd.OnAdDidDismissFullScreenContent += HandleClosed;
                    rewardedAd.OnAdDidRecordImpression += HandleRecordImpression;
                    rewardedAd.OnAdDidPresentFullScreenContent += HandleShown;
                    rewardedAd.OnAdFailedToPresentFullScreenContent += HandleFailToShow;
                    rewardedAd.OnPaidEvent += HandlePaidEvent;
                }

                private void HandleShown(object sender, EventArgs e)
                {
                    ShownEvent.Invoke(this, sender, e);
                }

                private void HandleFailToShow(object sender, AdErrorEventArgs e)
                {
                    FailToShowEvent.Invoke(this, sender, e);
                }

                private void HandleFailToLoad(AdFailedToLoadEventArgs e)
                {
                    FailToLoadEvent.Invoke(this, e);
                    Destroy();
                }

                private void HandleRecordImpression(object sender, EventArgs e)
                {
                    RecordImpressionEvent.Invoke(this, sender, e);
                }

                private void HandleClosed(object sender, EventArgs e)
                {
                    #if UNITY_EDITOR
                    HandleRewardEvent(new Reward());
                    #endif
                    ClosedEvent.Invoke(this, sender, e);
                    if(Rewarded)
                        Completed.Invoke(this);
                    else
                        Skipped.Invoke(this);
                    Destroy();
                }
                
                private void HandleRewardEvent(Reward e)
                {
                    Rewarded = true;
                    RewardEvent.Invoke(this, e);
                }
#endif
            }
        }
        private static class Utils{
            public static string CreateMD5(string input)
            {
                if (string.IsNullOrEmpty(input)) return string.Empty;

                using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
                {
                    byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                    byte[] hashBytes = md5.ComputeHash(inputBytes);

                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < hashBytes.Length; i++)
                    {
                        sb.Append(hashBytes[i].ToString("X2"));
                    }
                    return sb.ToString();
                }
            }

            public static string GetTestDeviceId()
            {
                string testDeviceID = "";
#if UNITY_ANDROID
                        testDeviceID = SystemInfo.deviceUniqueIdentifier.ToUpper();
#elif UNITY_IOS
                        testDeviceID = UnityEngine.iOS.Device.advertisingIdentifier;
                        testDeviceID = CreateMD5(testDeviceID);
                        testDeviceID = testDeviceID.ToLower();
#endif
                return testDeviceID;
            }

            internal static AdId FindIdForPlacement(Dictionary<AdPlacement, AdId> idDict, AdPlacement placement)
            {
                if(!idDict.ContainsKey(placement))
                    return null;
                return idDict[placement];
            }

            internal static string GetIdString(AdId id)
            {
                string stringId = "";
                if(id == null)
                    return stringId;
                stringId = id.Id;
#if UNITY_ANDROID
                    stringId = id.AndroidId;
#elif UNITY_IOS
                    stringId = id.IosId;
#endif
                return stringId;
            }

#if EM_ADMOB
            /// <summary>
            /// Used to specify that only non-personalized ads should be requested when creating ad request in
            /// <see cref="CreateAdMobAdRequest"/>
            /// </summary>
            private static KeyValuePair<string, string> mNonPersonalizedPair = new KeyValuePair<string, string>("npa", "1");

            internal static AdRequest CreateAdMobAdRequest(ConsentStatus consent)
            {
                AdRequest.Builder adBuilder = new AdRequest.Builder();
                // Targeting settings.
                AdMobSettings.AdMobTargetingSettings targeting = mAdSettings.TargetingSettings;
                // Extras.
                if (targeting.ExtraOptions != null)
                {
                    foreach (KeyValuePair<string, string> extra in targeting.ExtraOptions)
                    {
                        if (!string.IsNullOrEmpty(extra.Key) && !string.IsNullOrEmpty(extra.Value))
                            adBuilder.AddExtra(extra.Key, extra.Value);
                    }
                }

                // Configure the ad request to serve non-personalized ads.
                // The default behavior of the Google Mobile Ads SDK is to serve personalized ads,
                // we only do this if the user has explicitly denied to grant consent.
                // https://developers.google.com/admob/unity/eu-consent
                if (consent == ConsentStatus.Revoked)
                    adBuilder.AddExtra(mNonPersonalizedPair.Key, mNonPersonalizedPair.Value);
                return adBuilder.Build();
            }

            public static TagForChildDirectedTreatment Convert(AdChildDirectedTreatment childTreatment)
            {
                switch (childTreatment)
                {
                    case AdChildDirectedTreatment.Yes:
                        return TagForChildDirectedTreatment.True;
                    case AdChildDirectedTreatment.No:
                        return TagForChildDirectedTreatment.False;
                    case AdChildDirectedTreatment.Unspecified:
                        return TagForChildDirectedTreatment.Unspecified;
                }
                return TagForChildDirectedTreatment.Unspecified;
            }

            public static AdSize Convert(BannerAdSize adSize)
            {
                return adSize.IsSmartBanner
                    ? (mAdSettings.UseAdaptiveBanner
                        ? AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth)
                        : AdSize.SmartBanner)
                    : new AdSize(adSize.Width, adSize.Height);
            }

            public static AdPosition Convert(BannerAdPosition adPosition)
            {
                switch (adPosition)
                {
                    case BannerAdPosition.Top:
                        return AdPosition.Top;
                    case BannerAdPosition.Bottom:
                        return AdPosition.Bottom;
                    case BannerAdPosition.TopLeft:
                        return AdPosition.TopLeft;
                    case BannerAdPosition.TopRight:
                        return AdPosition.TopRight;
                    case BannerAdPosition.BottomLeft:
                        return AdPosition.BottomLeft;
                    case BannerAdPosition.BottomRight:
                        return AdPosition.BottomRight;
                    default:
                        return AdPosition.Top;
                }
            }
#endif
        }
    }
}