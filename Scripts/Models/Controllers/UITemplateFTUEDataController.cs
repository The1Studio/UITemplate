namespace TheOneStudio.UITemplate.UITemplate.Models.Controllers
{
    using Cysharp.Threading.Tasks;
    using TheOneStudio.UITemplate.UITemplate.FTUE.RemoteConfig;
    using UnityEngine.Scripting;

    public class UITemplateFTUEDataController
    {
        [Preserve]
        public UITemplateFTUEDataController(
            UITemplateFTUEData                 templateFtueData,
            UITemplateFTUEBlueprintDataHandler uiTemplateFtueBlueprint,
            UITemplateInventoryDataController  uiTemplateInventoryDataController
        )
        {
            this.templateFtueData                  = templateFtueData;
            this.uiTemplateFtueBlueprint           = uiTemplateFtueBlueprint;
            this.uiTemplateInventoryDataController = uiTemplateInventoryDataController;
        }

        public bool IsFinishedStep(string stepId) { return this.templateFtueData.FinishedStep.Contains(stepId); }

        public void CompleteStep(string stepId)
        {
            if (this.templateFtueData.FinishedStep.Contains(stepId)) return;
            this.templateFtueData.FinishedStep.Add(stepId);
            foreach (var previousStep in this.uiTemplateFtueBlueprint.GetDataById(stepId).PreviousSteps) this.CompleteStep(previousStep);
        }

        public bool IsRewardedStep(string stepId) { return this.templateFtueData.RewardedStep.Contains(stepId); }

        public void GiveReward(string stepId)
        {
            if (this.IsRewardedStep(stepId)) return;
            this.templateFtueData.RewardedStep.Add(stepId);
            foreach (var pair in this.uiTemplateFtueBlueprint.GetDataById(stepId).BonusOnStart)
            {
                this.uiTemplateInventoryDataController.AddCurrency(pair.Value, pair.Key, "ftue").Forget();
            }
        }

        #region inject

        private readonly UITemplateFTUEBlueprintDataHandler uiTemplateFtueBlueprint;
        private readonly UITemplateInventoryDataController  uiTemplateInventoryDataController;
        private readonly UITemplateFTUEData                 templateFtueData;

        #endregion
    }
}