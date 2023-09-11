namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using System;
    using Cysharp.Threading.Tasks;

    public interface INotificationService
    {
        bool IsInitialized { get; }

        UniTask CheckPermission();
        void    SetUpNotification();
        void    CancelNotification();
        void    SendNotification(string title, string body, DateTime fireTime, TimeSpan delayTime);
        void    SetupCustomNotification(string notificationId, TimeSpan? delayTime = null);
    }
}