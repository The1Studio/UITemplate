namespace Core.AnalyticServices.Data
{
    using System;
    using System.Collections.Generic;
    using Core.AnalyticServices.CommonEvents;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using UnityEngine;

    internal sealed class SessionWatcher : MonoBehaviour
    {
        private IAnalyticServices                   analyticServices;
        private IScreenManager                      screenManager;
        private UITemplateGameSessionDataController gameSessionDataController;

        private string SessionId { get; set; }


        private const float HeartbeatInterval = 1f;

        public void Construct(IAnalyticServices analyticServices, IScreenManager screenManager, UITemplateGameSessionDataController gameSessionDataController)
        {
            this.analyticServices          = analyticServices;
            this.screenManager             = screenManager;
            this.gameSessionDataController = gameSessionDataController;
            this.Init();
        }

        private void Init() { this.SessionId = Guid.NewGuid().ToString(); }

        private void Start() { this.Heartbeat().Forget(); }

        private async UniTaskVoid Heartbeat()
        {
            await UniTask.WaitForSeconds(HeartbeatInterval);
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

        private void OnApplicationFocus(bool hasFocus)
        {
            this.analyticServices.Track(new CustomEvent()
            {
                EventName = hasFocus ? "ut_focus_in" : "ut_focus_out",
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