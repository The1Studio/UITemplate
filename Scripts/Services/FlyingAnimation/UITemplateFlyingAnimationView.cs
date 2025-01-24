namespace TheOneStudio.UITemplate.UITemplate.Scenes.Utils
{
    using UnityEngine;

    public abstract class UITemplateFlyingAnimationView : MonoBehaviour
    {
        [SerializeField] private Transform targetFlyingAnimation;

        public          Transform TargetFlyingAnimation => this.targetFlyingAnimation ??= this.GetComponent<Transform>();
        public abstract string    CurrencyKey           { get; }
    }
}