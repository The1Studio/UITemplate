using System;

namespace EasyMobile
{
    /// <summary>
    /// App Tracking Transparency API.
    /// </summary>
    public interface IAppTrackingManager
    {
        /// <summary>
        /// The status values for app tracking authorization.
        /// Note that on iOS older than 14, the returned status is always <code>ATTrackingManagerAuthorizationStatusAuthorized</code>
        /// because IDFA access is always available on those systems as if authorized by the user.
        /// On Android and non-supported platforms it will return <code>ATTrackingManagerAuthorizationStatusNotDetermined</code>.
        /// </summary>
        AppTrackingAuthorizationStatus TrackingAuthorizationStatus { get; }

        /// <summary>
        /// The request for user authorization to access app-related data.
        /// Note that on iOS older than 14, no popup will be shown and the callback will be invoked immediately
        /// with the status <code>ATTrackingManagerAuthorizationStatusAuthorized</code>
        /// because IDFA access is always available on those systems as if authorized by the user.
        /// On Android and non-supported platforms, no popup will be shown and the callback will be invoked immediately
        /// with the status <code>ATTrackingManagerAuthorizationStatusNotDetermined</code>.
        /// </summary>
        /// <remarks>
        /// The ATT popup is a one-time request to authorize or deny access to app-related data that can be used for tracking the user or the device.
        /// The system remembers the user’s choice and doesn’t prompt again unless a user uninstalls and then reinstalls the app on the device.
        /// </remarks>
        /// <param name="callback"></param>
        void RequestTrackingAuthorization(Action<AppTrackingAuthorizationStatus> callback);
    }
}