namespace TheOneStudio.UITemplate.UITemplate.Models
{
    using System.Collections.Generic;

    public class UITemplateFTUEData
    {
        internal HashSet<string> FinishedStep { get; set; } = new();
        internal string          CurrentStep  { get; set; }
    }
}