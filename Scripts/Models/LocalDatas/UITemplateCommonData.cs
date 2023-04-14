namespace TheOneStudio.UITemplate.UITemplate.Models.LocalDatas
{
    using GameFoundation.Scripts.Interfaces;

    public class UITemplateCommonData : ILocalData
    {
        public bool IsFirstTimeOpenGame { get; set; } = true;
        public void Init()              { }
    }
}