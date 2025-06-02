namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using System;
    using Cysharp.Threading.Tasks;
    using TheOne.Logging;
    using UnityEngine.Scripting;

    public class DummyNotificationService : INotificationService
    {
        #region inject

        private readonly ILogger logger;

        #endregion

        [Preserve]
        public DummyNotificationService(ILoggerManager loggerManager)
        {
            this.logger = loggerManager.GetLogger(this);
        }

        public bool IsInitialized { get; set; }

        public void SendNotification(string title, string body, DateTime fireTime, TimeSpan delayTime)
        {
        }

        public void SetupCustomNotification(string notificationId, TimeSpan? delayTime = null)
        {
            this.logger.Info($"Notify: Id - {notificationId}, delay - {delayTime}");
        }

        public UniTask CheckPermission()
        {
            return UniTask.CompletedTask;
        }

        public void SetUpNotification()
        {
        }

        public void CancelNotification()
        {
        }
    }
}