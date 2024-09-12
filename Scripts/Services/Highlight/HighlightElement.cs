namespace TheOneStudio.UITemplate.UITemplate.Scripts.Services.Highlight
{
    using System;
    using System.Linq;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.UI;

    public class HighlightElement : MonoBehaviour
    {
        private const string CanvasSortingLayerName = "UI";

        private Canvas                  canvas;
        private GraphicRaycaster        graphicRaycaster;
        public  Action                  OnPositionChange;
        private bool                    isActive;

        public void Setup()
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
            this.isActive                = true;
        }

        private void Update()
        {
            if (!this.isActive) return;
            this.OnPositionChange?.Invoke();
        }


        public void Despawn()
        {
            this.isActive      = false;
            this.OnPositionChange = null;
            Destroy(this.graphicRaycaster);
            Destroy(this.canvas);
        }

        private bool DoesSortingLayerExist(string layerName)
        {
            return SortingLayer.layers.Any(layer => layer.name.Equals(layerName));
        }
    }
}