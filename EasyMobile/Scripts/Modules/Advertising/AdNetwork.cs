using UnityEngine;
using System.Collections;

namespace EasyMobile
{
    // List of all supported ad networks
    public enum AdNetwork
    {
        None,
        AdColony,
        AdMob,
        AppLovin,
        AudienceNetwork,
        Chartboost,
        FairBid,
        IronSource,
        TapJoy,
        UnityAds,
        Vungle,
    }

    public enum BannerAdNetwork
    {
        None = AdNetwork.None,
        AdColony = AdNetwork.AdColony,
        AdMob = AdNetwork.AdMob,
        AppLovin = AdNetwork.AppLovin,
        AudienceNetwork = AdNetwork.AudienceNetwork,
        FairBid = AdNetwork.FairBid,
        IronSource = AdNetwork.IronSource,
        UnityAds = AdNetwork.UnityAds,
        Vungle = AdNetwork.Vungle,
    }

    public enum InterstitialAdNetwork
    {
        None = AdNetwork.None,
        AdColony = AdNetwork.AdColony,
        AdMob = AdNetwork.AdMob,
        AppLovin = AdNetwork.AppLovin,
        AudienceNetwork = AdNetwork.AudienceNetwork,
        Chartboost = AdNetwork.Chartboost,
        FairBid = AdNetwork.FairBid,
        IronSource = AdNetwork.IronSource,
        TapJoy = AdNetwork.TapJoy,
        UnityAds = AdNetwork.UnityAds,
        Vungle = AdNetwork.Vungle,
    }

    public enum RewardedAdNetwork
    {
        None = AdNetwork.None,
        AdColony = AdNetwork.AdColony,
        AdMob = AdNetwork.AdMob,
        AppLovin = AdNetwork.AppLovin,
        AudienceNetwork = AdNetwork.AudienceNetwork,
        Chartboost = AdNetwork.Chartboost,
        FairBid = AdNetwork.FairBid,
        IronSource = AdNetwork.IronSource,
        TapJoy = AdNetwork.TapJoy,
        UnityAds = AdNetwork.UnityAds,
        Vungle = AdNetwork.Vungle,
    }

    public enum RewardedInterstitialAdNetwork
    {
        None = AdNetwork.None,
        AdMob = AdNetwork.AdMob
    }
}