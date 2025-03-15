namespace TheOneStudio.UITemplate.UITemplate.FTUE.RemoteConfig
{
    using System.Collections.Generic;
    using System.Linq;
    using BlueprintFlow.Signals;
    using Cysharp.Threading.Tasks;
    using GameFoundation.DI;
    using GameFoundation.Signals;
    using ServiceImplementation.FireBaseRemoteConfig;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using UnityEngine.Scripting;

    public class UITemplateFTUEBlueprintDataHandler : FTUEConfig, IInitializable
    {
        private readonly UITemplateFTUERemoteConfig        remoteConfig;
        private readonly UITemplateFTUEBlueprint           blueprint;
        private readonly SignalBus                         signalBus;
        private readonly UITemplateInventoryDataController uiTemplateInventoryDataController;
        private readonly UITemplateFTUEDataController      uiTemplateFtueDataController;

        [Preserve]
        public UITemplateFTUEBlueprintDataHandler(
            UITemplateFTUERemoteConfig remoteConfig,
            UITemplateFTUEBlueprint blueprint,
            SignalBus signalBus,
            UITemplateInventoryDataController uiTemplateInventoryDataController,
            UITemplateFTUEDataController uiTemplateFtueDataController
        )
        {
            this.remoteConfig                      = remoteConfig;
            this.blueprint                         = blueprint;
            this.signalBus                         = signalBus;
            this.uiTemplateInventoryDataController = uiTemplateInventoryDataController;
            this.uiTemplateFtueDataController      = uiTemplateFtueDataController;
        }

        public UITemplateFTUERecord GetDataById(string id) { return this.GetValueOrDefault(id); }

        public void Initialize()
        {
            this.signalBus.Subscribe<LoadBlueprintDataSucceedSignal>(this.LoadBlueprintData);
            this.signalBus.Subscribe<RemoteConfigFetchedSucceededSignal>(this.LoadRemoteData);
        }

        public void CompleteStep(string stepId)
        {
            if (this.uiTemplateFtueDataController.TryCompleteStep(stepId))
            {
                foreach (var previousStep in this.GetDataById(stepId).PreviousSteps) this.CompleteStep(previousStep);
            }
        }

        public void GiveReward(string stepId)
        {
            if (this.uiTemplateFtueDataController.TryGiveReward(stepId))
            {
                foreach (var pair in this.GetDataById(stepId).BonusOnStart)
                {
                    this.uiTemplateInventoryDataController.AddCurrency(pair.Value, pair.Key, "ftue");
                }  
            }
        }

        private void LoadBlueprintData()
        {
            foreach (var record in this.blueprint.Where(record => !this.Keys.Contains(record.Key)))
            {
                this[record.Key] = record.Value;
            }
        }

        private void LoadRemoteData(RemoteConfigFetchedSucceededSignal signal)
        {
#if !UNITY_EDITOR
            this.remoteConfig.LoadData(signal);
            foreach (var record in this.remoteConfig)
            {
                this[record.Key] = record.Value;
            }
#endif
        }
    }
}