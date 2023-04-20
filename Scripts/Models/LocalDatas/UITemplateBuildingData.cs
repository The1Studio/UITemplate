namespace TheOneStudio.UITemplate.UITemplate.Models.LocalDatas
{
    using System.Collections.Generic;
    using GameFoundation.Scripts.Interfaces;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;

    public class UITemplateBuildingData : ILocalData
    {
        public Dictionary<string, BuildingData> Buildings = new();
        public float                            CurrentEarnCurrency { get; set; } = 0;

        public void Init() { }
    }

    public class BuildingData : UITemplateBuildingRecord
    {
        public bool IsUnlocked;
        public int  RemainPrice;
    }
}