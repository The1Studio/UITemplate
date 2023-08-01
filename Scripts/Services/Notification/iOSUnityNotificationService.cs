#if NOTIFICATION && UNITY_IOS
namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using Core.AnalyticServices;
    using GameFoundation.Scripts.Utilities.LogService;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using Zenject;
    using System;
    using Cysharp.Threading.Tasks;
    using Unity.Notifications.iOS;

    public class IOSUnityNotificationService : BaseUnityNotificationService
    {
        public IOSUnityNotificationService(SignalBus signalBus, UITemplateNotificationBlueprint uiTemplateNotificationBlueprint,
            UITemplateNotificationDataBlueprint uiTemplateNotificationDataBlueprint, NotificationMappingHelper notificationMappingHelper, ILogService logger, IAnalyticServices analyticServices) :
            base(signalBus, uiTemplateNotificationBlueprint, uiTemplateNotificationDataBlueprint, notificationMappingHelper, logger, analyticServices)
        {
        }

        protected override void RegisterNotification() { }

        protected override async void CheckOpenedByNotification()
        {
            await UniTask.DelayFrame(1); // await 1 frame are required to get the last notification intent

            var intent = iOSNotificationCenter.GetLastRespondedNotification();
            if (intent != null)
                this.TrackEventClick(new NotificationContent(intent.Title, intent.Body));
        }

        protected override async UniTask CheckPermission()
        {
            var iOSNotificationSettings = iOSNotificationCenter.GetNotificationSettings();
            if (iOSNotificationSettings.AuthorizationStatus == AuthorizationStatus.NotDetermined)
            {
                using var req = new AuthorizationRequest(AuthorizationOption.Alert | AuthorizationOption.Badge, true);
                await UniTask.WaitUntil(() => req.IsFinished);
            }
        }

        protected override void CancelNotification() { iOSNotificationCenter.RemoveAllScheduledNotifications(); }

        protected override void SendNotification(string title, string body, DateTime fireTime, TimeSpan delayTime)
        {
            var notification = new iOSNotification()
            {
                Title = title,
                Body = body,
                Trigger = new iOSNotificationTimeIntervalTrigger() { TimeInterval = delayTime }
            };
            iOSNotificationCenter.ScheduleNotification(notification);
        }
    }
}

#endif