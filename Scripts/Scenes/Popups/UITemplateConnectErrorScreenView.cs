using System;
using System.Net;
using Cysharp.Threading.Tasks;
using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UITemplate.Scripts.Scenes.Popups
{
    public class UITemplateConnectErrorScreenView : BaseView
    {
        public Button   Reconnect;
        public TMP_Text Message;
        public TMP_Text ButtonText;
        public Image    ConnectErrorImage;
        public Image    ConnectingImage;
    }

    [PopupInfo(nameof(UITemplateConnectErrorScreenView))]
    public class UITemplateConnectErrorPresenter : BasePopupPresenter<UITemplateConnectErrorScreenView>
    {
        private static   double         _time               = 0;
        private static   string         connectingMessage   = "Trying to reconnect...\nPlease wait...";
        private static   string         connectErrorMessage = "Your connection has been lost!\nCheck your internet connection and try again";
        private readonly IScreenManager screenManager;
        public UITemplateConnectErrorPresenter(SignalBus signalBus, IScreenManager screenManager) : base(signalBus) { this.screenManager = screenManager; }

        public override void BindData() { UpdateContent(isConnecting: false); }
        protected override async void OnViewReady()
        {
            base.OnViewReady();
            this.View.Reconnect.onClick.AddListener(this.OnClickReconnect);
        }

        public override void Dispose() { base.Dispose(); }
        private void OnConnectSuccess()
        {
            this.screenManager.CloseCurrentScreen();
            // this.SignalBus.Fire<>(new ContinueSignal());
        }
        private void UpdateImage(bool isConnecting)
        {
            this.View.ConnectingImage.gameObject.SetActive(isConnecting);
            this.View.ConnectErrorImage.gameObject.SetActive(!isConnecting);
        }
        private void UpdateContent(bool isConnecting)
        {
            this.View.Message.text           = isConnecting ? connectingMessage : connectErrorMessage;
            this.View.ButtonText.text        = isConnecting ? "Reconnecting" : "Reconnect";
            this.View.Reconnect.interactable = !isConnecting;
            this.UpdateImage(isConnecting);
        }
        private async void OnClickReconnect()
        {
            _time = Time.realtimeSinceStartup;
            var timeSinceLastConnectCheck = _time - 0.1;
            UpdateContent(isConnecting: true);
            var isConnected = false;
            await UniTask.WaitUntil(() =>
            {
                var intervalTime = Time.realtimeSinceStartup - timeSinceLastConnectCheck;
                if (intervalTime >= 1)
                {
                    UniTask.RunOnThreadPool(() => isConnected = IsConnectedToInternet());
                    timeSinceLastConnectCheck = Time.realtimeSinceStartup;
                }

                return isConnected || Time.realtimeSinceStartup - _time > 5;
            });

            if (isConnected)
            {
                OnConnectSuccess();
                return;
            }

            UpdateContent(isConnecting: false);
        }

        bool IsConnectedToInternet()
        {
            try
            {
                using var client = new MyWebClient();
                using (client.OpenRead("https://www.google.com/"))
                    return true;
            }
            catch
            {
                return false;
            }
        }

        private class MyWebClient : WebClient
        {
            protected override WebRequest GetWebRequest(Uri address)
            {
                var request = base.GetWebRequest(address);
                request.Timeout = 1000;
                return request;
            }
        }
    }
}