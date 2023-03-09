namespace TheOneStudio.UITemplate.UITemplate.Scripts.Services
{
    using System;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using UnityEngine;
    using Zenject;

    public class CreativeService : IInitializable, ITickable
    {
        private readonly IScreenManager screenManager;

        private int      mouseDownCounter;
        private DateTime lastMouseDownTime;

        public CreativeService(IScreenManager screenManager) { this.screenManager = screenManager; }

        public void Initialize()
        {
            this.lastMouseDownTime = DateTime.Now;
        }
        
        public void Tick()
        {
            //TODO change this to more significant behaviour
            //Get three continuous mouse down in time
            if (Input.GetMouseButtonDown(0))
            {
                if ((DateTime.Now - this.lastMouseDownTime).TotalSeconds > 0.5f)
                {
                    this.mouseDownCounter = 1;
                }
                else
                {
                    this.mouseDownCounter++;
                    if (this.mouseDownCounter == 3)
                    {
                        this.screenManager.RootUICanvas.gameObject.SetActive(!this.screenManager.RootUICanvas.gameObject.activeSelf);
                        this.mouseDownCounter = 0;
                    }
                }

                this.lastMouseDownTime = DateTime.Now;
            }
        }
    }
}