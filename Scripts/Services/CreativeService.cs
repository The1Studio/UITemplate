namespace TheOneStudio.UITemplate.UITemplate.Scripts.Services
{
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using UnityEngine;
    using Zenject;

    public class CreativeService : ITickable
    {
        private readonly IScreenManager screenManager;

        private int mouseDownCounter;

        public CreativeService(IScreenManager screenManager) { this.screenManager = screenManager; }

        public void Tick()
        {
            //TODO change this to more significant behaviour
            //Get three continuous mouse down in time
            if (Input.touchCount == 3)
            {
                this.screenManager.RootUICanvas.gameObject.SetActive(!this.screenManager.RootUICanvas.gameObject.activeSelf);
            }
        }
    }
}