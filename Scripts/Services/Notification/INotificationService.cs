namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using System;

    public interface INotificationService
    {
        void SetupCustomNotification(string notificationId, TimeSpan? delayTime = null);
    }
}