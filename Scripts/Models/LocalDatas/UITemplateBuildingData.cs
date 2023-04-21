namespace TheOneStudio.UITemplate.UITemplate.Models.LocalDatas
{
    using System;
    using System.Collections.Generic;
    using GameFoundation.Scripts.Interfaces;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;

    public class UITemplateBuildingData : ILocalData,IUITemplateLocalData
    {
        public Dictionary<string, BuildingData> Buildings = new();
        public bool                             IsFirstTimeToBuilding { get; set; }
        public float                            CurrentEarnCurrency   { get; set; } = 0;

        public void Init()         { }
        public Type ControllerType => typeof(UITemplateBuildingController);
    }

    public class BuildingData : UITemplateBuildingRecord
    {
        public bool IsUnlocked;
        public int  RemainPrice;
    }
}