using System;

namespace EasyMobile
{
    public enum AppTrackingAuthorizationStatus
    {
        /// <summary>
        /// The value returned if the user authorizes access to app-related data
        /// that can be used for tracking the user or the device.
        /// </summary>
        ATTrackingManagerAuthorizationStatusAuthorized = 0,
        /// <summary>
        /// The value returned if the user denies authorization to access app-related data
        /// that can be used for tracking the user or the device.
        /// </summary>
        ATTrackingManagerAuthorizationStatusDenied,
        /// <summary>
        /// The value returned if a user has not yet received a request to authorize access
        /// to app-related data that can be used for tracking the user or the device.
        /// </summary>
        ATTrackingManagerAuthorizationStatusNotDetermined,
        /// <summary>
        /// The value returned if authorization to access app-related data that can be used
        /// for tracking the user or the device is restricted.
        /// </summary>
        ATTrackingManagerAuthorizationStatusRestricted
    }
}