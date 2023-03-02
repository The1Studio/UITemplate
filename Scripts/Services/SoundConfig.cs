namespace TheOneStudio.UITemplate.UITemplate.Scripts.Services
{
    using System;
    using global::Models;
    using UnityEngine;
    using Utilities.SoundServices;

    [Serializable]
    [CreateAssetMenu(fileName = "SoundConfig", menuName = "Configs/SoundConfig", order = 0)]
    public class SoundConfig : ScriptableObject, IGameConfig
    {
        public MasterAaaSoundMasterModel masterAaaSoundMasterModel;
    }
}