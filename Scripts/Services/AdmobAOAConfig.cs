namespace TheOneStudio.UITemplate.UITemplate.Scripts.Services
{
    using System;
    using System.Collections.Generic;
    using Core.AdsServices;
    using global::Models;
    using UnityEngine;

    [Serializable]
    [CreateAssetMenu(fileName = "AdmobAOAConfig", menuName = "Configs/AdmobAOAConfig", order = 0)]
    public class AdmobAOAConfig : ScriptableObject, IGameConfig
    {
        [SerializeField] private List<string> listAoaAppId    = new();
        [SerializeField] private List<string> listAoaIOSAppId = new();

        [SerializeField] private int adMobAOAOpenAppThreshold = 5;

        public List<AdViewPosition> listMRecAdViewPosition = new();

        [SerializeField] private List<string> listMRecAndroidId = new();
        [SerializeField] private List<string> listMRecIOSId     = new();

        [SerializeField] private List<string> listNativeAndroidId = new();
        [SerializeField] private List<string> listNativeIOSId     = new();

        public List<string> ListAoaAppId             => Application.platform is RuntimePlatform.Android or RuntimePlatform.WindowsEditor ? this.listAoaAppId : this.listAoaIOSAppId;
        public List<string> ListMRecId               => Application.platform is RuntimePlatform.Android or RuntimePlatform.WindowsEditor ? this.listMRecAndroidId : this.listMRecIOSId;
        public List<string> ListNativeId             => Application.platform is RuntimePlatform.Android or RuntimePlatform.WindowsEditor ? this.listNativeAndroidId : this.listNativeIOSId;
        public int          AdMObAOAOpenAppThreshold => this.adMobAOAOpenAppThreshold;
    }
}