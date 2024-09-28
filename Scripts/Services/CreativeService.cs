namespace TheOneStudio.UITemplate.UITemplate.Scripts.Services
{
    using GameFoundation.DI;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.Scripting;

    public class CreativeService : ITickable
    {
        private readonly EventSystem eventSystem;
        private          int         touchCount;
        private          float       touchTime = 0.25f;
        private          float       counter;

        public bool IsShowUI        { get; private set; } = true;
        public bool EnableTripleTap { get; set; }         = false;

        public readonly UnityEvent OnTripleTap = new();

        [Preserve]
        public CreativeService(EventSystem eventSystem) { this.eventSystem = eventSystem; }

        public void Tick()
        {
            if (!this.EnableTripleTap) return;

            for (var i = 0; i < Input.touchCount; ++i)
            {
                if (Input.GetTouch(i).phase != TouchPhase.Began) continue;

                if (Input.touchCount == 3)
                {
                    this.IsShowUI = !this.IsShowUI;
                    this.OnTripleTap?.Invoke();
                    this.eventSystem.enabled = true;
                }
            }

            if (this.counter > 0)
            {
                this.counter -= Time.unscaledDeltaTime;
            }
            else
            {
                this.touchCount = 0;
            }

            if (Input.GetMouseButtonDown(0))
            {
                this.counter = this.touchTime;
                this.touchCount++;
                if (this.touchCount == 3)
                {
                    this.IsShowUI = !this.IsShowUI;
                    this.OnTripleTap?.Invoke();
                    this.eventSystem.enabled = true;
                }
            }
        }

        public void DisableTripleTap() { this.EnableTripleTap = false; }
    }
}