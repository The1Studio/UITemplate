namespace TheOneStudio.UITemplate.UITemplate.Scripts.Services
{
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using UnityEngine;
    using Zenject;

    public class CreativeService : ITickable
    {
        private readonly IScreenManager screenManager;

        public CreativeService(IScreenManager screenManager) { this.screenManager = screenManager; }

        public void Tick()
        {
            for (var i = 0; i < Input.touchCount; ++i)
            {
                if (Input.GetTouch(i).phase != TouchPhase.Began) continue;

                if (Input.touchCount == 3)
                {
                    this.screenManager.RootUICanvas.gameObject.SetActive(!this.screenManager.RootUICanvas.gameObject.activeInHierarchy);
                }
            }
        }
    }
}