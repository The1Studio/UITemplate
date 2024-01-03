namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using System;
    using Core.AdsServices.Signals;
    using UnityEngine.EventSystems;
    using Zenject;

    public class LockInputService : IInitializable
    {
        #region Inject

        private readonly SignalBus signalBus;

        public LockInputService(SignalBus signalBus) { this.signalBus = signalBus; }

        #endregion

        private EventSystem eventSystem;

        public void Initialize()
        {
            this.eventSystem = EventSystem.current;
            this.signalBus.Subscribe<InterstitialAdCalledSignal>(() => this.SetLockInput(false));
            this.signalBus.Subscribe<InterstitialAdDisplayedFailedSignal>(() => this.SetLockInput(true));
            this.signalBus.Subscribe<InterstitialAdClosedSignal>(() => this.SetLockInput(true));
        }

        private void SetLockInput(bool value)
        {
            if (this.eventSystem == null) return;
            this.eventSystem.enabled = value;
        }
    }
}