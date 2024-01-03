namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using System;
    using Core.AdsServices.Signals;
    using UnityEngine.EventSystems;
    using Zenject;

    public class LockInputService : IInitializable
    {
        #region Inject

        private readonly SignalBus   signalBus;
        private readonly EventSystem eventSystem;

        public LockInputService(SignalBus signalBus, EventSystem eventSystem)
        {
            this.signalBus   = signalBus;
            this.eventSystem = eventSystem;
        }

        #endregion


        public void Initialize()
        {
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