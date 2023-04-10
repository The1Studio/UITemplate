namespace TheOneStudio.UITemplate.UITemplate.Models.Controllers
{
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;

    public class UITemplateBuildingController
    {
        private readonly UITemplateBuildingData      buildingData;
        private readonly UITemplateBuildingBlueprint uiTemplateBuildingBlueprint;

        public UITemplateBuildingController(UITemplateBuildingData buildingData, UITemplateBuildingBlueprint uiTemplateBuildingBlueprint)
        {
            this.buildingData                = buildingData;
            this.uiTemplateBuildingBlueprint = uiTemplateBuildingBlueprint;
        }

        public bool CheckBuildingStatus(string buildingId)
        {
            if (this.buildingData.Buildings.TryGetValue(buildingId, out var buildingData)) return buildingData.IsUnlocked;

            buildingData = new BuildingData()
            {
                Id            = buildingId,
                IsUnlocked    = false,
                UnlockPrice   = this.uiTemplateBuildingBlueprint[buildingId].UnlockPrice,
                EarnPerSecond = this.uiTemplateBuildingBlueprint[buildingId].EarnPerSecond
            };

            this.buildingData.Buildings.Add(buildingId, buildingData);

            return buildingData.IsUnlocked;
        }

        public BuildingData GetBuildingData(string buildingId) { return this.buildingData.Buildings[buildingId]; }

        public void UnlockBuilding(string buildingId) { this.buildingData.Buildings[buildingId].IsUnlocked = true; }
    }
}