namespace TheOneStudio.UITemplate.UITemplate.Scenes.Play.End
{
    using Core.AdsServices;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateLoseScreenView : BaseView
    {
        public Button                 HomeButton;
        public Button                 ReplayButton;
        public UITemplateAdsButton    SkipButton;
        public UITemplateCurrencyView CurrencyView;
    }

    [ScreenInfo(nameof(UITemplateLoseScreenView))]
    public class UITemplateLoseScreenPresenter : UITemplateBaseScreenPresenter<UITemplateLoseScreenView>
    {
        #region Inject

        protected readonly DiContainer                       diContainer;
        protected readonly UITemplateAdServiceWrapper        adService;
        protected readonly IScreenManager                    screenManager;
        protected readonly UITemplateInventoryDataController inventoryDataController;

        public UITemplateLoseScreenPresenter(
            SignalBus                         signalBus,
            DiContainer                       diContainer,
            UITemplateAdServiceWrapper        adService,
            IScreenManager                    screenManager,
            UITemplateInventoryDataController inventoryDataController
        ) : base(signalBus)
        {
            this.diContainer             = diContainer;
            this.adService               = adService;
            this.screenManager           = screenManager;
            this.inventoryDataController = inventoryDataController;
        }

        #endregion

        protected virtual string AdPlacement => "replay";
        
        public override UniTask BindData()
        {
            this.View.SkipButton?.BindData(this.AdPlacement);
            this.View.CurrencyView.Subscribe(this.SignalBus, this.inventoryDataController.GetCurrencyValue());
            return UniTask.CompletedTask;
        }

        public override void Dispose()
        {
            base.Dispose();
            this.View.SkipButton?.Dispose();
            this.View.CurrencyView.Unsubscribe(this.SignalBus);
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.View.SkipButton?.OnViewReady(this.adService);
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
    }
}