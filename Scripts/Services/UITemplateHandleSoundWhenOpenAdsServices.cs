namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using GameFoundation.Scripts.Utilities;
    using GameFoundation.Scripts.Utilities.ApplicationServices;
    using Zenject;

    public class UITemplateHandleSoundWhenOpenAdsServices
    {
        private readonly SignalBus     signalBus;
        private readonly IAudioManager audioManager;

        public UITemplateHandleSoundWhenOpenAdsServices(SignalBus signalBus, IAudioManager audioManager)
        {
            this.signalBus    = signalBus;
            this.audioManager = audioManager;
            this.signalBus.Subscribe<ApplicationPauseSignal>(this.OnApplicationPause);
        }

        private void OnApplicationPause(ApplicationPauseSignal obj)
        {
            if (obj.PauseStatus)
            {
                this.audioManager.PauseEverything();
            }
            else
            {
                this.audioManager.ResumeEverything();
            }
        }
    }
}