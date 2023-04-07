namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using GameFoundation.Scripts.Utilities;
    using ServiceImplementation.AdsServices.Signal;
    using Zenject;

    public class UITemplateHandleSoundWhenOpenAdsServices
    {
        private readonly SignalBus     signalBus;
        private readonly IAudioManager audioManager;

        public UITemplateHandleSoundWhenOpenAdsServices(SignalBus signalBus, IAudioManager audioManager)
        {
            this.signalBus    = signalBus;
            this.audioManager = audioManager;
            this.signalBus.Subscribe<AppStateChangeSignal>(this.OnApplicationPause);
        }

        private void OnApplicationPause(AppStateChangeSignal obj)
        {
            if (obj.IsBackground)
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