﻿namespace TheOneStudio.UITemplate.UITemplate.Services.Permissions
{
    using System;

    public static class PermissionHelper
    {
        public static string ToPermissionString(this PermissionRequest permission)
        {
            return permission switch
            {
                PermissionRequest.Camera               => UnityEngine.Android.Permission.Camera,
                PermissionRequest.Microphone           => UnityEngine.Android.Permission.Microphone,
                PermissionRequest.FineLocation         => UnityEngine.Android.Permission.FineLocation,
                PermissionRequest.CoarseLocation       => UnityEngine.Android.Permission.CoarseLocation,
                PermissionRequest.ExternalStorageRead  => UnityEngine.Android.Permission.ExternalStorageRead,
                PermissionRequest.ExternalStorageWrite => UnityEngine.Android.Permission.ExternalStorageWrite,
                PermissionRequest.Notification         => "android.permission.POST_NOTIFICATIONS",
                _                                      => throw new ArgumentOutOfRangeException(nameof(permission), permission, "Permission Not Implemented")
            };
        }

        public static int GetSDKVersionInt()
        {
#if !UNITY_EDITOR && UNITY_ANDROID
            using (var version = new UnityEngine.AndroidJavaClass("android.os.Build$VERSION"))
            {
                return version.GetStatic<int>("SDK_INT");
            }
#else
            return 30;
#endif
        }
    }
}