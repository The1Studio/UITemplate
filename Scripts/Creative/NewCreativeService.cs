namespace TheOneStudio.UITemplate.UITemplate.Creative
{
    using System;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using UniRx;
    using UnityEngine;
    using Zenject;
    using Object = UnityEngine.Object;

    public class NewCreativeService : IInitializable
    {
        private bool EnableTripleTap { get; set; } = true;

        public void Initialize()
        {
            var clickStream = Observable.EveryUpdate().Where(_ => Input.GetMouseButtonDown(0));

            clickStream.Buffer(clickStream.Throttle(TimeSpan.FromMilliseconds(250)))
                       .Where(xs => xs.Count >= 3)
                       .Subscribe(xs => this.OnTripleClick());
        }

        private void OnTripleClick()
        {
            if (!this.EnableTripleTap) return;

            var rootUICanvas = Object.FindObjectOfType<RootUICanvas>();
            if (rootUICanvas == null) return;
            var canvas = rootUICanvas.GetComponentInChildren<Canvas>();
            if (canvas == null) return;
            canvas.enabled = !canvas.enabled;
        }

        public void DisableTripleTapAction() { this.EnableTripleTap = false; }
    }
}