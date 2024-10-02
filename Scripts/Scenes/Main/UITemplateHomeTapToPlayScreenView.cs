namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main
{
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.Utilities.LogService;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main.CollectionNew;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Play;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using UnityEngine.Scripting;
    using UnityEngine.UI;

    public class UITemplateHomeTapToPlayScreenView : BaseView
    {
        public Button                      TaptoplayButton;
        public Button                      ShopButton;
        public UITemplateCurrencyView      CoinText;
        public UITemplateSettingButtonView SettingButtonView;
    }

    [ScreenInfo(nameof(UITemplateHomeTapToPlayScreenView))]
    public class UITemplateHomeTapToPlayScreenPresenter : UITemplateBaseScreenPresenter<UITemplateHomeTapToPlayScreenView>
    {
        [Preserve]
        public UITemplateHomeTapToPlayScreenPresenter(
            SignalBus                         signalBus,
            ILogService                       logger,
            IScreenManager                    screenManager,
            UITemplateInventoryDataController uiTemplateInventoryDataController,
            UITemplateSoundServices           soundServices
        ) : base(signalBus, logger)
        {
            this.ScreenManager                     = screenManager;
            this.uiTemplateInventoryDataController = uiTemplateInventoryDataController;
            this.SoundServices                     = soundServices;
        }

        protected override async void OnViewReady()
        {
            base.OnViewReady();
            await this.OpenViewAsync();
            this.View.TaptoplayButton.onClick.AddListener(this.OnClickTapToPlayButton);
            this.View.ShopButton.onClick.AddListener(this.OnClickShopButton);
        }

        public override UniTask BindData()
        {
            return UniTask.CompletedTask;
        }

        protected virtual void OnClickShopButton()
        {
            this.ScreenManager.OpenScreen<UITemplateNewCollectionScreenPresenter>();
        }

        protected virtual void OnClickTapToPlayButton()
        {
            this.ScreenManager.OpenScreen<UITemplateGameplayScreenPresenter>();
        }

        #region inject

        protected readonly IScreenManager                    ScreenManager;
        private readonly   UITemplateInventoryDataController uiTemplateInventoryDataController;
        protected readonly UITemplateSoundServices           SoundServices;

        #endregion
    }
}