namespace TheOneStudio.HyperCasual.DrawCarBase.Scripts.Runtime.Scenes.Building
{
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using UnityEngine;
    using Zenject;

    public class BuildingEarnCurrencySystem : ITickable
    {
        private readonly UITemplateBuildingController buildingController;
        private          float                        TimeToEarnCurrency      = 0;
        private          float                        TimeDelayToEarnCurrency = 1;

        public BuildingEarnCurrencySystem(UITemplateBuildingController buildingController) { this.buildingController = buildingController; }

        public void Tick()
        {
            this.TimeToEarnCurrency += Time.deltaTime;

            if (this.TimeToEarnCurrency >= this.TimeDelayToEarnCurrency)
            {
                this.buildingController.EarnCurrency();
                this.TimeToEarnCurrency = 0;
            }
        }
    }
}