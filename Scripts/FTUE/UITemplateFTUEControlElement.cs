namespace TheOneStudio.UITemplate.UITemplate.FTUE
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class UITemplateFTUEControlElement : MonoBehaviour
    {
        private const string CanvasSortingLayerName = "UI";

        private Canvas           canvas;
        private GraphicRaycaster graphicRaycaster;
        public  Action           OnPositionChange;

        private void Awake()
        {
            this.canvas                 = this.gameObject.AddComponent<Canvas>();
            this.canvas.overrideSorting = true;
            this.canvas.sortingOrder    = 1;
            if (!this.DoesSortingLayerExist(CanvasSortingLayerName))
            {
                throw new Exception("You need to create new sorting layer with name: " + CanvasSortingLayerName + " in Edit -> Project Settings -> Tags and Layers");
            }

            this.canvas.sortingLayerName = CanvasSortingLayerName;
            this.graphicRaycaster        = this.gameObject.AddComponent<GraphicRaycaster>();
        }

        public bool DoesSortingLayerExist(string layerName)
        {
            foreach (SortingLayer layer in SortingLayer.layers)
            {
                if (layer.name.Equals(layerName))
                    return true;
            }

            return false;
        }

        private void OnRectTransformDimensionsChange() { this.OnPositionChange?.Invoke(); }

        private void OnDestroy()
        {
            Destroy(this.graphicRaycaster);
            Destroy(this.canvas);
        }
    }
}