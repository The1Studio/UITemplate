#nullable enable
namespace UITemplate.Scripts.Services.Vibration
{
    using GameFoundation.DI;
    using TheOneStudio.UITemplate.UITemplate.Interfaces;
    using UnityEngine;

    internal sealed class PlayVibrationConstant : MonoBehaviour
    {
        [SerializeField] private float amplitude;
        [SerializeField] private float frequency;
        [SerializeField] private float duration;

        private IVibrationService vibrationService = null!;

        private void Awake()
        {
            this.vibrationService = this.GetCurrentContainer().Resolve<IVibrationService>();
        }

        private void OnEnable()
        {
            this.vibrationService.PlayConstant(this.amplitude, this.frequency, this.duration);
        }
    }
}