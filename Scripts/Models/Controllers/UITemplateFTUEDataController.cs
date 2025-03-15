namespace TheOneStudio.UITemplate.UITemplate.Models.Controllers
{
    using Cysharp.Threading.Tasks;
    using UnityEngine.Scripting;

    public class UITemplateFTUEDataController
    {
        [Preserve]
        public UITemplateFTUEDataController(
            UITemplateFTUEData                 templateFtueData,
            UITemplateInventoryDataController  uiTemplateInventoryDataController
        )
        {
            this.templateFtueData                  = templateFtueData;
            this.uiTemplateInventoryDataController = uiTemplateInventoryDataController;
        }

        public bool IsFinishedStep(string stepId) { return this.templateFtueData.FinishedStep.Contains(stepId); }

        public bool TryCompleteStep(string stepId)
        {
            if (this.templateFtueData.FinishedStep.Contains(stepId)) return false;
            this.templateFtueData.FinishedStep.Add(stepId);
            return true;
        }

        public bool IsRewardedStep(string stepId) { return this.templateFtueData.RewardedStep.Contains(stepId); }

        public bool TryGiveReward(string stepId)
        {
            if (this.IsRewardedStep(stepId)) return false;
            this.templateFtueData.RewardedStep.Add(stepId);
            return true;
        }

        #region inject

        private readonly UITemplateInventoryDataController  uiTemplateInventoryDataController;
        private readonly UITemplateFTUEData                 templateFtueData;

        #endregion
    }
}