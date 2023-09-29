namespace TheOneStudio.UITemplate.UITemplate.Models.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;

    public class UITemplateFTUEControllerData:IUITemplateControllerData
    {
        #region inject

        private readonly UITemplateFTUEBlueprint uiTemplateFtueBlueprint;
        private readonly UITemplateFTUEData      templateFtueData;

        #endregion

        public UITemplateFTUEControllerData(UITemplateFTUEData templateFtueData, UITemplateFTUEBlueprint uiTemplateFtueBlueprint)
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

        public bool IsCompleteAllRequireCondition(List<string> listStepRequire)
        {
            return listStepRequire.Count == 0 || listStepRequire.All(step => this.templateFtueData.FinishedStep.Contains(step));
        }
    }
}