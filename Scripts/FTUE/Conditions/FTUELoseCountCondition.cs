namespace TheOneStudio.UITemplate.UITemplate.FTUE.Conditions
{
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;

    public class FTUELoseCountModel
    {
        public int Count;
    }

    public class FTUELoseCountCondition : FtueCondition<FTUELoseCountModel>
    {
        #region inject

        private readonly UITemplateLevelDataController uiTemplateLevelDataController;

        #endregion

        public override string   Id => "lose_count";

        public FTUELoseCountCondition(UITemplateLevelDataController uiTemplateLevelDataController) { this.uiTemplateLevelDataController = uiTemplateLevelDataController; }

        protected override bool IsPassedCondition(FTUELoseCountModel data) { return this.uiTemplateLevelDataController.TotalLose == data.Count; }
    }
}