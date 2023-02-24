#if UNITY_IOS && EM_ATT
using System;
using System.Runtime.InteropServices;
using AOT;
using EasyMobile.Internal;
using EasyMobile.Internal.iOS;

namespace EasyMobile.iOS.AppTrackingTransparency
{
    internal class ATTrackingManager : iOSObjectProxy
    {
        protected const string FrameworkName = "AppTrackingTransparency";

        /// <summary>
        /// The status values for app tracking authorization.
        /// </summary>
        public enum ATTrackingManagerAuthorizationStatus
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

        internal ATTrackingManager(IntPtr selfPointer) : base(selfPointer)
        {
        }

        /// <summary>
        /// The authorization status that is current for the calling application.
        /// </summary>
        public static ATTrackingManagerAuthorizationStatus TrackingAuthorizationStatus
        {
            get
            {
                return C.ATTrackingManager_trackingAuthorizationStatus();
            }

        }

        /// <summary>
        /// The request for user authorization to access app-related data.
        /// This method is intended to be used on iOS 14 or newer. On older systems the
        /// result will always be ATTrackingManagerAuthorizationStatusNotDetermined.
        /// </summary>
        /// <remarks>
        /// The requestTrackingAuthorizationWithCompletionHandler: is the one-time request
        /// to authorize or deny access to app-related data that can be used for tracking the user or the device.
        /// The system remembers the user’s choice and doesn’t prompt again unless a user uninstalls and then reinstalls the app on the device.
        /// </remarks>
        /// <param name="completionHandler"></param>
        public static void RequestTrackingAuthorization(Action<ATTrackingManagerAuthorizationStatus> completionHandler)
        {
            C.ATTrackingManager_RequestTrackingAuthorizationWithCompletionHandler(
                completionHandler == null ? (C.RequestTrackingAuthorizationCallback)null : InternalRequestTrackingAuthorizationCallback,
                completionHandler == null ? IntPtr.Zero : PInvokeCallbackUtil.ToIntPtr(completionHandler));
        }

        [MonoPInvokeCallback(typeof(C.RequestTrackingAuthorizationCallback))]
        private static void InternalRequestTrackingAuthorizationCallback(ATTrackingManagerAuthorizationStatus status, IntPtr secondaryCallback)
        {
            if (secondaryCallback != IntPtr.Zero)
            {
                PInvokeCallbackUtil.PerformInternalCallback(
                    "ATTrackingManager#InternalRequestTrackingAuthorizationCallback",
                    PInvokeCallbackUtil.Type.Temporary,
                    status,
                    secondaryCallback);
            }
        }

        #region C wrapper

        private static class C
        {
            internal delegate void RequestTrackingAuthorizationCallback(ATTrackingManagerAuthorizationStatus status, IntPtr secondaryCallback);

            [DllImport("__Internal")]
            internal static extern void ATTrackingManager_RequestTrackingAuthorizationWithCompletionHandler(
                                          RequestTrackingAuthorizationCallback callback,
                                          IntPtr secondaryCallback);

            [DllImport("__Internal")]
            internal static extern ATTrackingManagerAuthorizationStatus ATTrackingManager_trackingAuthorizationStatus();
        }

        #endregion
    }
}
#endif