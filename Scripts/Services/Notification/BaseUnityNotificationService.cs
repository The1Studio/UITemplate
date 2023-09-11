namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BlueprintFlow.Signals;
    using Core.AnalyticServices;
    using Core.AnalyticServices.CommonEvents;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.Utilities.Extension;
    using GameFoundation.Scripts.Utilities.LogService;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using UnityEngine;
    using Zenject;

    public abstract class BaseUnityNotificationService : INotificationService
    {
        #region Inject

        protected readonly SignalBus                           SignalBus;
        protected readonly UITemplateNotificationBlueprint     UITemplateNotificationBlueprint;
        protected readonly UITemplateNotificationDataBlueprint UITemplateNotificationDataBlueprint;
        protected readonly ILogService                         Logger;
        protected readonly IAnalyticServices                   AnalyticServices;
        protected readonly NotificationMappingHelper           NotificationMappingHelper;

        #endregion

        protected static string ChannelId          => "default_channel_id";
        protected static string ChannelName        => Application.productName;
        protected static string ChannelDescription => Application.productName;
        protected static string SmallIcon          => "icon_0";
        protected static string LargeIcon          => "icon_1";

        protected BaseUnityNotificationService(SignalBus signalBus, UITemplateNotificationBlueprint uiTemplateNotificationBlueprint,
            UITemplateNotificationDataBlueprint uiTemplateNotificationDataBlueprint, NotificationMappingHelper notificationMappingHelper,
            ILogService logger, IAnalyticServices analyticServices)
        {
            this.SignalBus                           = signalBus;
            this.UITemplateNotificationBlueprint     = uiTemplateNotificationBlueprint;
            this.UITemplateNotificationDataBlueprint = uiTemplateNotificationDataBlueprint;
            this.NotificationMappingHelper           = notificationMappingHelper;
            this.Logger                              = logger;
            this.AnalyticServices                    = analyticServices;

            this.SignalBus.Subscribe<LoadBlueprintDataSucceedSignal>(this.OnLoadBlueprintComplete);
        }

        private async void OnLoadBlueprintComplete(LoadBlueprintDataSucceedSignal obj)
        {
            await this.InitNotification();
            this.SetUpNotification();
        }

        #region Initial

        private async UniTask InitNotification()
        {
            await this.CheckPermission();
            this.RegisterNotification();
            this.CheckOpenedByNotification();
            this.IsInitialized = true;
        }

        protected virtual void RegisterNotification() { }

        protected virtual async void CheckOpenedByNotification() { }

        protected void TrackEventClick(NotificationContent content)
        {
            this.AnalyticServices.Track(new CustomEvent()
            {
                EventName = "LocalNotificationOpened",
                EventProperties = new Dictionary<string, object>()
                {
                    { content.Title, content.Body }
                }
            });
            this.Logger.Log($"onelog: Notification Opened: {content.Title} - {content.Body}");
        }

        #endregion

        #region Schedule Notification

        public               bool    IsInitialized     { get; set; } = false;
        public virtual async UniTask CheckPermission() { }

        public async void SetUpNotification()
        {
            if (!this.IsInitialized) return;

            // Cancels all pending local notifications.
            this.CancelNotification();

            // Prepare the Remind
            var tasks                         = new List<UniTask<NotificationContent>>();
            var uiTemplateNotificationRecords = this.UITemplateNotificationBlueprint.Values.Where(x => x.RandomAble).ToList();

            foreach (var notificationData in uiTemplateNotificationRecords)
            {
                tasks.Add(this.PrepareRemind(notificationData));
            }

            var notificationContents = await UniTask.WhenAll(tasks);
            for (var i = 0; i < uiTemplateNotificationRecords.Count; i++)
            {
                var notificationData = uiTemplateNotificationRecords[i];
                var delayTime        = new TimeSpan(notificationData.TimeToShow[0], notificationData.TimeToShow[1], notificationData.TimeToShow[2]);
                this.ScheduleNotification(notificationData, delayTime, notificationContents[i]);
            }
        }

        public virtual void CancelNotification() { }

        private void ScheduleNotification(UITemplateNotificationRecord notificationData, TimeSpan delayTime, NotificationContent notificationContent = null)
        {
            var fireTime = DateTime.Now.Add(delayTime);
            var highHour = notificationData.HourRangeShow[1];
            var lowHour  = notificationData.HourRangeShow[0];

            this.Logger.Log($"onelog: Notification Schedule: {notificationData.Title} - {notificationData.Body} - {fireTime}");
            if (fireTime.Hour >= highHour || fireTime.Hour <= lowHour) return;

            var title = notificationContent != null ? notificationContent.Title : notificationData.Title;
            var body  = notificationContent != null ? notificationContent.Body : notificationData.Body;

            this.SendNotification(title, body, fireTime, delayTime);
        }

        public virtual void SendNotification(string title, string body, DateTime fireTime, TimeSpan delayTime) { }

        #endregion

        #region Set Up Notification Content

        private async UniTask<NotificationContent> PrepareRemind(UITemplateNotificationRecord record)
        {
            var title = "";
            var body  = "";

            var listCanRandom = this.UITemplateNotificationDataBlueprint.Values.Where(x => x.RandomAble).ToList();
            var itemRandom    = listCanRandom.Count > 0 ? listCanRandom.PickRandom() : null;

            if (itemRandom == null)
            {
                this.Logger.Warning($"There is no item can random in {this.UITemplateNotificationDataBlueprint.GetType().Name}, ignore random");
            }

            if (record.Title.Equals("Random"))
            {
                if (itemRandom != null)
                {
                    title = await this.NotificationMappingHelper.GetFormatString(itemRandom.Title);
                }
            }
            else
            {
                title = await this.NotificationMappingHelper.GetFormatString(this.UITemplateNotificationDataBlueprint[record.Title].Title);
            }

            if (record.Body.Equals("Random"))
            {
                if (itemRandom != null)
                {
                    body = await this.NotificationMappingHelper.GetFormatString(itemRandom.Body);
                }
            }
            else
            {
                body = await this.NotificationMappingHelper.GetFormatString(this.UITemplateNotificationDataBlueprint[record.Body].Body);
            }

            return new NotificationContent(title, body);
        }

        public void SetupCustomNotification(string notificationId, TimeSpan? delayTime = null)
        {
            if (!this.UITemplateNotificationBlueprint.TryGetValue(notificationId, out var notificationData)) return;

            delayTime ??= new TimeSpan(notificationData.TimeToShow[0], notificationData.TimeToShow[1], notificationData.TimeToShow[2]);
            this.ScheduleNotification(notificationData, delayTime.Value);
        }

        #endregion
    }
}