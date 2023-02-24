#if UNITY_IOS
using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using EasyMobile.iOS.CoreFoundation;
using EasyMobile.iOS.Foundation;
using EasyMobile.iOS.UIKit;

namespace EasyMobile.Internal.iOS.UIKit
{
    using UIAdaptivePresentationControllerDelegate = UIPresentationController.UIAdaptivePresentationControllerDelegate;

    internal class UIAdaptivePresentationControllerDelegateForwarder : iOSDelegateForwarder<UIAdaptivePresentationControllerDelegate>
    {
        internal UIAdaptivePresentationControllerDelegateForwarder(IntPtr selfPtr)
            : base(selfPtr)
        {
        }

        internal static UIAdaptivePresentationControllerDelegateForwarder FromPointer(IntPtr pointer)
        {
            return InteropObjectFactory<UIAdaptivePresentationControllerDelegateForwarder>.FromPointer(
                pointer,
                ptr => new UIAdaptivePresentationControllerDelegateForwarder(ptr));
        }

        internal UIAdaptivePresentationControllerDelegateForwarder()
            : this(C.UIAdaptivePresentationControllerDelegate_new(
                    InternalPresentationControllerDidAttemptToDismissCallback,
                    InternalPresentationControllerShouldDismissCallback,
                    InternalPresentationControllerDidDismissCallback,
                    InternalPresentationControllerWillDismissCallback))
        {
            // We're using a pointer returned by a native constructor: call CFRelease to balance native ref count
            CFFunctions.CFRelease(this.ToPointer());
        }

        [MonoPInvokeCallback(typeof(C.InternalPresentationControllerDidAttemptToDismiss))]
        private static void InternalPresentationControllerDidAttemptToDismissCallback(IntPtr delegatePtr, IntPtr presentationControllerPtr)
        {
            var forwarder = FromPointer(delegatePtr);

            if (forwarder != null && forwarder.Listener != null)
            {
                // PresentationController.
                var controller = InteropObjectFactory<UIPresentationController>.FromPointer(presentationControllerPtr, ptr => new UIPresentationController(ptr));

                // Invoke consumer delegates.
                forwarder.InvokeOnListener(l => l.PresentationControllerDidAttemptToDismiss(controller));
            }
        }

        [MonoPInvokeCallback(typeof(C.InternalPresentationControllerShouldDismiss))]
        private static bool InternalPresentationControllerShouldDismissCallback(IntPtr delegatePtr, IntPtr presentationControllerPtr)
        {
            var forwarder = FromPointer(delegatePtr);

            if (forwarder != null && forwarder.Listener != null)
            {
                // PresentationController.
                var controller = InteropObjectFactory<UIPresentationController>.FromPointer(presentationControllerPtr, ptr => new UIPresentationController(ptr));

                // Invoke consumer delegates.
                return forwarder.InvokeOnListener(l => l.PresentationControllerShouldDismiss(controller));
            }

            return true;    // view should be dismissible by default
        }

        [MonoPInvokeCallback(typeof(C.InternalPresentationControllerDidDismiss))]
        private static void InternalPresentationControllerDidDismissCallback(IntPtr delegatePtr, IntPtr presentationControllerPtr)
        {
            var forwarder = FromPointer(delegatePtr);

            if (forwarder != null && forwarder.Listener != null)
            {
                // PresentationController.
                var controller = InteropObjectFactory<UIPresentationController>.FromPointer(presentationControllerPtr, ptr => new UIPresentationController(ptr));

                // Invoke consumer delegates.
                forwarder.InvokeOnListener(l => l.PresentationControllerDidDismiss(controller));
            }
        }

        [MonoPInvokeCallback(typeof(C.InternalPresentationControllerWillDismiss))]
        private static void InternalPresentationControllerWillDismissCallback(IntPtr delegatePtr, IntPtr presentationControllerPtr)
        {
            var forwarder = FromPointer(delegatePtr);

            if (forwarder != null && forwarder.Listener != null)
            {
                // PresentationController.
                var controller = InteropObjectFactory<UIPresentationController>.FromPointer(presentationControllerPtr, ptr => new UIPresentationController(ptr));

                // Invoke consumer delegates.
                forwarder.InvokeOnListener(l => l.PresentationControllerWillDismiss(controller));
            }
        }

        #region C wrapper

        private static class C
        {
            internal delegate void InternalPresentationControllerDidAttemptToDismiss(
            /* UIAdaptivePresentationControllerDelegateForwarder */ IntPtr delegatePtr,
            /* UIPresentationController */ IntPtr presentationController);

            internal delegate bool InternalPresentationControllerShouldDismiss(
            /* UIAdaptivePresentationControllerDelegateForwarder */ IntPtr delegatePtr,
            /* UIPresentationController */ IntPtr presentationController);

            internal delegate void InternalPresentationControllerDidDismiss(
            /* UIAdaptivePresentationControllerDelegateForwarder */ IntPtr delegatePtr,
            /* UIPresentationController */ IntPtr presentationController);

            internal delegate void InternalPresentationControllerWillDismiss(
            /* UIAdaptivePresentationControllerDelegateForwarder */ IntPtr delegatePtr,
            /* UIPresentationController */ IntPtr presentationController);

            [DllImport("__Internal")]
            internal static extern /* UIAdaptivePresentationControllerDelegateForwarder */ IntPtr
            UIAdaptivePresentationControllerDelegate_new(InternalPresentationControllerDidAttemptToDismiss didAttemptToDismissCallback,
                                                                  InternalPresentationControllerShouldDismiss shouldDismissCallback,
                                                                  InternalPresentationControllerDidDismiss didDismissCallback,
                                                                  InternalPresentationControllerWillDismiss willDismissCallback);
        }

        #endregion
    }
}
#endif