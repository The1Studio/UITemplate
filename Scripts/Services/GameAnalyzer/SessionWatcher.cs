namespace Core.AnalyticServices.Data
{
    using System;
    using System.Collections.Generic;
    using Core.AnalyticServices.CommonEvents;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.Utilities.ApplicationServices;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using UnityEngine.Scripting;
    using VContainer.Unity;

    internal sealed class SessionWatcher : IInitializable
    {
        private readonly IAnalyticServices                   analyticServices;
        private readonly IScreenManager                      screenManager;
        private readonly UITemplateGameSessionDataController gameSessionDataController;
        private readonly SignalBus                           signalBus;

        private string SessionId { get; set; }

        private const float HeartbeatInterval = 1f;

        [Preserve]
        public SessionWatcher(IAnalyticServices analyticServices, IScreenManager screenManager, UITemplateGameSessionDataController gameSessionDataController, SignalBus signalBus)
        {
            this.analyticServices          = analyticServices;
            this.screenManager             = screenManager;
            this.gameSessionDataController = gameSessionDataController;
            this.signalBus                 = signalBus;
        }

        public void Initialize()
        {
            this.SessionId = Guid.NewGuid().ToString();
            this.Heartbeat().Forget();

            this.signalBus.Subscribe<ApplicationQuitSignal>(this.OnApplicationQuit);
            this.signalBus.Subscribe<ApplicationPauseSignal>(this.OnApplicationPause);
        }

        private async UniTaskVoid Heartbeat()
        {
            await UniTask.SwitchToMainThread();
            await UniTask.WaitForSeconds(HeartbeatInterval, true);
            this.analyticServices.Track(new CustomEvent()
            {
                EventName = "ut_heartbeat",
                EventProperties = new Dictionary<string, object>
                {
                    { "session_id", this.SessionId },
                    { "game_session_id", this.gameSessionDataController.OpenTime },
                    { "placement", this.screenManager.CurrentActiveScreen?.Value.ScreenId ?? "unknown" }
                },
            });
            this.Heartbeat().Forget();
        }

        private void OnApplicationQuit()
        {
            this.analyticServices.Track(new CustomEvent()
            {
                EventName = "ut_application_quit",
                EventProperties = new Dictionary<string, object>
                {
                    { "session_id", this.SessionId },
                    { "game_session_id", this.gameSessionDataController.OpenTime },
                    { "placement", this.screenManager.CurrentActiveScreen?.Value.ScreenId ?? "unknown" }
                },
            });
        }

        private void OnApplicationPause(ApplicationPauseSignal pauseSignal)
        {
            this.analyticServices.Track(new CustomEvent()
            {
                EventName = pauseSignal.PauseStatus ? "ut_focus_out" : "ut_focus_in",
                EventProperties = new Dictionary<string, object>()
                {
                    { "session_id", this.SessionId },
                    { "game_session_id", this.gameSessionDataController.OpenTime },
                    { "placement", this.screenManager.CurrentActiveScreen?.Value.ScreenId ?? "unknown" }
                }
            });
        }
    }
}