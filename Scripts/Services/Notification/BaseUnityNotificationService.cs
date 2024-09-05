namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using BlueprintFlow.Signals;
    using Core.AnalyticServices;
    using Core.AnalyticServices.CommonEvents;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.Utilities.Extension;
    using GameFoundation.Scripts.Utilities.LogService;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Services.Permissions;
    using UnityEngine;

    public abstract class BaseUnityNotificationService : INotificationService, IDisposable
    {
        #region Inject

        protected readonly SignalBus                           SignalBus;
        protected readonly UITemplateNotificationBlueprint     UITemplateNotificationBlueprint;
        protected readonly UITemplateNotificationDataBlueprint UITemplateNotificationDataBlueprint;
        protected readonly ILogService                         Logger;
        protected readonly IAnalyticServices                   AnalyticServices;
        protected readonly IPermissionService                   PermissionService;
        protected readonly NotificationMappingHelper           NotificationMappingHelper;

        #endregion

        private CancellationTokenSource ctsSetupNotification;

        protected string ChannelId          { get; set; } = "default_channel_id";
        protected string ChannelName        { get; set; } = Application.productName;
        protected string ChannelDescription { get; set; } = Application.productName;
        protected string SmallIcon          { get; set; } = "icon_0";
        protected string LargeIcon          { get; set; } = "icon_1";

        protected BaseUnityNotificationService(SignalBus signalBus, UITemplateNotificationBlueprint uiTemplateNotificationBlueprint,
            UITemplateNotificationDataBlueprint uiTemplateNotificationDataBlueprint, NotificationMappingHelper notificationMappingHelper,
            ILogService logger, IAnalyticServices analyticServices, IPermissionService permissionService)
        {
            this.SignalBus                           = signalBus;
            this.UITemplateNotificationBlueprint     = uiTemplateNotificationBlueprint;
            this.UITemplateNotificationDataBlueprint = uiTemplateNotificationDataBlueprint;
            this.NotificationMappingHelper           = notificationMappingHelper;
            this.Logger                              = logger;
            this.AnalyticServices                    = analyticServices;
            this.PermissionService                   = permissionService;

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
#if LOCALIZATION
            UnityEngine.Localization.Settings.LocalizationSettings.SelectedLocaleChanged += this.OnLanguageChange;
#endif
            this.IsInitialized = true;
        }

        public void Dispose()
        {
#if LOCALIZATION
            UnityEngine.Localization.Settings.LocalizationSettings.SelectedLocaleChanged -= this.OnLanguageChange;
#endif
        }

#if LOCALIZATION
        private void OnLanguageChange(UnityEngine.Localization.Locale obj) { this.SetUpNotification(); }
#endif

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

        public async UniTask CheckPermission() { await this.PermissionService.RequestPermission(PermissionRequest.Notification);}

        public async void SetUpNotification()
        {
            if (!this.IsInitialized) return;

            try
            {
                this.ctsSetupNotification?.Cancel();
                this.ctsSetupNotification = new CancellationTokenSource();

                // Cancels all pending local notifications.
                this.CancelNotification();

                // Prepare the Remind
                var tasks                         = new List<UniTask<NotificationContent>>();
                var uiTemplateNotificationRecords = this.UITemplateNotificationBlueprint.Values.Where(x => x.RandomAble).ToList();

                foreach (var notificationData in uiTemplateNotificationRecords)
                {
                    tasks.Add(this.PrepareRemind(notificationData));
                }

                var notificationContents = await UniTask.WhenAll(tasks).AttachExternalCancellation(this.ctsSetupNotification.Token);
                for (var i = 0; i < uiTemplateNotificationRecords.Count; i++)
                {
                    var notificationData = uiTemplateNotificationRecords[i];
                    var delayTime        = new TimeSpan(notificationData.TimeToShow[0], notificationData.TimeToShow[1], notificationData.TimeToShow[2]);
                    this.ScheduleNotification(notificationData, delayTime, notificationContents[i]);
                }
            }
            catch (Exception e)
            {
                // ignored
            }
        }

        public virtual void CancelNotification() { }

        private void ScheduleNotification(UITemplateNotificationRecord notificationData, TimeSpan delayTime, NotificationContent notificationContent)
        {
            var fireTime = DateTime.Now.Add(delayTime);
            var highHour = notificationData.HourRangeShow[1];
            var lowHour  = notificationData.HourRangeShow[0];

            this.Logger.Log($"onelog: Notification Schedule: {notificationContent.Title} - {notificationContent.Body} - {fireTime}");
            if (fireTime.Hour >= highHour || fireTime.Hour <= lowHour) return;

            this.SendNotification(notificationContent.Title, notificationContent.Body, fireTime, delayTime);
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

        public async void SetupCustomNotification(string notificationId, TimeSpan? delayTime = null)
        {
            if (!this.UITemplateNotificationBlueprint.TryGetValue(notificationId, out var notification)) return;
            if (!this.UITemplateNotificationDataBlueprint.TryGetValue(notificationId, out var notificationData)) return;
            var title = await this.NotificationMappingHelper.GetFormatString(notificationData.Title);
            var body  = await this.NotificationMappingHelper.GetFormatString(notificationData.Body);

            delayTime ??= new TimeSpan(notification.TimeToShow[0], notification.TimeToShow[1], notification.TimeToShow[2]);
            this.ScheduleNotification(notification, delayTime.Value, new NotificationContent(title, body));
        }

        #endregion

        #region Setter

        public void SetChannelId(string val)          { this.ChannelId          = val; }
        public void SetChannelName(string val)        { this.ChannelName        = val; }
        public void SetChannelDescription(string val) { this.ChannelDescription = val; }
        public void SetSmallIcon(string val)          { this.SmallIcon          = val; }
        public void SetLargeIcon(string val)          { this.LargeIcon          = val; }

        #endregion
    }
}