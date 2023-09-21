namespace TheOneStudio.UITemplate.UITemplate.Services.Vibration
{
    using System;
    using Lofelt.NiceVibrations;
    using TheOneStudio.UITemplate.UITemplate.Interfaces;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;

    public class UITemPlateVibrateServices : IVibrate
    {
        #region inject

        private readonly UITemplateSettingDataController uiTemplateSettingDataController;

        #endregion

        private readonly bool hapticsSupported;

        public UITemPlateVibrateServices(UITemplateSettingDataController uiTemplateSettingDataController)
        {
            this.uiTemplateSettingDataController = uiTemplateSettingDataController;
            this.hapticsSupported = DeviceCapabilities.isVersionSupported;
        }

        private HapticPatterns.PresetType GetHapticPatternsPresetType(VibrationPresetType vibrationPresetType)
        {
            return vibrationPresetType switch
            {
                VibrationPresetType.Selection => HapticPatterns.PresetType.Selection,
                VibrationPresetType.Success => HapticPatterns.PresetType.Success,
                VibrationPresetType.Warning => HapticPatterns.PresetType.Warning,
                VibrationPresetType.Failure => HapticPatterns.PresetType.Failure,
                VibrationPresetType.LightImpact => HapticPatterns.PresetType.LightImpact,
                VibrationPresetType.MediumImpact => HapticPatterns.PresetType.MediumImpact,
                VibrationPresetType.HeavyImpact => HapticPatterns.PresetType.HeavyImpact,
                VibrationPresetType.RigidImpact => HapticPatterns.PresetType.RigidImpact,
                VibrationPresetType.SoftImpact => HapticPatterns.PresetType.SoftImpact,
                VibrationPresetType.None => HapticPatterns.PresetType.None,
                _ => throw new ArgumentOutOfRangeException(nameof(vibrationPresetType), vibrationPresetType, null)
            };
        }

        public void VibratePresetType(VibrationPresetType vibrationPresetType)
        {
            HapticPatterns.PlayPreset(this.GetHapticPatternsPresetType(vibrationPresetType));
        }
    }
}