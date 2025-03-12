namespace TheOneStudio.UITemplate.UITemplate.FTUE.Conditions
{
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using UnityEngine.Scripting;

    public class FTUEPassedLevelConditionModel
    {
        public int Level { get; }

        [Preserve]
        public FTUEPassedLevelConditionModel(int level)
        {
            this.Level = level;
        }
    }

    public class FTUEPassedLevelCondition : FtueCondition<FTUEPassedLevelConditionModel>
    {
        #region inject

        private readonly UITemplateLevelDataController uiTemplateLevelDataController;

        #endregion

        [Preserve]
        public FTUEPassedLevelCondition(UITemplateLevelDataController uiTemplateLevelDataController)
        {
            this.uiTemplateLevelDataController = uiTemplateLevelDataController;
        }

        public override string Id => "passed_level";

        protected override string GetTooltipText(FTUEPassedLevelConditionModel data) => $"You need to pass level {data.Level}";

        protected override bool IsPassedCondition(FTUEPassedLevelConditionModel data) => this.uiTemplateLevelDataController.MaxLevel >= data.Level;

        protected override string GetShortConditionText(FTUEPassedLevelConditionModel data) => $"Pass Level {data.Level}";
    }
}