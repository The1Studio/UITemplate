#if UNITY_IOS
using UnityEngine;
using System;
using EasyMobile.Internal.iOS;
using EasyMobile.iOS.Foundation;
using EasyMobile.iOS.UIKit;
using UIAdaptivePresentationControllerDelegate = EasyMobile.iOS.UIKit.UIPresentationController.UIAdaptivePresentationControllerDelegate;

namespace EasyMobile.Internal.NativeAPIs.Media
{
    internal class InternalUIAdaptivePresentationControllerDelegate : UIAdaptivePresentationControllerDelegate
    {
        // Using Action & Func properties so we can have return type.
        public Action<UIPresentationController> DidAttemptToDismiss { get; set; }
        public Func<UIPresentationController, bool> ShouldDismiss { get; set; }
        public Action<UIPresentationController> DidDismiss { get; set; }
        public Action<UIPresentationController> WillDismiss { get; set; }

        internal InternalUIAdaptivePresentationControllerDelegate()
        {
        }

        #region InternalUIAdaptivePresentationControllerDelegate implementation

        public void PresentationControllerDidAttemptToDismiss(UIPresentationController presentationController)
        {
            if (DidAttemptToDismiss != null)
                DidAttemptToDismiss(presentationController);
        }

        public bool PresentationControllerShouldDismiss(UIPresentationController presentationController)
        {
            if (ShouldDismiss != null)
                return ShouldDismiss(presentationController);
            else
                return true;    // view should be dismissible by default
        }

        public void PresentationControllerDidDismiss(UIPresentationController presentationController)
        {
            if (DidDismiss != null)
                DidDismiss(presentationController);
        }

        public void PresentationControllerWillDismiss(UIPresentationController presentationController)
        {
            if (WillDismiss != null)
                WillDismiss(presentationController);
        }

        #endregion
    }
}
#endif // UNITY_IOS
