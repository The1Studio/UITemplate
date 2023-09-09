namespace TheOneStudio.UITemplate.UITemplate.FTUE
{
    using UnityEngine;
    using UnityEngine.UI;

    public class UITemplateFTUEControlElement : MonoBehaviour
    {
        private Canvas           canvas;
        private GraphicRaycaster graphicRaycaster;

        private void Awake()
        {
            this.canvas                  = this.gameObject.AddComponent<Canvas>();
            this.canvas.overrideSorting  = true;
            this.canvas.sortingOrder     = 1;
            this.canvas.sortingLayerName = "UI";
            this.graphicRaycaster        = this.gameObject.AddComponent<GraphicRaycaster>();
        }

        private void OnDestroy()
        {
            Destroy(this.graphicRaycaster);
            Destroy(this.canvas);
        }
    }
}