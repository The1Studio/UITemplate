#if UNITY_IOS
using System;
using System.Runtime.InteropServices;
using AOT;
using EasyMobile.Internal;
using EasyMobile.Internal.iOS;

namespace EasyMobile.iOS.UIKit
{
    internal class UIViewController : iOSObjectProxy
    {
        protected const string FrameworkName = "UIKit";

        internal UIViewController(IntPtr selfPointer)
            : base(selfPointer)
        {
        }

        /// <summary>
        /// The presentation controller that’s managing the current view controller.
        /// </summary>
        public UIPresentationController PresentationController
        {
            get
            {
                var pointer = C.UIViewController_presentationController(SelfPtr());
                var controller = InteropObjectFactory<UIPresentationController>.FromPointer(pointer, ptr => new UIPresentationController(ptr));
                CoreFoundation.CFFunctions.CFRelease(pointer);  // release pointer returned by the native method
                return controller;
            }
            
        }

        /// <summary>
        /// Gets Unity's GL view controller.
        /// </summary>
        /// <returns>The get GL view controller.</returns>
        public static UIViewController UnityGetGLViewController()
        {
            var ptr = C.UIViewController_UnityGetGLViewController();
            var vc = new UIViewController(ptr);
            CoreFoundation.CFFunctions.CFRelease(ptr); // pointer returned by a native constructor: must call CFRelease
            return vc;
        }

        /// <summary>
        /// Presents a view controller modally.
        /// </summary>
        /// <param name="viewController">View controller.</param>
        /// <param name="animated">If set to <c>true</c> animated.</param>
        /// <param name="completionHandler">Completion handler.</param>
        public void PresentViewController(UIViewController viewController, bool animated, Action completionHandler)
        {
            C.UIViewController_presentViewController(
                SelfPtr(),
                viewController.ToPointer(),
                animated,
                completionHandler == null ? (C.PresentViewControllerCallback)null : InternalPresentViewControllerCallback,
                completionHandler == null ? IntPtr.Zero : PInvokeCallbackUtil.ToIntPtr(completionHandler)
            );
        }

        /// <summary>
        /// Dismisses the view controller that was presented modally by the view controller.
        /// </summary>
        /// <param name="animated">If set to <c>true</c> animated.</param>
        /// <param name="completionHandler">Completion handler.</param>
        public void DismissViewController(bool animated, Action completionHandler)
        {
            C.UIViewController_dismissViewController(
                SelfPtr(),
                animated,
                completionHandler == null ? (C.DismissViewControllerCallback)null : InternalDismissViewControllerCallback,
                completionHandler == null ? IntPtr.Zero : PInvokeCallbackUtil.ToIntPtr(completionHandler)
            );
        }

        [MonoPInvokeCallback(typeof(C.PresentViewControllerCallback))]
        private static void InternalPresentViewControllerCallback(IntPtr secondaryCallback)
        {
            if (secondaryCallback != IntPtr.Zero)
            {
                PInvokeCallbackUtil.PerformInternalCallback(
                    "UIViewController#InternalPresentViewControllerCallback",
                    PInvokeCallbackUtil.Type.Temporary,
                    secondaryCallback);
            }
        }

        [MonoPInvokeCallback(typeof(C.DismissViewControllerCallback))]
        private static void InternalDismissViewControllerCallback(IntPtr secondaryCallback)
        {
            if (secondaryCallback != IntPtr.Zero)
            {
                PInvokeCallbackUtil.PerformInternalCallback(
                    "UIViewController#InternalDismissViewControllerCallback",
                    PInvokeCallbackUtil.Type.Temporary,
                    secondaryCallback);
            }
        }

        #region C wrapper

        private static class C
        {
            internal delegate void PresentViewControllerCallback(IntPtr secondaryCallback);

            internal delegate void DismissViewControllerCallback(IntPtr secondaryCallback);

            [DllImport("__Internal")]
            internal static extern /* InteropUIViewController */ IntPtr UIViewController_UnityGetGLViewController();

            [DllImport("__Internal")]
            internal static extern void UIViewController_presentViewController(
            /* InteropUIViewController */ HandleRef selfPtr,
            /* InteropUIViewController */IntPtr viewController,
                                          bool animated,
                                          PresentViewControllerCallback callback, 
                                          IntPtr secondaryCallback);

            [DllImport("__Internal")]
            internal static extern void UIViewController_dismissViewController(
                /* InteropUIViewController */ HandleRef selfPtr,
                                              bool animated,
                                              DismissViewControllerCallback callback, 
                                              IntPtr secondaryCallback);

            [DllImport("__Internal")]
            internal static extern /* UIPresentationController */ IntPtr UIViewController_presentationController(
                /* InteropUIViewController */ HandleRef selfPtr);
        }

        #endregion
    }
}
#endif
