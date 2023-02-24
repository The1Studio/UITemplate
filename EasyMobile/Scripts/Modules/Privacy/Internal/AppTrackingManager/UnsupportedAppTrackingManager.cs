using System;
using UnityEngine;

namespace EasyMobile.Internal.Privacy
{
    internal class UnsupportedAppTrackingManager : IAppTrackingManager
    {
        protected const string UnsupportedMessage = "App Tracking Transparency is only supported on iOS 14 or newer.";

        public AppTrackingAuthorizationStatus TrackingAuthorizationStatus
        {
            get { return AppTrackingAuthorizationStatus.ATTrackingManagerAuthorizationStatusNotDetermined; }
        }

        public void RequestTrackingAuthorization(Action<AppTrackingAuthorizationStatus> callback)
        {
            Debug.LogWarning(UnsupportedMessage);
            if (callback != null)
                callback(AppTrackingAuthorizationStatus.ATTrackingManagerAuthorizationStatusNotDetermined);
        }
    }
}