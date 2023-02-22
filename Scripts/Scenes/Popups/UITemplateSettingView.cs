namespace TheOneStudio.UITemplate.UITemplate.Scenes.Popups
{
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateSettingPopupView : BaseView
    {
        public UITemplateOnOffButton SoundButton;
        public UITemplateOnOffButton MusicButton;
        public UITemplateOnOffButton VibrationButton;

        public Button ReplayButton;
        public Button HomeButton;
    }

    [PopupInfo(nameof(UITemplateSettingPopupView))]
    public class UITemplateSettingPopupPresenter : BasePopupPresenter<UITemplateSettingPopupView>
    {
        #region inject

        private readonly UITemplateUserSettingData uiTemplateUserSettingData;

        #endregion

        public UITemplateSettingPopupPresenter(SignalBus signalBus, UITemplateUserSettingData uiTemplateUserSettingData) : base(signalBus) { this.uiTemplateUserSettingData = uiTemplateUserSettingData; }

        protected override void OnViewReady()
        {
            this.View.SoundButton.Button.onClick.AddListener(this.OnClickSoundButton);
            this.View.MusicButton.Button.onClick.AddListener(this.OnClickMusicButton);
            this.View.VibrationButton.Button.onClick.AddListener(this.OnVibrationButton);
        }

        private void OnClickSoundButton()
        {
            this.uiTemplateUserSettingData.SetSoundOnOff();
            this.InitButton();
        }

        private void OnClickMusicButton()
        {
            this.uiTemplateUserSettingData.SetMusicOnOff();
            this.InitButton();
        }

        private void OnVibrationButton()
        {
            this.uiTemplateUserSettingData.IsVibrationEnable = !this.uiTemplateUserSettingData.IsVibrationEnable;
            this.InitButton();
        }

        private void InitButton()
        {
            this.View.SoundButton.SetOnOff(this.uiTemplateUserSettingData.IsSoundOn);
            this.View.MusicButton.SetOnOff(this.uiTemplateUserSettingData.IsMusicOn);
            this.View.VibrationButton.SetOnOff(this.uiTemplateUserSettingData.IsVibrationEnable);
        }

        public override void BindData()
        {
            this.InitButton();
        }
    }
}