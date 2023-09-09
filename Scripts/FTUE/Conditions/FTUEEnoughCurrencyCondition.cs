namespace TheOneStudio.UITemplate.UITemplate.FTUE.Conditions
{
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;

    public class FTUEEnoughCurrencyContidionModel
    {
        public string Condition;
        public string Id;
        public int    Value;
    }

    public class FTUEEnoughCurrencyCondition : FtueCondition<FTUEEnoughCurrencyContidionModel>
    {
        #region inject

        private readonly UITemplateInventoryDataController uiTemplateInventoryDataController;
        private readonly UITemplateFTUEHelper              uiTemplateFtueHelper;

        #endregion

        public FTUEEnoughCurrencyCondition(UITemplateInventoryDataController uiTemplateInventoryDataController, UITemplateFTUEHelper uiTemplateFtueHelper)
        {
            this.uiTemplateInventoryDataController = uiTemplateInventoryDataController;
            this.uiTemplateFtueHelper              = uiTemplateFtueHelper;
        }

        public override string Id => "enough_currency";
        protected override bool IsPassedCondition(FTUEEnoughCurrencyContidionModel data)
        {
            return this.uiTemplateFtueHelper.CompareIntWithCondition(this.uiTemplateInventoryDataController.GetCurrencyValue(data.Id), data.Value, data.Condition);
        }
    }
}