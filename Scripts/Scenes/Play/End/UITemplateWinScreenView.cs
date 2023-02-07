namespace UITemplate.Scripts.Scenes.Play.End
{
    using System;
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.Utilities.LogService;
    using UITemplate.Scripts.Models;
    using UITemplate.Scripts.Scenes.Main;
    using UITemplate.Scripts.Scenes.Popups;
    using UITemplate.Scripts.Scenes.Utils;
    using UniRx;
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
        private readonly IScreenManager     screenManager;
        private readonly IGameAssets        gameAssets;
        private readonly UITemplateUserData userData;

        private IDisposable spinDisposable;

        public UITemplateWinScreenPresenter(SignalBus signalBus, ILogService logService, IScreenManager screenManager, IGameAssets gameAssets,
            UITemplateUserData userData) : base(signalBus, logService)
        {
            this.screenManager = screenManager;
            this.gameAssets    = gameAssets;
            this.userData      = userData;
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
            this.View.CoinText.Subscribe(this.SignalBus);
            this.ItemUnlockProgress(model.ItemUnlockLastPercent, model.ItemUnlockPercent);
            this.spinDisposable = this.View.LightGlowImage.transform.Spin(-80f);
            await this.View.StarRateView.SetStarRate(model.StarRate);
        }

        private async void ItemUnlockProgress(float lastPercent, float percent)
        {
            var spriteItemUnlock = await this.gameAssets.LoadAssetAsync<Sprite>(this.Model.ItemId);
            this.View.ItemUnlockImage.sprite   = spriteItemUnlock;
            this.View.ItemUnlockBgImage.sprite = spriteItemUnlock;
            float lastValue = lastPercent / 100f;
            float value     = percent / 100f;
            DOTween.To(() => this.View.ItemUnlockImage.fillAmount = lastValue, x => this.View.ItemUnlockImage.fillAmount = x, value, 1f)
                .SetEase(Ease.Linear);

            var itemStatus = Math.Abs(value - 1) < Mathf.Epsilon ? ItemData.Status.Owned : ItemData.Status.InProgress;
            this.userData.ShopData.UpdateStatusItemData(this.Model.ItemId, itemStatus);
        }

        private void OnClickHome() { this.screenManager.OpenScreen<UITemplateHomeSimpleScreenPresenter>(); }

        private void OnClickReplay() { this.screenManager.OpenScreen<UITemplateHomeSimpleScreenPresenter>(); }

        private void OnClickNext() { this.screenManager.OpenScreen<UITemplateHomeSimpleScreenPresenter>(); }

        public override void Dispose()
        {
            base.Dispose();
            this.spinDisposable.Dispose();
            this.View.CoinText.Unsubscribe(this.SignalBus);
        }
    }
}