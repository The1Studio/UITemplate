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
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateWinScreenModel
    {
        public int    StarRate;
        public string ItemId;
        public float  ItemUnlockLastPercent;
        public float  ItemUnlockPercent;

        public UITemplateWinScreenModel(int starRate, string itemId, float itemUnlockLastPercent, float itemUnlockPercent)
        {
            this.StarRate              = starRate;
            this.ItemId                = itemId;
            this.ItemUnlockLastPercent = itemUnlockLastPercent;
            this.ItemUnlockPercent     = itemUnlockPercent;
        }
    }

    public class UITemplateWinScreenView : BaseView
    {
        public Button                 HomeButton;
        public Button                 ReplayEndgameButton;
        public Button                 NextEndgameButton;
        public UITemplateCurrencyView CoinText;
        public UITemplateStarRateView StarRateView;
        public Image                  LightGlowImage;
        public Image                  ItemUnlockBgImage;
        public Image                  ItemUnlockImage;
    }

    [ScreenInfo(nameof(UITemplateWinScreenView))]
    public class UITemplateWinScreenPresenter : BaseScreenPresenter<UITemplateWinScreenView, UITemplateWinScreenModel>
    {
        #region inject

        private readonly UITemplateInventoryData           inventoryData;
        private readonly IScreenManager                    screenManager;
        private readonly IGameAssets                       gameAssets;
        private readonly UITemplateInventoryDataController uiTemplateInventoryDataController;
        private readonly UITemplateSoundServices            soundServices;

        #endregion
       

        private IDisposable spinDisposable;
        private Tween       tweenSpin;

        public UITemplateWinScreenPresenter(SignalBus signalBus, UITemplateInventoryData inventoryData, ILogService logService, IScreenManager screenManager, IGameAssets gameAssets, UITemplateInventoryDataController uiTemplateInventoryDataController, UITemplateSoundServices soundServices) : base(signalBus, logService)
        {
            this.inventoryData                     = inventoryData;
            this.screenManager                     = screenManager;
            this.gameAssets                        = gameAssets;
            this.uiTemplateInventoryDataController = uiTemplateInventoryDataController;
            this.soundServices                      = soundServices;
        }

        protected override async void OnViewReady()
        {
            base.OnViewReady();
            await this.OpenViewAsync();
            this.View.HomeButton.onClick.AddListener(this.OnClickHome);
            this.View.ReplayEndgameButton.onClick.AddListener(this.OnClickReplay);
            this.View.NextEndgameButton.onClick.AddListener(this.OnClickNext);
        }

        public override async void BindData(UITemplateWinScreenModel model)
        {
            this.View.CoinText.Subscribe(this.SignalBus, this.uiTemplateInventoryDataController.GetCurrency(UITemplateItemData.UnlockType.SoftCurrency.ToString()).Value);
            this.ItemUnlockProgress(model.ItemUnlockLastPercent, model.ItemUnlockPercent);
            this.tweenSpin = this.View.LightGlowImage.transform.DORotate(new Vector3(0, 0, -360), 5f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
            await this.View.StarRateView.SetStarRate(model.StarRate);
        }

        private async void ItemUnlockProgress(float lastPercent, float percent)
        {
            var spriteItemUnlock = await this.gameAssets.LoadAssetAsync<Sprite>(this.Model.ItemId);
            this.View.ItemUnlockImage.sprite   = spriteItemUnlock;
            this.View.ItemUnlockBgImage.sprite = spriteItemUnlock;
            var lastValue = lastPercent / 100f;
            var value     = percent     / 100f;
            DOTween.To(() => this.View.ItemUnlockImage.fillAmount = lastValue, x => this.View.ItemUnlockImage.fillAmount = x, value, 1f).SetEase(Ease.Linear);

            var itemStatus = Math.Abs(value - 1) < Mathf.Epsilon ? UITemplateItemData.Status.Owned : UITemplateItemData.Status.InProgress;
            this.uiTemplateInventoryDataController.UpdateStatusItemData(this.Model.ItemId, itemStatus);
        }

        protected virtual void OnClickHome()
        {
            this.soundServices.PlaySoundClick();
            this.screenManager.OpenScreen<UITemplateHomeSimpleScreenPresenter>();
        }

        protected virtual void OnClickReplay()
        {
            this.soundServices.PlaySoundClick();
            this.screenManager.OpenScreen<UITemplateHomeSimpleScreenPresenter>();
        }

        protected virtual void OnClickNext()
        {
            this.soundServices.PlaySoundClick();
            this.screenManager.OpenScreen<UITemplateHomeSimpleScreenPresenter>();
        }

        public override void Dispose()
        {
            base.Dispose();
            DOTween.Kill(this.tweenSpin);
            this.View.CoinText.Unsubscribe(this.SignalBus);
        }
    }
}