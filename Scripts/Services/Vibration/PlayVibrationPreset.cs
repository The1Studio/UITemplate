#nullable enable
namespace UITemplate.Scripts.Services.Vibration
{
    using GameFoundation.DI;
    using TheOneStudio.UITemplate.UITemplate.Interfaces;
    using TheOneStudio.UITemplate.UITemplate.Services.Vibration;
    using UnityEngine;

    internal sealed class PlayVibrationPreset : MonoBehaviour
    {
        [SerializeField] private VibrationPresetType vibrationPresetType;

        private IVibrationService vibrationService = null!;

        private void Awake()
        {
            this.vibrationService = this.GetCurrentContainer().Resolve<IVibrationService>();
        }

        private void OnEnable()
        {
            this.vibrationService.PlayPresetType(this.vibrationPresetType);
        }
    }
}