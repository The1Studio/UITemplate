namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using TheOneStudio.UITemplate.UITemplate.Blueprints.Gacha;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Gacha.LuckyWheel;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using Zenject;

    public class UITemplateLuckySpinServices : IInitializable
    {
        private readonly ScreenManager                     screenManager;
        private readonly EventSystem                       eventSystem;
        private readonly UITemplateInventoryDataController uiTemplateInventoryDataController;
        private readonly UITemplateLuckySpinBlueprint      spinBlueprint;
        private readonly SignalBus                         signalBus;
        private readonly UITemplateLuckySpinController     uiTemplateLuckySpinController;

        public UITemplateLuckySpinServices(ScreenManager screenManager, EventSystem eventSystem, UITemplateInventoryDataController uiTemplateInventoryDataController,
            UITemplateLuckySpinBlueprint spinBlueprint,
            SignalBus signalBus, UITemplateLuckySpinController uiTemplateLuckySpinController)
        {
            this.screenManager                     = screenManager;
            this.eventSystem                       = eventSystem;
            this.uiTemplateInventoryDataController = uiTemplateInventoryDataController;
            this.spinBlueprint                     = spinBlueprint;
            this.signalBus                         = signalBus;
            this.uiTemplateLuckySpinController     = uiTemplateLuckySpinController;
        }

        public void Initialize() { }

        public void OpenLuckySpin()
        {
            var isFreeSpin         = true;
            var isUseFreeSpinToday = this.uiTemplateLuckySpinController.IsUsedFreeSpinToDay();

            if (isUseFreeSpinToday)
            {
                isFreeSpin = false;
            }

            var forceSpin = -1;

            if (isFreeSpin)
            {
                forceSpin = Random.Range(0, this.spinBlueprint.Values.Count);
            }

            _ = this.screenManager.OpenScreen<UITemplateLuckyWheelSpinScreenPresenter, UITemplateLuckyWheelSpinModel>(new UITemplateLuckyWheelSpinModel()
            {
                AutoClose      = false,
                IsFreeSpin     = isFreeSpin,
                ForceSpinIndex = forceSpin,
                SpinRecords    = this.spinBlueprint.Values.ToList(),
                OnSpinComplete = this.OnSpinComplete
            });
        }

        private async void OnSpinComplete(int index, RectTransform startRect)
        {
            this.eventSystem.gameObject.SetActive(false);
          
            var item = this.spinBlueprint.ElementAt(index).Value;

            foreach (var r in item.Rewards)
            {
                this.uiTemplateInventoryDataController.AddGenericReward(r.Key, r.Value, startRect);
            }

            this.uiTemplateLuckySpinController.SaveTimeSpinToDay();
            await UniTask.Delay(1000);
            this.eventSystem.gameObject.SetActive(true);
        }
    }
}