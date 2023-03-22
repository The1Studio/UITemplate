namespace TheOneStudio.UITemplate.UITemplate.Models
{
    using GameFoundation.Scripts.Interfaces;

    public class UITemplateUserSettingData : ILocalData
    {
        public bool IsVibrationEnable  = true;
        public bool IsFlashLightEnable = true;

        public void Init() { }
    }
}