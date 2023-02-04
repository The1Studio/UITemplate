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
        private readonly IScreenManager                  screenManager;
        private readonly UITemplateButtonAnimationHelper uiTemplateButtonAnimationHelper;
        private          IDisposable                     spinDisposable;
        private const    float                           glowSpinSpeed = -100f;
        
        public UITemplateWinScreenPresenter(SignalBus signalBus, ILogService logService, IScreenManager screenManager,
            UITemplateButtonAnimationHelper uiTemplateButtonAnimationHelper) : base(signalBus, logService)
        {
            this.screenManager                    = screenManager;
            this.uiTemplateButtonAnimationHelper = uiTemplateButtonAnimationHelper;
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
            this.AnimGlow();
        }

        private void AnimGlow()
        {
            this.spinDisposable = Observable.EveryUpdate().Subscribe(_ => Spin());

            void Spin()
            {
                var transform = this.View.LightGlowImage.transform;
                var zAngle    = transform.eulerAngles.z + Time.deltaTime * glowSpinSpeed;
                transform.eulerAngles = new Vector3(0, 0, zAngle);
            }
        }

        private async void OnClickHome()
        {
            await this.uiTemplateButtonAnimationHelper.AnimationButton(this.View.HomeButton.transform);
            this.screenManager.OpenScreen<UITemplateHomeSimpleScreenPresenter>();
        }
        private async void OnClickReplay()
        {
            await this.uiTemplateButtonAnimationHelper.AnimationButton(this.View.ReplayEndgameButton.transform);
            this.screenManager.OpenScreen<UITemplateHomeSimpleScreenPresenter>();
        }
        private async void OnClickNext()
        {
            await this.uiTemplateButtonAnimationHelper.AnimationButton(this.View.NextEndgameButton.transform);
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