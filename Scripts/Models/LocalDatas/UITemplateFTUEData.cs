namespace TheOneStudio.UITemplate.UITemplate.Models
{
    using System.Collections.Generic;
    using GameFoundation.Scripts.Interfaces;

    public class UITemplateFTUEData : ILocalData
    {
        public List<string> FinishedStep { get; set; } = new();
        public string       CurrentStep  { get; set; } = "";

        public void Init() { }
    }
}