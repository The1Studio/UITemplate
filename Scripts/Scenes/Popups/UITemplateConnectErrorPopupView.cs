namespace TheOneStudio.UITemplate.UITemplate.Scenes.Popups
{
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateConnectErrorPopupView : BaseView
    {
        public Button   Reconnect;
        public TMP_Text Message;
        public TMP_Text ButtonText;
        public Image    ConnectErrorImage;
        public Image    ConnectingImage;
    }

    [PopupInfo(nameof(UITemplateConnectErrorPopupView), true, false)]
    public class UITemplateConnectErrorPresenter : UITemplateBasePopupPresenter<UITemplateConnectErrorPopupView>
    {
        private static readonly double checkTimeout        = 5;
        private static readonly string connectingMessage   = "Trying to reconnect...\nPlease wait...";
        private static readonly string connectErrorMessage = "Your connection has been lost!\nCheck your internet connection and try again";

        public UITemplateConnectErrorPresenter(SignalBus signalBus, IScreenManager screenManager, IInternetService internetService) : base(signalBus)
        {
            this.screenManager   = screenManager;
            this.internetService = internetService;
        }

        public override UniTask BindData()
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
            this.CloseView();
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
            var time                      = Time.realtimeSinceStartup;
            var timeSinceLastConnectCheck = time - 0.1;
            this.UpdateContent(true);
            var isConnected = false;
            await UniTask.WaitUntil(() =>
            {
                var intervalTime = Time.realtimeSinceStartup - timeSinceLastConnectCheck;
                if (intervalTime >= 1)
                {
                    UniTask.RunOnThreadPool(() => isConnected = this.internetService.IsInternetAvailable);
                    timeSinceLastConnectCheck = Time.realtimeSinceStartup;
                }

                return isConnected || Time.realtimeSinceStartup - time > checkTimeout;
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