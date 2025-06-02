#if THEONE_NOTIFICATION && UNITY_IOS
namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using Core.AnalyticServices;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using GameFoundation.Signals;
    using System;
    using Cysharp.Threading.Tasks;
    using TheOne.Logging;
    using TheOneStudio.UITemplate.UITemplate.Services.Permissions;
    using Unity.Notifications.iOS;
    using UnityEngine.Scripting;

    public class IOSUnityNotificationService : BaseUnityNotificationService
    {
        [Preserve]
        public IOSUnityNotificationService(
            SignalBus                           signalBus,
            UITemplateNotificationBlueprint     uiTemplateNotificationBlueprint,
            UITemplateNotificationDataBlueprint uiTemplateNotificationDataBlueprint,
            NotificationMappingHelper           notificationMappingHelper,
            ILoggerManager                      loggerManager,
            IAnalyticServices                   analyticServices,
            IPermissionService                  permissionService
        ) : base(signalBus, uiTemplateNotificationBlueprint, uiTemplateNotificationDataBlueprint, notificationMappingHelper, loggerManager, analyticServices, permissionService)
        {
        }

        protected override async void CheckOpenedByNotification()
        {
            await UniTask.DelayFrame(1); // await 1 frame are required to get the last notification intent

            var intent = iOSNotificationCenter.GetLastRespondedNotification();
            if (intent != null) this.TrackEventClick(new NotificationContent(intent.Title, intent.Body));
        }

        public override void CancelNotification()
        {
            iOSNotificationCenter.RemoveAllScheduledNotifications();
            iOSNotificationCenter.ApplicationBadge = 0;
        }

        public override void SendNotification(string title, string body, DateTime fireTime, TimeSpan delayTime)
        {
            var notification = new iOSNotification()
            {
                Title   = title,
                Body    = body,
                Trigger = new iOSNotificationTimeIntervalTrigger() { TimeInterval = delayTime },
                Badge   = 1,
            };
            iOSNotificationCenter.ScheduleNotification(notification);
        }
    }
}

#endif