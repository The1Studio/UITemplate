namespace TheOneStudio.UITemplate.UITemplate.Models
{
    using System.Collections.Generic;
    using GameFoundation.Scripts.Interfaces;

    public class UITemplateFTUEData:ILocalData
    {
        internal List<string> FinishedStep = new();
        internal string       CurrentStep { get; set; } = "";

        public void Init()
        {
            
        }
    }
}