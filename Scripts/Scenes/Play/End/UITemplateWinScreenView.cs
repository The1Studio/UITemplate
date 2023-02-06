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
        public int    ItemUnlockPercent;
        public UITemplateWinScreenModel(int starRate, string itemId, int itemUnlockPercent)
        {
            this.StarRate          = starRate;
            this.ItemId            = itemId;
            this.ItemUnlockPercent = itemUnlockPercent;
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
        private readonly IScreenManager screenManager;
        private readonly IGameAssets    gameAssets;

        private IDisposable spinDisposable;
        
        public UITemplateWinScreenPresenter(SignalBus signalBus, ILogService logService, IScreenManager screenManager, IGameAssets gameAssets) : base(signalBus, logService)
        {
            this.screenManager = screenManager;
            this.gameAssets   = gameAssets;
        }

        protected override async void OnViewReady()
        {
            base.OnViewReady();
            await this.OpenViewAsync();
            this.View.HomeButton.onClick.AddListener(this.OnClickHome);
            this.View.ReplayEndgameButton.onClick.AddListener(this.OnClickReplay);
            this.View.NextEndgameButton.onClick.AddListener(this.OnClickNext);
        }

        public override void BindData(UITemplateWinScreenModel model)
        {
            this.View.CoinText.Subscribe(this.SignalBus);
            this.View.StarRateView.SetStarRate(this.Model.StarRate);
            this.ItemUnlockProgress(50);
            this.spinDisposable = this.View.LightGlowImage.transform.Spin(-80f);
        }

        private async void ItemUnlockProgress(int percent)
        {
            var spriteItemUnlock = await this.gameAssets.LoadAssetAsync<Sprite>(this.Model.ItemId);
            this.View.ItemUnlockImage.sprite = spriteItemUnlock;
            this.View.ItemUnlockBgImage.sprite = spriteItemUnlock;
            
            var value = (float)(percent / 100);
            this.View.ItemUnlockImage.fillAmount = value;
        }

        private void OnClickHome()
        {
            this.screenManager.OpenScreen<UITemplateHomeSimpleScreenPresenter>();
        }
        private void OnClickReplay()
        {
            this.screenManager.OpenScreen<UITemplateHomeSimpleScreenPresenter>();
        }
        private void OnClickNext()
        {
            this.screenManager.OpenScreen<UITemplateHomeSimpleScreenPresenter>();
        }

        public override void Dispose()
        {
            base.Dispose();
            this.spinDisposable.Dispose();
            this.View.CoinText.Unsubscribe(this.SignalBus);
        }
    }
}