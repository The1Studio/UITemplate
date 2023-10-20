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

        public List<AdViewPosition> listMRecAdViewPosition = new();

        [SerializeField] private List<string> listMRecAndroidId = new();
        [SerializeField] private List<string> listMRecIOSId     = new();

        [SerializeField] private List<string> listNativeAndroidId = new();
        [SerializeField] private List<string> listNativeIOSId     = new();

        public List<string> ListAoaAppId             => Application.platform is RuntimePlatform.Android or RuntimePlatform.WindowsEditor or RuntimePlatform.OSXEditor ? this.listAoaAppId : this.listAoaIOSAppId;
        public List<string> ListMRecId               => Application.platform is RuntimePlatform.Android or RuntimePlatform.WindowsEditor or RuntimePlatform.OSXEditor ? this.listMRecAndroidId : this.listMRecIOSId;
        public List<string> ListNativeId             => Application.platform is RuntimePlatform.Android or RuntimePlatform.WindowsEditor or RuntimePlatform.OSXEditor ? this.listNativeAndroidId : this.listNativeIOSId;
    }
}