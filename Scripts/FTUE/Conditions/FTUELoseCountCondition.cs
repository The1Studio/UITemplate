namespace TheOneStudio.UITemplate.UITemplate.FTUE.Conditions
{
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using UnityEngine.Scripting;

    public class FTUELoseCountModel
    {
        public int Count { get; }

        [Preserve]
        public FTUELoseCountModel(int count)
        {
            this.Count = count;
        }
    }

    public class FTUELoseCountCondition : FtueCondition<FTUELoseCountModel>
    {
        #region inject

        private readonly UITemplateLevelDataController uiTemplateLevelDataController;

        #endregion

        public override string Id => "lose_count";

        [Preserve]
        public FTUELoseCountCondition(UITemplateLevelDataController uiTemplateLevelDataController)
        {
            this.uiTemplateLevelDataController = uiTemplateLevelDataController;
        }

        protected override string GetTooltipText(FTUELoseCountModel data)
        {
            return "";
        }

        protected override bool IsPassedCondition(FTUELoseCountModel data)
        {
            return this.uiTemplateLevelDataController.TotalLose == data.Count;
        }
    }
}