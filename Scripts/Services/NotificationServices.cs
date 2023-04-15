namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BlueprintFlow.Signals;
    using Core.AnalyticServices;
    using Core.AnalyticServices.CommonEvents;
    using Cysharp.Threading.Tasks;
    using EasyMobile;
    using GameFoundation.Scripts.Utilities.Extension;
    using GameFoundation.Scripts.Utilities.LogService;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Scripts.Signals;
    using UnityEngine;
    using Zenject;
    using LocalNotification = EasyMobile.LocalNotification;
    using Random = UnityEngine.Random;
    using RemoteNotification = EasyMobile.RemoteNotification;

    /// <summary>
    /// For handle notification
    /// </summary>
    public class NotificationServices
    {
        private readonly SignalBus                           signalBus;
        private readonly UITemplateNotificationBlueprint     uiTemplateNotificationBlueprint;
        private readonly UITemplateNotificationDataBlueprint uiTemplateNotificationDataBlueprint;
        private readonly ILogService                         logger;
        private readonly IAnalyticServices                   analyticHandler;
        private          bool                                isLoadBlueprintComplete;

        public NotificationServices(SignalBus signalBus, UITemplateNotificationBlueprint uiTemplateNotificationBlueprint, UITemplateNotificationDataBlueprint uiTemplateNotificationDataBlueprint,
            ILogService logger, IAnalyticServices analyticServices)
        {
            this.signalBus                           = signalBus;
            this.uiTemplateNotificationBlueprint     = uiTemplateNotificationBlueprint;
            this.uiTemplateNotificationDataBlueprint = uiTemplateNotificationDataBlueprint;
            this.logger                              = logger;
            this.analyticHandler                     = analyticServices;
#if FIREBASE_REMOTE_CONFIG
            this.signalBus.Subscribe<RemoteConfigInitializeSucceededSignal>(this.InitNotification);
#endif
            this.signalBus.Subscribe<LoadBlueprintDataSucceedSignal>(this.OnLoadBlueprintComplete);
        }

        private void OnLoadBlueprintComplete(LoadBlueprintDataSucceedSignal obj)
        {
            this.isLoadBlueprintComplete = true;
#if !FIREBASE_REMOTE_CONFIG
            this.InitNotification();
#endif
        }

        #region Initial

        private async void InitNotification()
        {
            await UniTask.WaitUntil(() => this.isLoadBlueprintComplete);
            Notifications.Init();

            while (!Notifications.IsInitialized())
            {
                await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
            }

            Notifications.LocalNotificationOpened  += this.OnLocalNotificationOpened;
            Notifications.RemoteNotificationOpened += this.OnRemoteNotificationOpened;
            Notifications.PushTokenReceived        += this.OnPushTokenReceived;
            this.SetUpNotification();
        }

        #endregion

        #region Event Open

        private void OnLocalNotificationOpened(LocalNotification delivered)
        {
            var content = delivered.content;

            this.analyticHandler.Track(new CustomEvent()
            {
                EventName = "LocalNotificationOpened",
                EventProperties = new Dictionary<string, object>()
                {
                    { content.title, content.body }
                }
            });
        }

        private void OnRemoteNotificationOpened(RemoteNotification delivered)
        {
            var content = delivered.firebasePayload;

            foreach (var data in content.Data)
            {
                this.analyticHandler.Track(new CustomEvent()
                {
                    EventName = "OnRemoteNotificationOpened",
                    EventProperties = new Dictionary<string, object>()
                    {
                        { data.Key, data.Value }
                    }
                });
            }
        }

        private void OnPushTokenReceived(string obj) { this.logger.Log($"Debug push token: {obj}"); }

        #endregion

        #region Schedule Notification

        private void SetUpNotification()
        {
            // Cancels all pending local notifications.
            Notifications.CancelAllPendingLocalNotifications();

            // Prepare the Remind 
            var currentHour = DateTime.Now.Hour;

            foreach (var notificationData in this.uiTemplateNotificationBlueprint.Where(x => x.Value.RandomAble))
            {
                var delayTime = new TimeSpan(notificationData.Value.TimeToShow[0], notificationData.Value.TimeToShow[1], notificationData.Value.TimeToShow[2]);

                var hourToShow = currentHour + delayTime.Hours;
                var highHour   = notificationData.Value.HourRangeShow[1];
                var lowHour    = notificationData.Value.HourRangeShow[0];

                if (hourToShow < highHour && hourToShow > lowHour)
                {
                    Notifications.ScheduleLocalNotification(delayTime,
                        this.PrepareRemind(notificationData.Value),
                        Enum.TryParse<NotificationRepeat>(notificationData.Value.Repeat, out var result) ? result : NotificationRepeat.None);
                }
            }
        }

        #endregion

        #region Set Up Notification Content

        private NotificationContent PrepareRemind(UITemplateNotificationRecord record)
        {
            var title = "";
            var body  = "";

            var listCanRandom = this.uiTemplateNotificationDataBlueprint.Values.Where(x => x.RandomAble).ToList();

            UITemplateNotificationDataRecord itemRandom = null;

            if (listCanRandom.Count > 0)
            {
                itemRandom = listCanRandom.PickRandom();
            }

            if (itemRandom == null)
            {
                this.logger.Warning($"There is no item can random in {this.uiTemplateNotificationDataBlueprint.GetType().Name}, ignore random");
            }

            if (record.Title.Equals("Random"))
            {
                if (itemRandom != null)
                {
                    title = itemRandom.GetTitle(new object[]
                    {
                        Application.productName
                    });
                }
            }
            else
            {
                title = this.uiTemplateNotificationDataBlueprint[record.Title].GetTitle(new object[]
                {
                    Application.productName
                });
            }

            if (record.Body.Equals("Random"))
            {
                if (itemRandom != null)
                {
                    body = itemRandom.GetBody(new object[]
                    {
                        Application.productName
                    });
                }
            }
            else
            {
                body = this.uiTemplateNotificationDataBlueprint[record.Body].GetBody(new object[]
                {
                    Application.productName
                });
            }

            var content = new NotificationContent
            {
                title = title,
                body  = body
            };

            return content;
        }

        #endregion

        public void SetupCustomNotification(string notificationId)
        {
            if (!this.uiTemplateNotificationBlueprint.TryGetValue(notificationId, out var notificationData))
            {
                return;
            }

            var currentHour = DateTime.Now.Hour;
            var delayTime   = new TimeSpan(notificationData.TimeToShow[0], notificationData.TimeToShow[1], notificationData.TimeToShow[2]);

            var hourToShow = currentHour + delayTime.Hours;
            var highHour   = notificationData.HourRangeShow[1];
            var lowHour    = notificationData.HourRangeShow[0];

            if (hourToShow < highHour && hourToShow > lowHour)
            {
                Notifications.ScheduleLocalNotification(delayTime,
                    this.PrepareRemind(notificationData),
                    Enum.TryParse<NotificationRepeat>(notificationData.Repeat, out var result) ? result : NotificationRepeat.None);
            }
        }

        public void SetupCustomNotification(string notificationId, TimeSpan delayTime)
        {
            if (!this.uiTemplateNotificationBlueprint.TryGetValue(notificationId, out var notificationData))
            {
                return;
            }

            var currentHour = DateTime.Now.Hour;
            var hourToShow  = currentHour + delayTime.Hours;
            var highHour    = notificationData.HourRangeShow[1];
            var lowHour     = notificationData.HourRangeShow[0];

            if (hourToShow < highHour && hourToShow > lowHour)
            {
                Notifications.ScheduleLocalNotification(delayTime,
                    this.PrepareRemind(notificationData),
                    Enum.TryParse<NotificationRepeat>(notificationData.Repeat, out var result) ? result : NotificationRepeat.None);
            }
        }
    }
}