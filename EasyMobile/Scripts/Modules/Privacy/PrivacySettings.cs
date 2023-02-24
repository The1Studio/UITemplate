using System;
using System.Collections.Generic;
using UnityEngine;

namespace EasyMobile
{
    [Serializable]
    public class PrivacySettings
    {
        [SerializeField]
        private ConsentDialog mDefaultConsentDialog = null;

        [SerializeField]
        private ConsentDialogComposerSettings mConsentDialogComposerSettings;

        [SerializeField]
        private bool mIsAppTrackingEnabled = false;

        [SerializeField]
        private AppTrackingSettings mAppTrackingSettings = null;

        /// <summary>
        /// Gets the default consent dialog.
        /// </summary>
        /// <value>The default consent dialog.</value>
        public ConsentDialog DefaultConsentDialog { get { return mDefaultConsentDialog; } }

        /// <summary>
        /// Is App Tracking submodule enabled?
        /// </summary>
        public bool IsAppTrackingEnabled { get { return mIsAppTrackingEnabled; } }

        /// <summary>
        /// Gets the App Tracking submodule settings.
        /// </summary>
        /// <value>The App Tracking settings.</value>
        public AppTrackingSettings AppTracking { get { return mAppTrackingSettings; } }
    }

    [Serializable]
    public class ConsentDialogComposerSettings
    {
        [SerializeField]
        private int mToggleSelectedIndex;
        [SerializeField]
        private int mButtonSelectedIndex;
        [SerializeField]
        private bool mEnableCopyPasteMode;
    }
}
