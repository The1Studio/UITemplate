namespace TheOneStudio.UITemplate.UITemplate.Utils
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    [DisallowMultipleComponent, RequireComponent(typeof(RectTransform)), RequireComponent(typeof(BoxCollider))]
    public class AdjustSizeBoxCollider3DWithRectTransform : UIBehaviour
    {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private BoxCollider   boxCollider;

        protected override void Awake()
        {
            base.Awake();
            this.ValidateRef();
            this.AdjustSize();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            this.ValidateRef();
        }
#endif

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            this.AdjustSize();
        }

        private void ValidateRef()
        {
            this.boxCollider   ??= this.GetComponent<BoxCollider>();
            this.rectTransform ??= this.GetComponent<RectTransform>();
        }

        private void AdjustSize()
        {
            var sizeRect  = new Vector2(this.rectTransform.rect.width, this.rectTransform.rect.height);
            var pivotRect = this.rectTransform.pivot;
            this.boxCollider.size   = new Vector3(sizeRect.x,                        sizeRect.y, 1);
            this.boxCollider.center = new Vector3(sizeRect.x * (0.5f - pivotRect.x), sizeRect.y * (0.5f - pivotRect.y));
        }
    }
}