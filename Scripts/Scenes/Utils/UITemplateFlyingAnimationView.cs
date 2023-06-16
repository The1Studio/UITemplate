namespace TheOneStudio.UITemplate.UITemplate.Scenes.Utils
{
    using UnityEngine;

    public class UITemplateFlyingAnimationView : MonoBehaviour
    {
        [SerializeField] private Transform targetFlyingAnimation;

        public Transform TargetFlyingAnimation => this.targetFlyingAnimation ??= this.GetComponent<Transform>();
    }
}