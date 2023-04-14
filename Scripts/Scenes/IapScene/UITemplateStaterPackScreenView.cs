namespace TheOneStudio.UITemplate.UITemplate.Scenes.IapScene
{
    using System;
    using System.Collections.Generic;
    using Core.AdsServices;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.Utilities.LoadImage;
    using GameFoundation.Scripts.Utilities.LogService;
    using ServiceImplementation.IAPServices;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Extension;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateStaterPackModel
    {
        public Action<string> OnComplete { get; set; }
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
        private readonly IAdServices                  adServices;
        private readonly UITemplateShopPackBlueprint  uiTemplateShopPackBlueprint;
        private readonly UITemplateIapServices        uiTemplateIapServices;
        private readonly UITemplateMiscParamBlueprint uiTemplateMiscParamBlueprint;
        private readonly DiContainer                  diContainer;
        private readonly LoadImageHelper              loadImageHelper;
        private readonly IUnityIapServices            iapServices;
        private     string                       IapPack = "com.car.climb.draw.bridge.start_pack_1";

        public UITemplateStartPackScreenPresenter(SignalBus signalBus,IAdServices adServices, UITemplateShopPackBlueprint uiTemplateShopPackBlueprint, UITemplateIapServices uiTemplateIapServices,
            UITemplateMiscParamBlueprint uiTemplateMiscParamBlueprint, DiContainer diContainer,
            LoadImageHelper loadImageHelper,
            IUnityIapServices iapServices, ILogService logger) : base(signalBus,
            logger)
        {
            this.adServices                   = adServices;
            this.uiTemplateShopPackBlueprint  = uiTemplateShopPackBlueprint;
            this.uiTemplateIapServices        = uiTemplateIapServices;
            this.uiTemplateMiscParamBlueprint = uiTemplateMiscParamBlueprint;
            this.diContainer                  = diContainer;
            this.loadImageHelper              = loadImageHelper;
            this.iapServices                  = iapServices;
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
            if (!string.IsNullOrEmpty(this.uiTemplateMiscParamBlueprint.PolicyAddress))
            {
                Application.OpenURL(this.uiTemplateMiscParamBlueprint.PolicyAddress);
            }
        }

        private void OnOpenTerm()
        {
            if (!string.IsNullOrEmpty(this.uiTemplateMiscParamBlueprint.TermsAddress))
            {
                Application.OpenURL(this.uiTemplateMiscParamBlueprint.TermsAddress);
            }
        }

        private void OnRestore() { this.uiTemplateIapServices.RestorePurchase(() => { }); }

        private void OnBuyClick()
        {
            this.uiTemplateIapServices.BuyProduct(this.View.btnBuy.gameObject, this.IapPack, (x) =>
            {
                this.CloseView();
                this.Model.OnComplete?.Invoke(x);
            });
        }

        public override async UniTask BindData(UITemplateStaterPackModel screenModel)
        {
            if (this.adServices.IsRemoveAds())
            {
                this.IapPack= "com.car.climb.draw.bridge.start_pack_2";
            }

            if (this.uiTemplateShopPackBlueprint.TryGetValue(IapPack, out var shopPackRecord))
            {
                if (!shopPackRecord.ImageAddress.IsNullOrEmpty())
                {
                    this.View.imgGift.sprite = await this.loadImageHelper.LoadLocalSprite(shopPackRecord.ImageAddress);
                }

                var model = new List<UITemplateStartPackItemModel>();

                foreach (var rewardBlueprintData in shopPackRecord.RewardIdToRewardDatas)
                {
                    model.Add(new UITemplateStartPackItemModel()
                    {
                        IconAddress = rewardBlueprintData.Value.RewardIcon,
                        Value       = rewardBlueprintData.Value.RewardContent
                    });
                }

                _ = this.View.adapter.InitItemAdapter(model, this.diContainer);
            }
        }
    }
}