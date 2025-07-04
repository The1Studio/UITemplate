﻿namespace TheOneStudio.UITemplate.UITemplate.Scenes.Popups
{
    using System;
    using Core.AdsServices;
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Signals;
    using ServiceImplementation.Configs.Ads;
    using TheOne.Logging;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using TMPro;
    using UnityEngine;
    using UnityEngine.Scripting;
    using UnityEngine.UI;

    public class NativeOverlayInterModel
    {
        public string       InterPlacement;
        public Action<bool> OnComplete;

        public NativeOverlayInterModel(string interPlacement, Action<bool> onComplete)
        {
            this.InterPlacement = interPlacement;
            this.OnComplete     = onComplete;
        }
    }

    public class NativeOverlayInterPopupView : BaseView
    {
        public Button     BtnClose;
        public GameObject ObjTimer;
        public TMP_Text   TxtTimer;
    }

    [PopupInfo(nameof(NativeOverlayInterPopupView), false, false, true)]
    public class NativeOverlayInterPopupPresenter : BasePopupPresenter<NativeOverlayInterPopupView, NativeOverlayInterModel>
    {
        private readonly UITemplateAdServiceWrapper adServiceWrapper;
        private readonly AdServicesConfig           adServicesConfig;

        [Preserve]
        public NativeOverlayInterPopupPresenter(
            SignalBus                  signalBus,
            ILoggerManager             logger,
            UITemplateAdServiceWrapper adServiceWrapper,
            AdServicesConfig           adServicesConfig
        ) : base(signalBus, logger)
        {
            this.adServiceWrapper = adServiceWrapper;
            this.adServicesConfig = adServicesConfig;
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.View.BtnClose.onClick.AddListener(this.OnClose);
        }

        public override UniTask BindData(NativeOverlayInterModel model)
        {
            this.View.BtnClose.gameObject.SetActive(false);
            this.View.ObjTimer.SetActive(true);
            this.adServiceWrapper.ShowNativeOverlayAd(this.Model.InterPlacement, AdViewPosition.Center);
            this.StartCountDown();
            return UniTask.CompletedTask;
        }

        public override void Dispose()
        {
            base.Dispose();
            this.adServiceWrapper.HideNativeOverlayAd(this.Model.InterPlacement);
            this.View.BtnClose.gameObject.SetActive(false);
        }

        private void StartCountDown()
        {
            this.View.TxtTimer.DOCounter(this.adServicesConfig.NativeInterCountdown, 1, this.adServicesConfig.NativeInterCountdown).SetUpdate(true).SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    this.View.ObjTimer.SetActive(false);
                    this.View.BtnClose.gameObject.SetActive(true);
                });
        }

        private void OnClose()
        {
            if (this.adServicesConfig.NativeInterShowAdsComplete)
            {
                this.adServiceWrapper.ShowInterstitialAd(this.Model.InterPlacement, _ =>
                {
                    this.OnComplete();
                });
            }
            else
            {
                this.OnComplete();
            }
        }

        private async void OnComplete()
        {
            await this.CloseViewAsync();
            this.Model.OnComplete?.Invoke(false);
            this.adServiceWrapper.LastTimeShowNativeOverInterAd = Time.time;
        }
    }
}