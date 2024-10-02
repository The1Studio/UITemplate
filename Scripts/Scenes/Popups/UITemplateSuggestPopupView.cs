namespace TheOneStudio.UITemplate.UITemplate.Scenes.Popups
{
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.Utilities.LogService;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using TMPro;
    using UnityEngine;
    using UnityEngine.Scripting;
    using UnityEngine.UI;

    public class UITemplateSuggestPopupView : BaseView
    {
        [SerializeField]
        private TMP_Text textItemName;

        [SerializeField]
        private Image imageItemIcon;

        [SerializeField]
        private Button buttonClaim;

        [SerializeField]
        private Button buttonClose;

        public string ItemName
        {
            get => this.textItemName.text;
            set => this.textItemName.text = value;
        }

        public Sprite ItemIcon
        {
            get => this.imageItemIcon.sprite;
            set => this.imageItemIcon.sprite = value;
        }

        public Button ButtonClaim => this.buttonClaim;

        public Button ButtonClose => this.buttonClose;
    }

    public class UITemplateSuggestPopupModel
    {
        public string ItemID   { get; set; }
        public string ItemIcon { get; set; }
        public string ItemName { get; set; }
    }


    [PopupInfo(nameof(UITemplateSuggestPopupView))]
    public class UITemplateSuggestPopupPresenter : UITemplateBasePopupPresenter<UITemplateSuggestPopupView, UITemplateSuggestPopupModel>
    {
        #region Inject

        private readonly IGameAssets                gameAssets;
        private readonly UITemplateAdServiceWrapper uiTemplateAdServiceWrapper;

        [Preserve]
        public UITemplateSuggestPopupPresenter(SignalBus                  signalBus,
                                               ILogService                logger,
                                               IGameAssets                gameAssets,
                                               UITemplateAdServiceWrapper uiTemplateAdServiceWrapper)
            : base(signalBus, logger)
        {
            this.gameAssets                 = gameAssets;
            this.uiTemplateAdServiceWrapper = uiTemplateAdServiceWrapper;
        }

        #endregion

        protected UITemplateSuggestPopupModel PopupModel { get; private set; }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.View.ButtonClaim.onClick.AddListener(this.OnClaimButtonClicked);
            this.View.ButtonClose.onClick.AddListener(this.OnCloseButtonClicked);
        }

        protected virtual void OnCloseButtonClicked()
        {
            this.CloseView();
        }

        protected virtual void OnClaimButtonClicked()
        {
            this.uiTemplateAdServiceWrapper.ShowRewardedAd("Suggest", this.OnClaimSuccess);
        }

        protected virtual void OnClaimSuccess()
        {
        }

        public override async UniTask BindData(UITemplateSuggestPopupModel popupModel)
        {
            this.PopupModel = popupModel;
            // Bind ItemName
            this.View.ItemName = popupModel.ItemName;

            // Load sprite from addressable using IGameAssets
            var spriteIcon = await this.gameAssets.LoadAssetAsync<Sprite>(popupModel.ItemIcon).Task;
            this.View.ItemIcon = spriteIcon;
        }
    }
}