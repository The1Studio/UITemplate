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
        private CancellationTokenSource cancellationTokenSource;

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
            if(this.cancellationTokenSource != null)
            {
                this.cancellationTokenSource.Cancel();
                this.cancellationTokenSource.Dispose();
            }
            this.cancellationTokenSource = new CancellationTokenSource();
            this.FollowObject().Forget();
        }

        private async UniTask FollowObject()
        {
            while (true)
            {
                try
                {
                    this.OnPositionChange?.Invoke();
                    await UniTask.Yield(PlayerLoopTiming.LastUpdate, this.cancellationTokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }
        public void Despawn()
        {
            this.cancellationTokenSource.Cancel();
            this.cancellationTokenSource.Dispose();
            Destroy(this.graphicRaycaster);
            Destroy(this.canvas);
        }

        private bool DoesSortingLayerExist(string layerName)
        {
            return SortingLayer.layers.Any(layer => layer.name.Equals(layerName));
        }

    }
}