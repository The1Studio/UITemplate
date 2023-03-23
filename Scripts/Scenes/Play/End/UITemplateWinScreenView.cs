namespace TheOneStudio.UITemplate.UITemplate.Scenes.Play.End
{
    using System;
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.Utilities.LogService;
    using Sirenix.OdinInspector;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateWinScreenModel
    {
        public readonly string ItemId;
        public readonly float  ItemUnlockLastValue;
        public readonly float  ItemUnlockNewValue;
        public readonly int    StarRate;

        public UITemplateWinScreenModel(string itemId, float itemUnlockLastValue, float itemUnlockNewValue, int starRate = 3)
        {
            this.ItemId              = itemId;
            this.ItemUnlockLastValue = itemUnlockLastValue;
            this.ItemUnlockNewValue  = itemUnlockNewValue;
            this.StarRate            = starRate;
        }
    }

    public class UITemplateWinScreenView : BaseView
    {
        [SerializeField] private Button                 btnHome;
        [SerializeField] private Button                 btnReplay;
        [SerializeField] private Button                 btnNext;
        [SerializeField] private UITemplateCurrencyView currencyView;

        [SerializeField] private bool useItemUnlockProgressText;

        [SerializeField] [ShowIf("useItemUnlockProgressText")]
        private TMP_Text txtItemUnlockProgress;

        [SerializeField] private bool useItemUnlockProgressImage;

        [SerializeField] [ShowIf("useItemUnlockProgressImage")]
        private Image imgItemUnlockProgress;

        [SerializeField] [ShowIf("useItemUnlockProgressImage")]
        private Image imgItemUnlockProgressBackground;

        [SerializeField] private bool useItemUnlockProgressSlider;

        [SerializeField] [ShowIf("useItemUnlockProgressSlider")]
        private Slider sliderItemUnlockProgress;

        [SerializeField] private bool useLightGlow;

        [SerializeField] [ShowIf("useLightGlow")]
        private Image imgLightGlow;

        [SerializeField] private bool useStarRate;

        [SerializeField] [ShowIf("UseStarRate")]
        private UITemplateStarRateView starRateView;

        public Button                 BtnHome                         => this.btnHome;
        public Button                 BtnReplay                       => this.btnReplay;
        public Button                 BtnNext                         => this.btnNext;
        public UITemplateCurrencyView CurrencyView                    => this.currencyView;
        public bool                   UseItemUnlockProgressText       => this.useItemUnlockProgressText;
        public TMP_Text               TxtItemUnlockProgress           => this.txtItemUnlockProgress;
        public bool                   UseItemUnlockProgressImage      => this.useItemUnlockProgressImage;
        public Image                  ImgItemUnlockProgress           => this.imgItemUnlockProgress;
        public Image                  ImgItemUnlockProgressBackground => this.imgItemUnlockProgressBackground;
        public bool                   UseItemUnlockProgressSlider     => this.useItemUnlockProgressSlider;
        public Slider                 SliderItemUnlockProgress        => this.sliderItemUnlockProgress;
        public bool                   UseLightGlow                    => this.useLightGlow;
        public Image                  ImgLightGlow                    => this.imgLightGlow;
        public bool                   UseStarRate                     => this.useStarRate;
        public UITemplateStarRateView StarRateView                    => this.starRateView;
    }

    [ScreenInfo(nameof(UITemplateWinScreenView))]
    public class UITemplateWinScreenPresenter : UITemplateBaseScreenPresenter<UITemplateWinScreenView, UITemplateWinScreenModel>
    {
        #region Inject

        protected readonly UITemplateInventoryData           inventoryData;
        protected readonly IScreenManager                    screenManager;
        protected readonly IGameAssets                       gameAssets;
        protected readonly UITemplateInventoryDataController uiTemplateInventoryDataController;
        protected readonly UITemplateSoundServices           soundServices;

        public UITemplateWinScreenPresenter(
            SignalBus signalBus,
            ILogService logService,
            UITemplateInventoryData inventoryData,
            IScreenManager screenManager,
            IGameAssets gameAssets,
            UITemplateInventoryDataController uiTemplateInventoryDataController,
            UITemplateSoundServices soundServices
        ) : base(signalBus, logService)
        {
            this.inventoryData                     = inventoryData;
            this.screenManager                     = screenManager;
            this.gameAssets                        = gameAssets;
            this.uiTemplateInventoryDataController = uiTemplateInventoryDataController;
            this.soundServices                     = soundServices;
        }

        #endregion

        private IDisposable spinDisposable;
        private Tween       tweenSpin;

        protected override async void OnViewReady()
        {
            base.OnViewReady();
            await this.OpenViewAsync();
            this.View.BtnHome.onClick.AddListener(this.OnClickHome);
            this.View.BtnReplay.onClick.AddListener(this.OnClickReplay);
            this.View.BtnNext.onClick.AddListener(this.OnClickNext);
        }

        public override async void BindData(UITemplateWinScreenModel model)
        {
            this.View.CurrencyView.Subscribe(this.SignalBus, this.uiTemplateInventoryDataController.GetCurrency(UITemplateItemData.UnlockType.SoftCurrency.ToString()).Value);
            this.ItemUnlockProgress(model.ItemUnlockLastValue, model.ItemUnlockNewValue);

            if (this.View.UseLightGlow)
            {
                this.tweenSpin = this.View.ImgLightGlow.transform.DORotate(new Vector3(0, 0, -360), 5f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
            }

            if (this.View.UseStarRate)
            {
                await this.View.StarRateView.SetStarRate(model.StarRate);
            }
        }

        protected async void ItemUnlockProgress(float lastValue, float newValue)
        {
            var sequence = DOTween.Sequence();

            if (this.View.UseItemUnlockProgressText)
            {
                var currentValue = lastValue;
                this.View.TxtItemUnlockProgress.text = $"{currentValue * 100:N0}%";
                sequence.Join(
                    DOTween.To(
                        getter: () => currentValue,
                        setter: value =>
                        {
                            currentValue                         = value;
                            this.View.TxtItemUnlockProgress.text = $"{currentValue * 100:N0}%";
                        },
                        endValue: newValue,
                        duration: .5f
                    ).SetEase(Ease.Linear)
                );
            }

            if (this.View.UseItemUnlockProgressImage)
            {
                var sprite = await this.gameAssets.LoadAssetAsync<Sprite>(this.Model.ItemId);
                this.View.ImgItemUnlockProgress.sprite           = sprite;
                this.View.ImgItemUnlockProgressBackground.sprite = sprite;
                sequence.Join(
                    DOTween.To(
                        getter: () => this.View.ImgItemUnlockProgress.fillAmount    = lastValue,
                        setter: value => this.View.ImgItemUnlockProgress.fillAmount = value,
                        endValue: newValue,
                        duration: .5f
                    ).SetEase(Ease.Linear)
                );
            }

            if (this.View.UseItemUnlockProgressSlider)
            {
                sequence.Join(
                    DOTween.To(
                        getter: () => this.View.SliderItemUnlockProgress.value    = lastValue,
                        setter: value => this.View.SliderItemUnlockProgress.value = value,
                        endValue: newValue,
                        duration: .5f
                    ).SetEase(Ease.Linear)
                );
            }

            if (newValue < 1f)
            {
                this.uiTemplateInventoryDataController.UpdateStatusItemData(this.Model.ItemId, UITemplateItemData.Status.InProgress);
            }
            else
            {
                sequence.onComplete += this.OnItemUnlock;
            }
        }

        protected virtual void OnClickHome()
        {
            this.screenManager.OpenScreen<UITemplateHomeSimpleScreenPresenter>();
        }

        protected virtual void OnClickReplay()
        {
            this.screenManager.OpenScreen<UITemplateHomeSimpleScreenPresenter>();
        }

        protected virtual void OnClickNext()
        {
            this.screenManager.OpenScreen<UITemplateHomeSimpleScreenPresenter>();
        }

        protected virtual void OnItemUnlock()
        {
            this.uiTemplateInventoryDataController.UpdateStatusItemData(this.Model.ItemId, UITemplateItemData.Status.Owned);
        }

        public override void Dispose()
        {
            base.Dispose();
            DOTween.Kill(this.tweenSpin);
            this.View.CurrencyView.Unsubscribe(this.SignalBus);
        }
    }
}