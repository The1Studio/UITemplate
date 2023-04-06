namespace TheOneStudio.UITemplate.UITemplate.Scripts.Services
{
    using System;
    using System.Collections.Generic;
    using Core.AdsServices;
    using global::Models;
    using UnityEngine;
    using UnityEngine.Serialization;

    [Serializable]
    [CreateAssetMenu(fileName = "AdmobAOAConfig", menuName = "Configs/AdmobAOAConfig", order = 0)]
    public class AdmobAOAConfig : ScriptableObject, IGameConfig
    {
        public List<string> listAoaAppId    = new();
        public List<string> listAoaIOSAppId = new();

        public List<AdViewPosition> listMRecAdViewPosition = new();
        public List<string>         listMRecAndroidId      = new();
        public List<string>         listMRecIOSId          = new();

        public List<string> listNativeAndroidId = new();
        public List<string> listNativeIOSId     = new();
    }
}