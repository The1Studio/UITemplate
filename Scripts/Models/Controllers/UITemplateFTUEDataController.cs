namespace TheOneStudio.UITemplate.UITemplate.Models.Controllers
{
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using UnityEngine.Scripting;

    public class UITemplateFTUEDataController : IUITemplateControllerData
    {
        #region inject

        private readonly UITemplateFTUEBlueprint uiTemplateFtueBlueprint;
        private readonly UITemplateFTUEData      templateFtueData;

        #endregion

        [Preserve]
        public UITemplateFTUEDataController(UITemplateFTUEData templateFtueData, UITemplateFTUEBlueprint uiTemplateFtueBlueprint)
        {
            this.templateFtueData        = templateFtueData;
            this.uiTemplateFtueBlueprint = uiTemplateFtueBlueprint;
        }
        public bool IsFinishedStep(string stepId) => this.templateFtueData.FinishedStep.Contains(stepId);

        public void CompleteStep(string stepId)
        {
            if (this.templateFtueData.FinishedStep.Contains(stepId)) return;
            this.templateFtueData.FinishedStep.Add(stepId);
            foreach (var previousStep in this.uiTemplateFtueBlueprint.GetDataById(stepId).PreviousSteps)
            {
                this.CompleteStep(previousStep);
            }
        }
    }
}