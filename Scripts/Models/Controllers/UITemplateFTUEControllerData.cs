namespace TheOneStudio.UITemplate.UITemplate.Models.Controllers
{
    using System.Collections.Generic;
    using System.Linq;

    public class UITemplateFTUEControllerData:IUITemplateControllerData
    {
        private readonly UITemplateFTUEData templateFtueData;

        public UITemplateFTUEControllerData(UITemplateFTUEData templateFtueData) { this.templateFtueData = templateFtueData; }
        public bool IsFinishedStep(string stepId) => this.templateFtueData.FinishedStep.Contains(stepId);

        public void CompleteStep(string stepId)
        {
            if (this.templateFtueData.FinishedStep.Contains(stepId)) return;
            this.templateFtueData.FinishedStep.Add(stepId);
        }

        public bool IsCompleteAllRequireCondition(List<string> listStepRequire)
        {
            return listStepRequire.Count == 0 || listStepRequire.All(step => this.templateFtueData.FinishedStep.Contains(step));
        }
    }
}