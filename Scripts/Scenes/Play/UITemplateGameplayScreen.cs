using TMPro;

namespace TheOneStudio.UITemplate.UITemplate.Scenes.Play
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
        [SerializeField] private TextMeshProUGUI        levelText;

        public Button                 BtnHome      => this.btnHome;
        public Button                 BtnReplay    => this.btnReplay;
        public UITemplateAdsButton    BtnSkip      => this.btnSkip;
        public UITemplateCurrencyView CurrencyView => this.currencyView;
        public TextMeshProUGUI        LevelText    => this.levelText;
    }

    [ScreenInfo(nameof(UITemplateGameplayScreen))]
    public class UITemplateGameplayScreenPresenter : UITemplateBaseScreenPresenter<UITemplateGameplayScreen>
    {
        #region Inject

        protected readonly SceneDirector                     SceneDirector;
        protected readonly ScreenManager                     ScreenManager;
        protected readonly UITemplateAdServiceWrapper        adService;
        protected readonly UITemplateSoundServices           SoundServices;
        protected readonly UITemplateInventoryDataController inventoryDataController;
        protected readonly UITemplateLevelDataController     levelDataController;

        public UITemplateGameplayScreenPresenter(SignalBus signalBus, SceneDirector sceneDirector, ScreenManager screenManager, UITemplateAdServiceWrapper adService, UITemplateSoundServices soundServices,
                                                 UITemplateInventoryDataController inventoryDataController, UITemplateLevelDataController levelDataController) : base(signalBus)
        {
            this.SceneDirector           = sceneDirector;
            this.ScreenManager           = screenManager;
            this.adService               = adService;
            this.SoundServices           = soundServices;
            this.inventoryDataController = inventoryDataController;
            this.levelDataController     = levelDataController;
        }

        #endregion

        protected virtual string NextSceneToLoad => "1.MainScene";
        protected virtual string AdPlacement     => "skip_level";

        protected override void OnViewReady()
        {
            base.OnViewReady();
            if (this.View.BtnSkip != null)
            {
                this.View.BtnSkip.OnViewReady(this.adService);
            }
            
            if (this.View.BtnHome != null)
            {
                this.View.BtnHome.onClick.AddListener(this.OnOpenHome);
            }

            if (this.View.BtnReplay != null)
            {
                this.View.BtnReplay.onClick.AddListener(this.OnClickReplay);
            }

            if (this.View.BtnSkip != null)
            {
                this.View.BtnSkip.onClick.AddListener(this.OnClickSkip);
            }
        }

        public override UniTask BindData()
        {

            if (this.View.BtnSkip != null)
            {
                this.View.BtnSkip.BindData(this.AdPlacement);
            }

            if (this.View.CurrencyView != null)
            {
                this.View.CurrencyView.Subscribe(this.SignalBus, this.inventoryDataController.GetCurrencyValue());
            }

            if (this.View.LevelText != null)
            {
                this.View.LevelText.text = "Level " + levelDataController.GetCurrentLevelData.Level;
            }
            return UniTask.CompletedTask;
        }

        public override void Dispose()
        {
            if (this.View.BtnSkip != null)
            {
                this.View.BtnSkip.Dispose();
            }

            if (this.View.CurrencyView != null)
            {
                this.View.CurrencyView.Unsubscribe(this.SignalBus);
            }
        }

        protected virtual async void OnOpenHome() { await this.ScreenManager.OpenScreen<UITemplateHomeTapToPlayScreenPresenter>(); }

        protected virtual void OnClickReplay() { }

        protected virtual void OnClickSkip() { }

        protected virtual void OpenNextScene() { this.SceneDirector.LoadSingleSceneAsync(this.NextSceneToLoad); }
    }
}