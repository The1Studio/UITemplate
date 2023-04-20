namespace TheOneStudio.UITemplate.UITemplate.Models.LocalDatas
{
    using System.Collections.Generic;
    using GameFoundation.Scripts.Interfaces;

    public class UITemplateIAPOwnerPackData : ILocalData
    {
        public List<string> OwnedPacks { get; set; } = new ();

        public void Init() { }
    }
}