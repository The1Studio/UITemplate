#if THEONE_LOCALIZATION && UNITY_LOCALIZATION
namespace TheOneStudio.UITemplate.UITemplate
{
    using UnityEngine;
    using UnityEngine.Localization.Components;

    [RequireComponent(typeof(LocalizeStringEvent))]
    public class AutoStringLocalization : MonoBehaviour
    {
    }
}
#endif