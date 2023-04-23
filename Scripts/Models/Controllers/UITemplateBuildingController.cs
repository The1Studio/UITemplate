namespace TheOneStudio.UITemplate.UITemplate.Models.Controllers
{
    using GameFoundation.Scripts.Utilities.LogService;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;
    using UnityEngine;

    public class UITemplateBuildingController:IUITemplateControllerData
    {
        private readonly UITemplateBuildingData            buildingData;
        private readonly UITemplateInventoryDataController uiTemplateInventoryDataController;
        private readonly ILogService                       logger;
        private readonly UITemplateBuildingBlueprint       uiTemplateBuildingBlueprint;

        public UITemplateBuildingController(UITemplateBuildingData buildingData, UITemplateInventoryDataController uiTemplateInventoryDataController, ILogService logger,
            UITemplateBuildingBlueprint uiTemplateBuildingBlueprint)
        {
            this.buildingData                      = buildingData;
            this.uiTemplateInventoryDataController = uiTemplateInventoryDataController;
            this.logger                            = logger;
            this.uiTemplateBuildingBlueprint       = uiTemplateBuildingBlueprint;
        }

        public bool CheckBuildingStatus(string buildingId)
        {
            if (this.buildingData.Buildings.TryGetValue(buildingId, out var buildingData)) return buildingData.IsUnlocked;

            buildingData = new BuildingData()
            {
                Id            = buildingId,
                IsUnlocked    = false,
                UnlockPrice   = this.uiTemplateBuildingBlueprint[buildingId].UnlockPrice,
                EarnPerSecond = this.uiTemplateBuildingBlueprint[buildingId].EarnPerSecond,
                RemainPrice   = this.uiTemplateBuildingBlueprint[buildingId].UnlockPrice
            };

            this.buildingData.Buildings.Add(buildingId, buildingData);

            return buildingData.IsUnlocked;
        }

        public BuildingData GetBuildingData(string buildingId) { return this.buildingData.Buildings[buildingId]; }

        public void UnlockBuilding(string buildingId) { this.buildingData.Buildings[buildingId].IsUnlocked = true; }

        public void EarnCurrency()
        {
            foreach (var buildingDataBuilding in this.buildingData.Buildings)
            {
                if (!buildingDataBuilding.Value.IsUnlocked) continue;
                this.buildingData.CurrentEarnCurrency += buildingDataBuilding.Value.EarnPerSecond["coin"];
            }
#if UNITY_EDITOR

            this.logger.LogWithColor($"Earn Currency{this.buildingData.CurrentEarnCurrency}", Color.cyan);
#endif

            if (((int)this.buildingData.CurrentEarnCurrency % 10 != 0) || ((int)this.buildingData.CurrentEarnCurrency == 0)) return;
            this.uiTemplateInventoryDataController.AddCurrency((int)this.buildingData.CurrentEarnCurrency);
            this.buildingData.CurrentEarnCurrency -= (int)this.buildingData.CurrentEarnCurrency;
        }
    }
}