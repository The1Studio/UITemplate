namespace TheOneStudio.UITemplate.UITemplate.Scenes.IapScene
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.Utilities.LoadImage;
    using GameFoundation.Scripts.Utilities.LogService;
    using ServiceImplementation.IAPServices;
    using TheOneStudio.UITemplate.UITemplate.Extension;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateStaterPackModel
    {
        public string                             PackId           { get; set; }
        public string                             ImageGiftAddress { get; set; }
        public string                             PolicyAddress    { get; set; }
        public string                             TermsAddress     { get; set; }
        public List<UITemplateStartPackItemModel> StarterDatas     { get; set; } = new();
        public Action<string>                     OnComplete       { get; set; }
        public Action<string>                     OnFail           { get; set; }
        public Action                             OnRestore        { get; set; }
    }

    public class UITemplateStaterPackScreenView : BaseView
    {
        public Button                      btnClose;
        public Button                      btnBuy;
        public Button                      btnRestore;
        public Button                      btnPolicy;
        public Button                      btnTerms;
        public Image                       imgGift;
        public UITemplateStaterPackAdapter adapter;
    }

    [ScreenInfo(nameof(UITemplateStaterPackScreenView))]
    public class UITemplateStartPackScreenPresenter : UITemplateBaseScreenPresenter<UITemplateStaterPackScreenView, UITemplateStaterPackModel>
    {
        private readonly DiContainer       diContainer;
        private readonly LoadImageHelper   loadImageHelper;
        private readonly IUnityIapServices iapServices;

        public UITemplateStartPackScreenPresenter(SignalBus signalBus, DiContainer diContainer, LoadImageHelper loadImageHelper, IUnityIapServices iapServices, ILogService logger) : base(signalBus,
            logger)
        {
            this.diContainer     = diContainer;
            this.loadImageHelper = loadImageHelper;
            this.iapServices     = iapServices;
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.View.btnBuy.onClick.AddListener(this.OnBuyClick);
            this.View.btnRestore.onClick.AddListener(this.OnRestore);
            this.View.btnClose.onClick.AddListener(this.CloseView);
            this.View.btnTerms.onClick.AddListener(this.OnOpenTerm);
            this.View.btnPolicy.onClick.AddListener(this.OnOpenPolicy);
            this.View.btnRestore.gameObject.SetActive(false);
#if UNITY_IOS
            this.View.btnRestore.gameObject.SetActive(true);
#endif
        }

        private void OnOpenPolicy()
        {
            if (!this.Model.PolicyAddress.IsNullOrEmpty())
            {
                Application.OpenURL(this.Model.PolicyAddress);
            }
        }

        private void OnOpenTerm()
        {
            if (!this.Model.TermsAddress.IsNullOrEmpty())
            {
                Application.OpenURL(this.Model.TermsAddress);
            }
        }

        private void OnRestore() { this.iapServices.RestorePurchases(this.Model.OnRestore); }

        private void OnBuyClick() { this.iapServices.BuyProductID(this.Model.PackId, this.Model.OnComplete, this.Model.OnFail); }

        public override async UniTask BindData(UITemplateStaterPackModel screenModel)
        {
            this.View.imgGift.sprite = await this.loadImageHelper.LoadLocalSprite(screenModel.ImageGiftAddress);
            _                        = this.View.adapter.InitItemAdapter(screenModel.StarterDatas, this.diContainer);
        }
    }
}