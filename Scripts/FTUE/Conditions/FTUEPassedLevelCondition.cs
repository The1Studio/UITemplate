namespace TheOneStudio.UITemplate.UITemplate.FTUE.Conditions
{
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;

    public class FTUEPassedLevelConditionModel
    {
        public int Level;
    }

    public class FTUEPassedLevelCondition : FtueCondition<FTUEPassedLevelConditionModel>
    {
        #region inject

        private readonly UITemplateLevelDataController uiTemplateLevelDataController;

        #endregion

        public FTUEPassedLevelCondition(UITemplateLevelDataController uiTemplateLevelDataController)
        {
            this.uiTemplateLevelDataController = uiTemplateLevelDataController;
        }

        public override string Id => "passed_level";

        protected override bool IsPassedCondition(FTUEPassedLevelConditionModel data) => this.uiTemplateLevelDataController.CurrentLevel >= data.Level;
    }
}