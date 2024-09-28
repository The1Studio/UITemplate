namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using System;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.Utilities.LogService;
    using UnityEngine.Scripting;

    public class DummyNotificationService : INotificationService
    {
        #region inject

        private readonly ILogService logService;

        #endregion

        [Preserve]
        public DummyNotificationService(ILogService logService) { this.logService = logService; }

        public bool IsInitialized { get; set; }

        public void SendNotification(string title, string body, DateTime fireTime, TimeSpan delayTime) { }

        public void SetupCustomNotification(string notificationId, TimeSpan? delayTime = null) { this.logService.Log($"Notify: Id - {notificationId}, delay - {delayTime}"); }

        public UniTask CheckPermission() { return UniTask.CompletedTask; }

        public void SetUpNotification()  { }
        public void CancelNotification() { }
    }
}