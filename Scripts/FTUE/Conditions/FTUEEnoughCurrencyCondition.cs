namespace TheOneStudio.UITemplate.UITemplate.FTUE.Conditions
{
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using UnityEngine.Scripting;

    public class FTUEEnoughCurrencyConditionModel
    {
        public string Condition { get; }
        public string Id        { get; }
        public int    Value     { get; }

        [Preserve]
        public FTUEEnoughCurrencyConditionModel(string condition, string id, int value)
        {
            this.Condition = condition;
            this.Id        = id;
            this.Value     = value;
        }
    }

    public class FTUEEnoughCurrencyCondition : FtueCondition<FTUEEnoughCurrencyConditionModel>
    {
        #region inject

        private readonly UITemplateInventoryDataController uiTemplateInventoryDataController;
        private readonly UITemplateFTUEHelper              uiTemplateFtueHelper;

        #endregion

        [Preserve]
        public FTUEEnoughCurrencyCondition(UITemplateInventoryDataController uiTemplateInventoryDataController, UITemplateFTUEHelper uiTemplateFtueHelper)
        {
            this.uiTemplateInventoryDataController = uiTemplateInventoryDataController;
            this.uiTemplateFtueHelper              = uiTemplateFtueHelper;
        }

        public override string Id => "enough_currency";

        protected override string GetTooltipText(FTUEEnoughCurrencyConditionModel data)
        {
            return $"Not enough currency";
        }

        protected override bool IsPassedCondition(FTUEEnoughCurrencyConditionModel data)
        {
            return this.uiTemplateFtueHelper.CompareIntWithCondition(this.uiTemplateInventoryDataController.GetCurrencyValue(data.Id), data.Value, data.Condition);
        }
    }
}