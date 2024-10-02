namespace TheOneStudio.UITemplate.UITemplate.Scenes.Play.End
{
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.Utilities.LogService;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using UnityEngine.Scripting;
    using UnityEngine.UI;

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

        protected readonly UITemplateAdServiceWrapper        adService;
        private readonly   UITemplateSoundServices           soundServices;
        protected readonly IScreenManager                    screenManager;
        protected readonly UITemplateInventoryDataController inventoryDataController;

        [Preserve]
        public UITemplateLoseScreenPresenter(
            SignalBus                         signalBus,
            ILogService                       logger,
            UITemplateAdServiceWrapper        adService,
            UITemplateSoundServices           soundServices,
            IScreenManager                    screenManager,
            UITemplateInventoryDataController inventoryDataController
        ) : base(signalBus, logger)
        {
            this.adService               = adService;
            this.soundServices           = soundServices;
            this.screenManager           = screenManager;
            this.inventoryDataController = inventoryDataController;
        }

        #endregion

        protected virtual string AdPlacement => "replay";

        protected override void OnViewReady()
        {
            base.OnViewReady();

            if (this.View.SkipButton != null)
            {
                this.View.SkipButton.OnViewReady(this.adService);
            }

            if (this.View.HomeButton != null)
            {
                this.View.HomeButton.onClick.AddListener(this.OnClickHome);
            }

            if (this.View.ReplayButton != null)
            {
                this.View.ReplayButton.onClick.AddListener(this.OnClickReplay);
            }

            if (this.View.SkipButton != null)
            {
                this.View.SkipButton.onClick.AddListener(this.OnClickSkip);
            }
        }

        public override UniTask BindData()
        {
            if (this.View.SkipButton != null)
            {
                this.View.SkipButton.BindData(this.AdPlacement);
            }

            this.soundServices.PlaySoundLose();
            return UniTask.CompletedTask;
        }

        public override void Dispose()
        {
            base.Dispose();
            if (this.View.SkipButton != null)
            {
                this.View.SkipButton.Dispose();
            }
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