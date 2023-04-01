namespace TheOneStudio.UITemplate.UITemplate.Scripts.Services
{
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using UnityEngine;
    using Zenject;

    public class CreativeService : ITickable
    {
        private readonly IScreenManager screenManager;

        private int   touchCount;
        private float touchTime = 0.25f;
        private float counter;

        public bool isShowUI { get; private set; } = true;

        public CreativeService(IScreenManager screenManager)
        {
            this.screenManager = screenManager;
        }

        public void Tick()
        {
            for (var i = 0; i < Input.touchCount; ++i)
            {
                if (Input.GetTouch(i).phase != TouchPhase.Began) continue;

                if (Input.touchCount == 3)
                {
                    this.screenManager.RootUICanvas.gameObject.SetActive(this.isShowUI = !this.isShowUI);
                }
            }

            if (this.counter > 0)
            {
                this.counter -= Time.deltaTime;
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
                    this.screenManager.RootUICanvas.gameObject.SetActive(this.isShowUI = !this.isShowUI);
                }
            }
        }
    }
}