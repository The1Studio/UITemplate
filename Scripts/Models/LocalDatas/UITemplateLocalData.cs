namespace TheOneStudio.UITemplate.Models
{
    using System;
    using GameFoundation.Scripts.Interfaces;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;

    public abstract class UITemplateLocalData<TController> : ILocalData, IUITemplateLocalData
    {
        Type IUITemplateLocalData.ControllerType => typeof(TController);

        void ILocalData.Init() => this.Init();

        protected virtual void Init()
        {
        }
    }
}