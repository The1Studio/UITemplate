namespace UITemplate.Scripts.Scenes.Popups
{
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using UITemplate.Scripts.Models;
    using Zenject;

    public class UITemplateSettingPopupView : BaseView
    {
        public UITemplateOnOffButton SoundButton;
        public UITemplateOnOffButton MusicButton;
        public UITemplateOnOffButton VibrationButton;
    }

    public class UITemplateSettingPresenter : BasePopupPresenter<UITemplateSettingPopupView>
    {
        #region inject

        private readonly UITemplateSettingData uiTemplateSettingData;

        #endregion

        public UITemplateSettingPresenter(SignalBus signalBus, UITemplateSettingData uiTemplateSettingData) : base(signalBus) { this.uiTemplateSettingData = uiTemplateSettingData; }

        protected override void OnViewReady()
        {
            this.View.SoundButton.Button.onClick.AddListener(this.OnClickSoundButton);
            this.View.MusicButton.Button.onClick.AddListener(this.OnClickMusicButton);
            this.View.VibrationButton.Button.onClick.AddListener(this.OnVibrationButton);
        }

        private void OnClickSoundButton()
        {
            this.uiTemplateSettingData.SoundEffectVolume = this.uiTemplateSettingData.SoundEffectVolume == 0 ? 1 : 0;
            this.View.SoundButton.SetOnOff(this.uiTemplateSettingData.SoundEffectVolume == 1);
        }

        private void OnClickMusicButton()
        {
            this.uiTemplateSettingData.MusicVolume = this.uiTemplateSettingData.MusicVolume == 0 ? 1 : 0;
            this.View.MusicButton.SetOnOff(this.uiTemplateSettingData.MusicVolume == 1);
        }

        private void OnVibrationButton()
        {
            this.uiTemplateSettingData.IsVibrationEnable = !this.uiTemplateSettingData.IsVibrationEnable;
            this.View.VibrationButton.SetOnOff(this.uiTemplateSettingData.IsVibrationEnable);
        }

        public override void BindData() { }
    }
}