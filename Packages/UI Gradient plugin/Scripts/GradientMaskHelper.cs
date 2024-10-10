
using UnityEngine;
using UnityEngine.UI;

namespace PolyAndCode.UI.effect
{
    /// <summary>
    /// Test case 1:
    ///    1. gradient component sets its isMasked property in onEnable.
    ///    2. IsMasked property won't set if mask's state changes since gradient component's onEnable won't call
    ///    2. This script changes isMasked property for all masked children when Mask's state changes
    /// Test Case 2 (happeining in Unity 2017) : 
    ///    1.Disable mask component -> 
    ///    2.Change gradient type  -> 
    ///    3.Enable mask : gradient will revert to the previous setting.
    ///    4.probably becuase material for rendering is still refrencing the material used last time when masked
    ///    5.So this script basically updates the material for rendering for all gradient components when mask is reenabled
  
    /// To handle masking for UI gradients.
    /// updates the gradient at mask component's state changes
    /// </summary>
    [RequireComponent(typeof(Mask))]
    [ExecuteInEditMode]
    public class GradientMaskHelper : MonoBehaviour
    {
        private Mask mask;
        private bool maskState;
        private UIGradient[] uIGradients;

        /// <summary>
        /// Get the mask and mask state in awake
        /// </summary>
        private void Awake()
        {
            mask = GetComponent<Mask>();
            if (mask != null)
            {
                maskState = mask.enabled;
            }
        }

        /// <summary>
        /// if disabled/enabled the gradients children count might have changed    
        /// </summary>
        private void OnEnable()
        {
            uIGradients = GetComponentsInChildren<UIGradient>();
        }

        void Update()
        {
            //If mask enable state changes, upate all children gradient
            if (mask != null && maskState != mask.enabled)
            {
                foreach (var item in uIGradients)
                {
                    //This inturn updates material; checked isMasked property
                    item.isMasked = mask.enabled;
                }
                maskState = mask.enabled;
            }
        }

        //if destroyed set all gradients to unmasked
        private void OnDestroy()
        {
            foreach (var item in uIGradients)
            {
                item.isMasked = false;
            }
        }
    }
}
