namespace TheOneStudio.UITemplate.UITemplate.Scripts.Services
{
    using System;
    using System.Collections.Generic;
    using global::Models;
    using UnityEngine;

    [Serializable]
    [CreateAssetMenu(fileName = "AdmobAOAConfig", menuName = "Configs/AdmobAOAConfig", order = 0)]
    public class AdmobAOAConfig : ScriptableObject, IGameConfig
    {
        public List<string> listAoaAppId = new();
        public List<string> listAoaIOSAppId = new();
    }
}