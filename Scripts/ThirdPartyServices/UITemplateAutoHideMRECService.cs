namespace TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices
{
    using System;
    using System.Collections.Generic;
    using Core.AdsServices;
    using Core.AdsServices.Signals;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using UnityEngine;
    using Zenject;

    public class UITemplateAutoHideMRECService : ITickable
    {
        #region Inject

        private readonly IScreenManager             screenManager;
        private readonly UITemplateAdServiceWrapper adServiceWrapper;
        private readonly SignalBus                  signalBus;

        #endregion

        private bool       hasMRECShow;
        private List<Type> screenAutoHideMREC = new();

        public UITemplateAutoHideMRECService(IScreenManager screenManager,
            UITemplateAdServiceWrapper adServiceWrapper,
            SignalBus signalBus)
        {
            this.screenManager    = screenManager;
            this.adServiceWrapper = adServiceWrapper;
            this.signalBus        = signalBus;

            this.signalBus.Subscribe<MRecAdDisplayedSignal>(this.OnMRECDisplayed);
            this.signalBus.Subscribe<MRecAdDismissedSignal>(this.OnMRECDismissed);
        }

        private void OnMRECDismissed() { this.hasMRECShow = true; }

        private void OnMRECDisplayed() { this.hasMRECShow = false; }

        public void AddScreenAutoHideMREC(Type screenType)
        {
            if (this.screenAutoHideMREC.Contains(screenType))
            {
                Debug.LogError($"Screen: {screenType.Name} contained, can't add to collection!");
                return;
            }

            this.screenAutoHideMREC.Add(screenType);
        }

        private void AutoHideMREC()
        {
            if (!this.hasMRECShow) return;
            if (!this.screenManager.CurrentActiveScreen.HasValue) return;
            if (!this.screenAutoHideMREC.Contains(this.screenManager.CurrentActiveScreen.Value.GetType())) return;

            foreach (AdViewPosition position in Enum.GetValues(typeof(AdViewPosition)))
            {
                this.adServiceWrapper.HideMREC(position);
            }
        }

        public void Tick() { this.AutoHideMREC(); }
    }
}