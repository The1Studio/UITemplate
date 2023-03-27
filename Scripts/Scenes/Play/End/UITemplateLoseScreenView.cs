namespace TheOneStudio.UITemplate.UITemplate.Scenes.Play.End
{
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateLoseScreenView : BaseView
    {
        public Button                 HomeButton;
        public Button                 ReplayButton;
        public Button                 SkipButton;
        public UITemplateCurrencyView CurrencyView;
    }

    [ScreenInfo(nameof(UITemplateLoseScreenView))]
    public class UITemplateLoseScreenPresenter : UITemplateBaseScreenPresenter<UITemplateLoseScreenView>
    {
        public UITemplateLoseScreenPresenter(SignalBus signalBus, DiContainer diContainer, IScreenManager screenManager, UITemplateInventoryDataController inventoryDataController) : base(signalBus)
        {
            this.diContainer             = diContainer;
            this.screenManager           = screenManager;
            this.inventoryDataController = inventoryDataController;
        }

        public override void BindData()
        {
            this.View.CurrencyView.Subscribe(this.SignalBus, this.inventoryDataController.GetCurrency().Value);
        }

        public override void Dispose()
        {
            base.Dispose();
            this.View.CurrencyView.Unsubscribe(this.SignalBus);
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.View.HomeButton?.onClick.AddListener(this.OnClickHome);
            this.View.ReplayButton?.onClick.AddListener(this.OnClickReplay);
            this.View.SkipButton?.onClick.AddListener(this.OnClickSkip);
        }

        protected virtual void OnClickHome()
        {
            this.screenManager.OpenScreen<UITemplateHomeSimpleScreenPresenter>();
        }

        protected virtual void OnClickReplay()
        {
        }

        protected virtual void OnClickSkip()
        {
        }

        #region inject

        protected readonly DiContainer                       diContainer;
        protected readonly IScreenManager                    screenManager;
        protected readonly UITemplateInventoryDataController inventoryDataController;

        #endregion
    }
}