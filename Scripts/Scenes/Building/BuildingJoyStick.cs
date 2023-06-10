namespace TheOneStudio.HyperCasual.DrawCarBase.Scripts.Runtime.Scenes.Building
{
    using System.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    using Zenject;

    public class BuildingJoyStick : MonoBehaviour
    {
        private GraphicRaycaster           GraphicRaycaster { get; set; }
        
        public  EventTrigger               JoystickEventTrigger;
        public  TaskCompletionSource<bool> IsInjected = new(false);
           
        #region zenject

        private BuildingCarController        buildingCarController;
        private SignalBus                    signalBus;
        private UITemplateBuildingController uiTemplateBuildingController;
        private IScreenManager               screenManager;

        
        [Inject]
        public void Init(BuildingCarController buildingCarController,SignalBus signalBus, UITemplateBuildingController uiTemplateBuildingController,
            IScreenManager screenManager)
        {
            this.buildingCarController        = buildingCarController;
            this.signalBus                    = signalBus;
            this.uiTemplateBuildingController = uiTemplateBuildingController;
            this.screenManager                = screenManager;
            
            this.IsInjected.TrySetResult(true);
            this.AddEventTrigger();
            this.GraphicRaycaster = this.GetComponent<GraphicRaycaster>();
        }

        #endregion

        private void AddEventTrigger()
        {
            var pointerDownEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerDown
            };
            pointerDownEntry.callback.AddListener(_ => this.OnPointerDown());
            this.JoystickEventTrigger.triggers.Add(pointerDownEntry);
        }

        private void OnPointerDown()
        {
            this.signalBus.Fire(new BuildingOnMouseDownSignal());
            this.uiTemplateBuildingController.IsFirstTimeOpenBuilding = false;
        }
        
        private async void FixedUpdate()
        {
            await this.IsInjected.Task;
            this.GraphicRaycaster.enabled = this.screenManager.CurrentActiveScreen is UITemplateBuildingScreenPresenter;
            // var h = UltimateJoystick.GetHorizontalAxis("move");
            // var v = UltimateJoystick.GetVerticalAxis("move");
            // this.buildingCarController.Moving(v, h);
        }
    }
}