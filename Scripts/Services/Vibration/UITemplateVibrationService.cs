namespace TheOneStudio.UITemplate.UITemplate.Services.Vibration
{
    using System;
    using Lofelt.NiceVibrations;
    using TheOneStudio.UITemplate.UITemplate.Interfaces;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using UnityEngine.Scripting;

    public class UITemplateVibrationService : IVibrationService
    {
        #region inject

        private readonly UITemplateSettingDataController uiTemplateSettingDataController;

        #endregion

        private readonly bool hapticsSupported;

        [Preserve]
        public UITemplateVibrationService(UITemplateSettingDataController uiTemplateSettingDataController)
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

        public void PlayPresetType(VibrationPresetType vibrationPresetType)
        {
            if (!this.uiTemplateSettingDataController.IsVibrationOn) return;
            HapticPatterns.PlayPreset(this.GetHapticPatternsPresetType(vibrationPresetType));
        }

        public void PlayEmphasis(float amplitude, float frequency)
        {
            if (!this.uiTemplateSettingDataController.IsVibrationOn) return;
            HapticPatterns.PlayEmphasis(amplitude, frequency);
        }

        public void PlayConstant(float amplitude, float frequency, float duration)
        {
            if (!this.uiTemplateSettingDataController.IsVibrationOn) return;
            HapticPatterns.PlayConstant(amplitude, frequency, duration);
        }
    }
}