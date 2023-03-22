namespace TheOneStudio.UITemplate.UITemplate.Models
{
    using GameFoundation.Scripts.Interfaces;

    public class UITemplateUserSettingData : ILocalData
    {
        internal bool IsVibrationEnable  { get; set; } = true;
        internal bool IsFlashLightEnable { get; set; } = true;

        public void Init() { }
    }
}