using System;
using System.Collections.Generic;
using UnityEngine;

namespace EasyMobile
{
    /// <summary>
    /// All settings of the App Tracking submodule of the Privacy module.
    /// </summary>
    [Serializable]
    public class AppTrackingSettings : IIOSInfoItemRequired
    {
        #region IIOSInfoItemRequired implementation

        [SerializeField]
        private List<iOSInfoPlistItem> mIOSInfoPlistKeys = new List<iOSInfoPlistItem>
        {
            new iOSInfoPlistItem(iOSInfoPlistItem.NSUserTrackingUsageDescription),
        };

        public List<iOSInfoPlistItem> GetIOSInfoPlistKeys()
        {
            return mIOSInfoPlistKeys;
        }

        #endregion
    }
}
