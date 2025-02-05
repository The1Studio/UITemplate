namespace TheOneStudio.UITemplate.UITemplate.Scenes.Popups
{
    using System;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.Utilities.LogService;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using TMPro;
    using UnityEngine;
    using UnityEngine.Scripting;
    using UnityEngine.UI;

    public class UITemplateConnectErrorPopupView : BaseView
    {
        public Button   Reconnect;
        public TMP_Text Message;
        public TMP_Text ButtonText;
        public Image    ConnectErrorImage;
        public Image    ConnectingImage;
    }
    
    public class UITemplateConnectErrorModel
    {
        public Action OnConnectSuccess { get; }
        
        public UITemplateConnectErrorModel(Action onConnectSuccess)
        {
            this.OnConnectSuccess    = onConnectSuccess;
        }
    }

    [PopupInfo(nameof(UITemplateConnectErrorPopupView), true, false, true)]
    public class UITemplateConnectErrorPresenter : UITemplateBasePopupPresenter<UITemplateConnectErrorPopupView, UITemplateConnectErrorModel>
    {
        protected virtual double CheckTimeout        => 5;
        protected virtual string ConnectingMessage   => "Trying to reconnect...\nPlease wait...";
        protected virtual string ConnectErrorMessage => "Your connection has been lost!\nCheck your internet connection and try again";

        protected virtual string ReconnectButtonMessage    => "Reconnect";
        protected virtual string ReconnectingButtonMessage => "Reconnecting";

        [Preserve]
        public UITemplateConnectErrorPresenter(
            SignalBus        signalBus,
            ILogService      logger,
            IScreenManager   screenManager,
            IInternetService internetService
        ) : base(signalBus, logger)
        {
            this.screenManager   = screenManager;
            this.internetService = internetService;
        }

        public override UniTask BindData(UITemplateConnectErrorModel model)
        {
            this.UpdateContent(false);
            return UniTask.CompletedTask;
        }

        protected override async void OnViewReady()
        {
            base.OnViewReady();
            this.View.Reconnect.onClick.AddListener(this.OnClickReconnect);
        }

        protected virtual void OnConnectSuccess()
        {
            this.Model?.OnConnectSuccess?.Invoke();
            this.CloseView();
        }

        private void UpdateImage(bool isConnecting)
        {
            this.View.ConnectingImage.gameObject.SetActive(isConnecting);
            this.View.ConnectErrorImage.gameObject.SetActive(!isConnecting);
        }

        private void UpdateContent(bool isConnecting)
        {
            this.View.Message.text           = isConnecting ? this.ConnectingMessage : this.ConnectErrorMessage;
            this.View.ButtonText.text        = isConnecting ? this.ReconnectingButtonMessage : this.ReconnectButtonMessage;
            this.View.Reconnect.interactable = !isConnecting;
            this.UpdateImage(isConnecting);
        }

        private async void OnClickReconnect()
        {
            var time                      = Time.realtimeSinceStartup;
            var timeSinceLastConnectCheck = time - 0.1;
            this.UpdateContent(true);
            var isConnected = false;
            await UniTask.WaitUntil(() =>
            {
                var intervalTime = Time.realtimeSinceStartup - timeSinceLastConnectCheck;
                if (intervalTime >= 1)
                {
                    isConnected               = this.internetService.IsInternetAvailable;
                    timeSinceLastConnectCheck = Time.realtimeSinceStartup;
                }

                return isConnected || Time.realtimeSinceStartup - time > this.CheckTimeout;
            });

            if (isConnected)
            {
                this.OnConnectSuccess();
                return;
            }

            this.UpdateContent(false);
        }

        #region MyRegion

        private readonly IScreenManager   screenManager;
        private readonly IInternetService internetService;

        #endregion
    }
}