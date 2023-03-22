namespace TheOneStudio.UITemplate.UITemplate.Models
{
    using System.Collections.Generic;

    public class UITemplateFTUEData
    {
        public HashSet<string> FinishedStep = new();
        public string          CurrentStep;
    }
}