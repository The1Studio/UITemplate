namespace TheOneStudio.UITemplate.UITemplate.Scenes.Play
{
    using Core.AdsServices;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateGameplayScreen : BaseView
    {
        [SerializeField] private Button                 btnHome;
        [SerializeField] private Button                 btnReplay;
        [SerializeField] private UITemplateAdsButton    btnSkip;
        [SerializeField] private UITemplateCurrencyView currencyView;

        public Button                 BtnHome      => this.btnHome;
        public Button                 BtnReplay    => this.btnReplay;
        public UITemplateAdsButton    BtnSkip      => this.btnSkip;
        public UITemplateCurrencyView CurrencyView => this.currencyView;
    }

    [ScreenInfo(nameof(UITemplateGameplayScreen))]
    public class UITemplateGameplayScreenPresenter : UITemplateBaseScreenPresenter<UITemplateGameplayScreen>
    {
        #region Inject

        protected readonly SceneDirector                     SceneDirector;
        protected readonly ScreenManager                     ScreenManager;
        protected readonly IAdServices                       adService;
        protected readonly UITemplateSoundServices           SoundServices;
        protected readonly UITemplateInventoryDataController inventoryDataController;

        public UITemplateGameplayScreenPresenter(
            SignalBus signalBus,
            SceneDirector sceneDirector,
            ScreenManager screenManager,
            IAdServices adService,
            UITemplateSoundServices soundServices,
            UITemplateInventoryDataController inventoryDataController
        ) : base(signalBus)
        {
            this.SceneDirector           = sceneDirector;
            this.ScreenManager           = screenManager;
            this.adService               = adService;
            this.SoundServices           = soundServices;
            this.inventoryDataController = inventoryDataController;
        }

        #endregion

        protected virtual string NextSceneToLoad => "1.MainScene";

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.View.BtnSkip?.OnViewReady(this.adService);
            this.View.BtnHome.onClick.AddListener(this.OnOpenHome);
            this.View.BtnReplay?.onClick.AddListener(this.OnClickReplay);
            this.View.BtnSkip?.onClick.AddListener(this.OnClickSkip);
        }

        public override void BindData()
        {
            this.View.BtnSkip?.BindData();
            this.View.CurrencyView.Subscribe(this.SignalBus, this.inventoryDataController.GetCurrencyValue());
        }

        public override void Dispose()
        {
            this.View.BtnSkip?.Dispose();
            this.View.CurrencyView.Unsubscribe(this.SignalBus);
        }

        protected virtual async void OnOpenHome()
        {
            await this.ScreenManager.OpenScreen<UITemplateHomeTapToPlayScreenPresenter>();
        }

        protected virtual void OnClickReplay()
        {
        }

        protected virtual void OnClickSkip()
        {
        }

        protected virtual void OpenNextScene()
        {
            this.SceneDirector.LoadSingleSceneAsync(this.NextSceneToLoad);
        }
    }
}