#if UNITY_IOS && EM_ATT
using System;
using UnityEngine;
using EasyMobile.iOS.AppTrackingTransparency;

namespace EasyMobile.Internal.Privacy
{
    internal class iOSAppTrackingManager : IAppTrackingManager
    {
        public AppTrackingAuthorizationStatus TrackingAuthorizationStatus
        {
            get { return (AppTrackingAuthorizationStatus)ATTrackingManager.TrackingAuthorizationStatus; }
        }

        public void RequestTrackingAuthorization(Action<AppTrackingAuthorizationStatus> callback)
        {
            Util.NullArgumentTest(callback);
            callback = RuntimeHelper.ToMainThread(callback);
            ATTrackingManager.RequestTrackingAuthorization(status =>
            {
                callback((AppTrackingAuthorizationStatus)status);
            });
        }
    }
}
#endif