namespace TheOneStudio.UITemplate.UITemplate.Models
{
    using GameFoundation.Scripts.Interfaces;
    using Sirenix.Serialization;

    public class UITemplateUserSettingData : ILocalData
    {
        [OdinSerialize]
        public bool IsVibrationEnable = true;

        [OdinSerialize]
        public bool IsFlashLightEnable = true;

        public void Init()
        {
        }
    }
}