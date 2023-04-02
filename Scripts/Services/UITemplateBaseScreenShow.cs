namespace TheOneStudio.UITemplate.UITemplate.Scripts.Services
{
    using System;
    using GameFoundation.Scripts.Utilities.LogService;

    public interface IUITemplateScreenShow
    {
        Type ScreenPresenter { get; }
        void OnProcessScreenShow();
    }

    public abstract class UITemplateBaseScreenShow : IUITemplateScreenShow
    {
        private readonly ILogService logger;
        public abstract  Type        ScreenPresenter { get; }

        protected UITemplateBaseScreenShow(ILogService logger) { this.logger = logger; }

        public abstract void OnProcessScreenShow();
    }
}