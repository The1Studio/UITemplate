using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EasyMobile.Demo
{
    [Serializable]
    public class RewardedInterstitialSection : LoadAndShowSection<RewardedInterstitialSection.DefaultRewardedInterstitialUI, RewardedInterstitialSection.CustomRewardedInterstitialUI>
    {
        [Serializable]
        public class DefaultRewardedInterstitialUI : DefaultElement
        {
            protected override string AdReadyMessage { get { return "IsRewardedInterstitialAdReady: TRUE"; } }

            protected override string AdNotReadyMessage { get { return "IsRewardedInterstitialAdReady: FALSE"; } }

            protected override string UnavailableAdAlertMessage { get { return "Rewarded Interstitial ad is not loaded."; } }

            protected override bool IsAdReady()
            {
                return Advertising.IsRewardedInterstitialAdReady();
            }

            /// <summary>
            /// Load default rewarded video.
            /// </summary>
            protected override void LoadAd()
            {
                if (Advertising.AutoAdLoadingMode == AutoAdLoadingMode.LoadAllDefinedPlacements || Advertising.AutoAdLoadingMode == AutoAdLoadingMode.LoadDefaultAds)
                {
                    NativeUI.Alert("Alert", "autoLoadDefaultAds is currently enabled. " +
                        "Ads will be loaded automatically in background without you having to do anything.");
                }

                Advertising.LoadRewardedInterstitialAd();
            }

            /// <summary>
            /// Show default rewarded video.
            /// </summary>
            protected override void ShowAd()
            {
                Advertising.ShowRewardedInterstitialAd();
            }
        }

        [Serializable]
        public class CustomRewardedInterstitialUI : CustomElement
        {
            private List<RewardedInterstitialAdNetwork> allRewardedInterstitialNetworks;

            protected override string AdReadyMessage
            {
                get { return string.Format("IsRewardedInterstitialAdReady{0}: TRUE", string.IsNullOrEmpty(CustomKey) ? "" : "(" + CustomKey + ")"); }
            }

            protected override string AdNotReadyMessage
            {
                get { return string.Format("IsRewardedInterstitialAdReady{0}: FALSE", string.IsNullOrEmpty(CustomKey) ? "" : "(" + CustomKey + ")"); }
            }

            protected override string UnavailableAdAlertMessage
            {
                get { return string.Format("The RewardedInterstitial ad at the {0} placement is not loaded.", string.IsNullOrEmpty(CustomKey) ? "default" : CustomKey); }
            }

            private RewardedInterstitialAdNetwork SelectedNetwork
            {
                get { return allRewardedInterstitialNetworks[networkSelector.value]; }
            }

            protected override void InitNetworkDropdown()
            {
                allRewardedInterstitialNetworks = new List<RewardedInterstitialAdNetwork>();
                List<Dropdown.OptionData> optionDatas = new List<Dropdown.OptionData>();
                foreach (RewardedInterstitialAdNetwork network in Enum.GetValues(typeof(RewardedInterstitialAdNetwork)))
                {
                    allRewardedInterstitialNetworks.Add(network);
                    optionDatas.Add(new Dropdown.OptionData(network.ToString()));
                }

                networkSelector.ClearOptions();
                networkSelector.AddOptions(optionDatas);
            }

            protected override bool IsAdReady()
            {
                return Advertising.IsRewardedInterstitialAdReady(SelectedNetwork, AdPlacement.PlacementWithName(CustomKey));
            }

            protected override void LoadAd()
            {
                Advertising.LoadRewardedInterstitialAd(SelectedNetwork, AdPlacement.PlacementWithName(CustomKey));
            }

            protected override void ShowAd()
            {
                Advertising.ShowRewardedInterstitialAd(SelectedNetwork, AdPlacement.PlacementWithName(CustomKey));
            }
        }
    }
}