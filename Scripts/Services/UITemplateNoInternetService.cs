namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using System;
    using Cysharp.Threading.Tasks;
    using GameFoundation.DI;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Signals;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Configs.GameEvents;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Popups;
    using UnityEngine;
    using UnityEngine.Scripting;

    public class UITemplateNoInternetService : IInitializable
    {
        private readonly SignalBus                           signalBus;
        private readonly GameFeaturesSetting                 gameFeaturesSetting;
        private readonly IScreenManager                      screenManager;
        private readonly UITemplateGameSessionDataController uiTemplateGameSessionDataController;

        [Preserve]
        public UITemplateNoInternetService(
            SignalBus                           signalBus,
            GameFeaturesSetting                 gameFeaturesSetting,
            IScreenManager                      screenManager,
            UITemplateGameSessionDataController uiTemplateGameSessionDataController
        )
        {
            this.signalBus                           = signalBus;
            this.gameFeaturesSetting                 = gameFeaturesSetting;
            this.screenManager                       = screenManager;
            this.uiTemplateGameSessionDataController = uiTemplateGameSessionDataController;
        }

        private bool IsAbleToCheck  => this.IsSessionValid && this.IsTimeValid && this.isScreenValid;
        private bool IsSessionValid => this.uiTemplateGameSessionDataController.OpenTime >= this.gameFeaturesSetting.NoInternetConfig.SessionToShow;
        private bool IsTimeValid    => this.gameFeaturesSetting.NoInternetConfig.DelayToCheck < Time.realtimeSinceStartup;
        private bool isScreenValid;

        private int   continuousNoInternetChecked = 0;
        private float CheckInterval => this.gameFeaturesSetting.NoInternetConfig.CheckInterval;

        public void Initialize()
        {
            this.signalBus.Subscribe<ScreenShowSignal>(this.OnScreenShow);
            this.CheckInternetInterval().Forget();
        }

        #region Check Internet

        private async UniTaskVoid CheckInternetInterval()
        {
            if (this.IsAbleToCheck)
            {
                if (this.CheckInternet())
                {
                    this.continuousNoInternetChecked = 0;
                }
                else
                {
                    this.continuousNoInternetChecked++;
                }

                if (this.continuousNoInternetChecked >= this.gameFeaturesSetting.NoInternetConfig.ContinuesFailToShow)
                {
                    this.continuousNoInternetChecked = 0;
                    this.screenManager.OpenScreen<UITemplateConnectErrorPresenter>().Forget();
                }
            }

            await UniTask.Delay(TimeSpan.FromSeconds(this.CheckInterval), true);
            this.CheckInternetInterval().Forget();
        }

        private bool CheckInternet() { return Application.internetReachability != NetworkReachability.NotReachable; }

        #endregion

        private void OnScreenShow(ScreenShowSignal obj)
        {
            if (this.gameFeaturesSetting.NoInternetConfig.isCustomScreenTrigger)
            {
                this.isScreenValid = this.gameFeaturesSetting.NoInternetConfig.screenTriggerIds.Contains(obj.ScreenPresenter.GetType().Name);
            }
            else
            {
                this.isScreenValid = obj.ScreenPresenter.GetType().Name != "UITemplateConnectErrorPresenter";
            }
        }
    }
}