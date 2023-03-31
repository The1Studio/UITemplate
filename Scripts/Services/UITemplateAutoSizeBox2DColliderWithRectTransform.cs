namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using UnityEngine;

    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(RectTransform))]
    public class UITemplateAutoSizeBox2DColliderWithRectTransform : MonoBehaviour
    {
        RectTransform rect => this.GetComponent<RectTransform>();
        BoxCollider2D box  => this.GetComponent<BoxCollider2D>();

        [Tooltip("Is auto size on update?")] public bool autoSizeOnUpdate = false;

        private void Start() { this.box.size = new Vector2(this.rect.rect.width, this.rect.rect.height); }

        private void Update()
        {
            if (this.autoSizeOnUpdate)
            {
                this.box.size = new Vector2(this.rect.rect.width, this.rect.rect.height);
            }
        }
    }
}