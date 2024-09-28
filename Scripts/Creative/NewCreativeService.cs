namespace TheOneStudio.UITemplate.UITemplate.Creative
{
    using GameFoundation.DI;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using R3;
    using UnityEngine;
    using UnityEngine.Scripting;
    using Object = UnityEngine.Object;

    [Preserve]
    public class NewCreativeService : IInitializable
    {
        private bool  EnableTripleTap { get; set; } = true;
        private float lastTimeCheck;
        private int   tapCheckCount;

        public void Initialize() { Observable.EveryUpdate().Where(_ => Input.GetMouseButtonDown(0)).Subscribe(this.OnMouseDown); }

        private void OnMouseDown(Unit _)
        {
            if (Time.unscaledTime - this.lastTimeCheck < 0.4f)
            {
                this.tapCheckCount++;
                if (this.tapCheckCount == 3)
                {
                    this.OnTripleClick();
                    this.tapCheckCount = 0;
                }
            }
            else
            {
                this.tapCheckCount = 1;
            }

            this.lastTimeCheck = Time.unscaledTime;
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