namespace UITemplate.Scripts.Scenes.Play.End
{
    using System;
    using DG.Tweening;
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
        public int StarRate;
        public UITemplateWinScreenModel(int starRate) { this.StarRate = starRate; }
    }

    public class UITemplateWinScreenView : BaseView
    {
        public Button                 HomeButton;
        public Button                 ReplayEndgameButton;
        public Button                 NextEndgameButton;
        public UITemplateCurrencyView CoinText;
        public UITemplateStarRateView StarRateView;
        public Image                  LightGlowImage;
    }

    [ScreenInfo(nameof(UITemplateWinScreenView))]
    public class UITemplateWinScreenPresenter : BaseScreenPresenter<UITemplateWinScreenView, UITemplateWinScreenModel>
    {
        private readonly IScreenManager                screenManager;

        private IDisposable spinDisposable;
        
        public UITemplateWinScreenPresenter(SignalBus signalBus, ILogService logService, IScreenManager screenManager) : base(signalBus, logService)
        {
            this.screenManager        = screenManager;
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
            this.spinDisposable = this.View.LightGlowImage.transform.Spin(-100f);
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