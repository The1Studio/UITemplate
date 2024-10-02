namespace TheOneStudio.UITemplate.UITemplate.Scenes.Popups
{
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.Utilities.LogService;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using UnityEngine.Scripting;
    using UnityEngine.UI;

    public class UITemplateBlendButtonSettingPopupView : BaseView
    {
        public Button BtnClose;

        public UITemplateBlendButton BtnMusic;
        public UITemplateBlendButton BtnSound;
        public UITemplateBlendButton BtnVibration;
    }

    [PopupInfo(nameof(UITemplateBlendButtonSettingPopupView), isCloseWhenTapOutside: false)]
    public class UITemplateBlendButtonSettingPopupPresenter : BasePopupPresenter<UITemplateBlendButtonSettingPopupView>
    {
        private readonly UITemplateSettingDataController uiTemplateSettingDataController;

        [Preserve]
        public UITemplateBlendButtonSettingPopupPresenter(
            SignalBus                       signalBus,
            ILogService                     logger,
            UITemplateSettingDataController uiTemplateSettingDataController
        ) : base(signalBus, logger)
        {
            this.uiTemplateSettingDataController = uiTemplateSettingDataController;
        }

        public override UniTask BindData()
        {
            this.View.BtnMusic.Init(this.uiTemplateSettingDataController.IsMusicOn);
            this.View.BtnSound.Init(this.uiTemplateSettingDataController.IsSoundOn);
            this.View.BtnVibration.Init(this.uiTemplateSettingDataController.IsVibrationOn);

            this.View.BtnClose.onClick.AddListener(this.CloseView);

            return UniTask.CompletedTask;
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.View.BtnMusic.Button.onClick.AddListener(this.OnClickMusicButton);
            this.View.BtnSound.Button.onClick.AddListener(this.OnClickSoundButton);
            this.View.BtnVibration.Button.onClick.AddListener(this.OnClickVibrationButton);
        }

        private void OnClickVibrationButton()
        {
            this.uiTemplateSettingDataController.SetVibrationOnOff();
        }

        private void OnClickSoundButton()
        {
            this.uiTemplateSettingDataController.SetSoundOnOff();
        }

        private void OnClickMusicButton()
        {
            this.uiTemplateSettingDataController.SetMusicOnOff();
        }
    }
}