namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using System;
    using GameFoundation.Scripts.Utilities.LogService;

    public class DummyNotificationService : INotificationService
    {
        #region inject

        private readonly ILogService logService;

        #endregion

        public DummyNotificationService(ILogService logService) { this.logService = logService; }

        public void SetupCustomNotification(string notificationId, TimeSpan? delayTime = null) { this.logService.Log($"Notify: Id - {notificationId}, delay - {delayTime}"); }
        public void SetUpNotification()                                                        { }
    }
}