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
        public List<AdViewPosition> listMRecAdViewPosition = new();

        [SerializeField] private List<string> listMRecAndroidId = new();
        [SerializeField] private List<string> listMRecIOSId     = new();

        public List<string> ListMRecId               => Application.platform is RuntimePlatform.Android or RuntimePlatform.WindowsEditor or RuntimePlatform.OSXEditor ? this.listMRecAndroidId : this.listMRecIOSId;
    }
}