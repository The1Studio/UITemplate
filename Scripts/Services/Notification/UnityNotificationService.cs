namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using System;
    using System.Linq;
    using BlueprintFlow.Signals;
    using Core.AnalyticServices;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.Utilities.Extension;
    using GameFoundation.Scripts.Utilities.LogService;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using Unity.Notifications.Android;
    using Unity.Notifications.iOS;
    using UnityEngine;
    using UnityEngine.Android;
    using Zenject;

    public class UnityNotificationService : INotificationService
    {
        #region Inject

        private readonly SignalBus                           signalBus;
        private readonly UITemplateNotificationBlueprint     uiTemplateNotificationBlueprint;
        private readonly UITemplateNotificationDataBlueprint uiTemplateNotificationDataBlueprint;
        private readonly ILogService                         logger;

        #endregion

        private static string ChannelId          => "default_channel_id";
        private static string ChannelName        => Application.productName;
        private static string ChannelDescription => Application.productName;
        protected      string SmallIcon          => "small";
        protected      string LargeIcon          => "large";

        public UnityNotificationService(SignalBus signalBus, UITemplateNotificationBlueprint uiTemplateNotificationBlueprint, UITemplateNotificationDataBlueprint uiTemplateNotificationDataBlueprint,
            ILogService logger)
        {
            this.signalBus                           = signalBus;
            this.uiTemplateNotificationBlueprint     = uiTemplateNotificationBlueprint;
            this.uiTemplateNotificationDataBlueprint = uiTemplateNotificationDataBlueprint;
            this.logger                              = logger;

            this.signalBus.Subscribe<LoadBlueprintDataSucceedSignal>(this.OnLoadBlueprintComplete);
        }

        private void OnLoadBlueprintComplete(LoadBlueprintDataSucceedSignal obj)
        {
            this.InitNotification();
        }

        #region Initial

        private void InitNotification()
        {
            this.CheckPermission();
            
            var channel = new AndroidNotificationChannel(ChannelId, ChannelName, ChannelDescription, Importance.Default);
            AndroidNotificationCenter.RegisterNotificationChannel(channel);

            this.SetUpNotification();
        }

        #endregion

        #region Schedule Notification
        
        private void CheckPermission()
        {
#if UNITY_ANDROID
            if (AndroidNotificationCenter.UserPermissionToPost is not (PermissionStatus.RequestPending or PermissionStatus.NotRequested)) return;

            if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
            {
                Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
            }
#elif UNITY_IOS
            var iOSNotificationSettings = iOSNotificationCenter.GetNotificationSettings();
            if (iOSNotificationSettings.AuthorizationStatus == AuthorizationStatus.NotDetermined)
            {
                using var req = new AuthorizationRequest(AuthorizationOption.Alert | AuthorizationOption.Badge, true);
            }
#endif
            
            
        }
        private void SetUpNotification()
        {
            // Cancels all pending local notifications.
#if UNITY_ANDROID
            AndroidNotificationCenter.CancelAllNotifications();
#elif UNITY_IOS
            iOSNotificationCenter.RemoveAllScheduledNotifications();
#endif

            // Prepare the Remind
            foreach (var notificationData in this.uiTemplateNotificationBlueprint.Values.Where(x => x.RandomAble))
            {
                var delayTime = new TimeSpan(notificationData.TimeToShow[0], notificationData.TimeToShow[1], notificationData.TimeToShow[2]);
                this.ScheduleNotification(notificationData, delayTime, this.PrepareRemind(notificationData));
            }
        }

        private void ScheduleNotification(UITemplateNotificationRecord notificationData, TimeSpan delayTime, NotificationContent notificationContent = null)
        {
            var fireTime = DateTime.Now.Date.AddHours(delayTime.Hours);
            var highHour = notificationData.HourRangeShow[1];
            var lowHour  = notificationData.HourRangeShow[0];

            if (fireTime.Hour >= highHour || fireTime.Hour <= lowHour) return;

            var title = notificationContent != null ? notificationContent.Title : notificationData.Title;
            var body  = notificationContent != null ? notificationContent.Body : notificationData.Body;
            var notification = new AndroidNotification
            {
                Title     = title,
                Text      = body,
                SmallIcon = this.SmallIcon,
                LargeIcon = this.LargeIcon,
                FireTime  = fireTime
            };
            AndroidNotificationCenter.SendNotification(notification, ChannelId);
        }

        #endregion

        #region Set Up Notification Content

        private NotificationContent PrepareRemind(UITemplateNotificationRecord record)
        {
            var title = "";
            var body  = "";

            var listCanRandom = this.uiTemplateNotificationDataBlueprint.Values.Where(x => x.RandomAble).ToList();
            var itemRandom    = listCanRandom.Count > 0 ? listCanRandom.PickRandom() : null;

            if (itemRandom == null)
            {
                this.logger.Warning($"There is no item can random in {this.uiTemplateNotificationDataBlueprint.GetType().Name}, ignore random");
            }

            if (record.Title.Equals("Random"))
            {
                if (itemRandom != null)
                {
                    title = itemRandom.GetTitle(new object[] { Application.productName });
                }
            }
            else
            {
                title = this.uiTemplateNotificationDataBlueprint[record.Title].GetTitle(new object[] { Application.productName });
            }

            if (record.Body.Equals("Random"))
            {
                if (itemRandom != null)
                {
                    body = itemRandom.GetBody(new object[] { Application.productName });
                }
            }
            else
            {
                body = this.uiTemplateNotificationDataBlueprint[record.Body].GetBody(new object[] { Application.productName });
            }

            return new NotificationContent(title, body);
        }

        #endregion

        public void SetupCustomNotification(string notificationId, TimeSpan? delayTime = null)
        {
            if (!this.uiTemplateNotificationBlueprint.TryGetValue(notificationId, out var notificationData))
            {
                return;
            }

            delayTime ??= new TimeSpan(notificationData.TimeToShow[0], notificationData.TimeToShow[1], notificationData.TimeToShow[2]);
            this.ScheduleNotification(notificationData, delayTime.Value);
        }
    }
}