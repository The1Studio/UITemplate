namespace TheOneStudio.UITemplate.UITemplate.Scripts.Services
{
    using System;

    public interface IUITemplateScreenShow
    {
        Type ScreenPresenter { get; }
        void OnProcessScreenShow();
    }

    public abstract class UITemplateBaseScreenShow : IUITemplateScreenShow
    {
        public abstract Type ScreenPresenter { get; }

        public abstract void OnProcessScreenShow();
    }
}