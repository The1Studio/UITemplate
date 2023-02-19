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

        private readonly UITemplateSettingData uiTemplateSettingData;

        #endregion

        public UITemplateSettingPopupPresenter(SignalBus signalBus, UITemplateSettingData uiTemplateSettingData) : base(signalBus) { this.uiTemplateSettingData = uiTemplateSettingData; }

        protected override void OnViewReady()
        {
            this.View.SoundButton.Button.onClick.AddListener(this.OnClickSoundButton);
            this.View.MusicButton.Button.onClick.AddListener(this.OnClickMusicButton);
            this.View.VibrationButton.Button.onClick.AddListener(this.OnVibrationButton);
        }

        private void OnClickSoundButton()
        {
            this.uiTemplateSettingData.SetSoundOnOff();
            this.InitButton();
        }

        private void OnClickMusicButton()
        {
            this.uiTemplateSettingData.SetMusicOnOff();
            this.InitButton();
        }

        private void OnVibrationButton()
        {
            this.uiTemplateSettingData.IsVibrationEnable = !this.uiTemplateSettingData.IsVibrationEnable;
            this.InitButton();
        }

        private void InitButton()
        {
            this.View.SoundButton.SetOnOff(this.uiTemplateSettingData.IsSoundOn);
            this.View.MusicButton.SetOnOff(this.uiTemplateSettingData.IsMusicOn);
            this.View.VibrationButton.SetOnOff(this.uiTemplateSettingData.IsVibrationEnable);
        }

        public override void BindData()
        {
            this.InitButton();
        }
    }
}