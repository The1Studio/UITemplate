#if UNITY_IOS
using System;
using System.Runtime.InteropServices;
using AOT;
using EasyMobile.Internal;
using EasyMobile.Internal.iOS;
using EasyMobile.Internal.iOS.UIKit;

namespace EasyMobile.iOS.UIKit
{
    internal class UIPresentationController : iOSObjectProxy
    {
        protected const string FrameworkName = "UIKit";

        #region UIAdaptivePresentationControllerDelegate

        /// <summary>
        /// A set of methods that, in conjunction with a presentation controller,
        /// determine how to respond to trait changes in your app.
        /// </summary>
        public interface UIAdaptivePresentationControllerDelegate
        {
            /// <summary>
            /// Notifies the delegate that a user-initiated attempt to dismiss a view was prevented.
            /// </summary>
            /// <param name="presentationController"></param>
            void PresentationControllerDidAttemptToDismiss(UIPresentationController presentationController);

            /// <summary>
            /// Asks the delegate for permission to dismiss the presentation.
            /// </summary>
            /// <param name="presentationController"></param>
            /// <returns></returns>
            bool PresentationControllerShouldDismiss(UIPresentationController presentationController);

            /// <summary>
            /// Notifies the delegate after a presentation is dismissed.
            /// </summary>
            /// <param name="presentationController"></param>
            void PresentationControllerDidDismiss(UIPresentationController presentationController);

            /// <summary>
            /// Notifies the delegate before a presentation is dismissed.
            /// </summary>
            /// <param name="presentationController"></param>
            void PresentationControllerWillDismiss(UIPresentationController presentationController);
        }

        #endregion

        private UIAdaptivePresentationControllerDelegate mDelegate;
        private UIAdaptivePresentationControllerDelegateForwarder mDelegateForwarder;

        internal UIPresentationController(IntPtr selfPointer)
            : base(selfPointer)
        {
        }

        /// <summary>
        /// The delegate object for managing adaptive presentations.
        /// </summary>
        /// <value>The delegate.</value>
        public UIAdaptivePresentationControllerDelegate Delegate
        {
            get
            {
                return mDelegate;
            }
            set
            {
                mDelegate = value;

                if (mDelegate == null)
                {
                    // Nil out the native delegate.
                    mDelegateForwarder = null;
                    C.UIPresentationController_setDelegate(SelfPtr(), IntPtr.Zero);
                }
                else
                {
                    // Create a delegate forwarder if needed.
                    if (mDelegateForwarder == null)
                    {
                        mDelegateForwarder = InteropObjectFactory<UIAdaptivePresentationControllerDelegateForwarder>.Create(
                            () => new UIAdaptivePresentationControllerDelegateForwarder(),
                            fwd => fwd.ToPointer());

                        // Assign on native side.
                        C.UIPresentationController_setDelegate(SelfPtr(), mDelegateForwarder.ToPointer());
                    }

                    // Set delegate.
                    mDelegateForwarder.Listener = mDelegate;
                }
            }
        }

        #region C wrapper

        private static class C
        {
            [DllImport("__Internal")]
            internal static extern /* InteropUIAdaptivePresentationControllerDelegate */ IntPtr UIPresentationController_delegate(HandleRef selfPtr);

            [DllImport("__Internal")]
            internal static extern void UIPresentationController_setDelegate(HandleRef selfPtr, /* InteropUIAdaptivePresentationControllerDelegate */IntPtr delegatePtr);
        }

        #endregion
    }
}
#endif
