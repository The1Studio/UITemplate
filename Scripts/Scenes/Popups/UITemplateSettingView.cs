namespace TheOneStudio.UITemplate.UITemplate.Scenes.Popups
{
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
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
    public class UITemplateSettingPopupPresenter : UITemplateBasePopupPresenter<UITemplateSettingPopupView>
    {
        #region inject

        private readonly UITemplateSettingDataController uiTemplateSettingDataController;

        #endregion

        public UITemplateSettingPopupPresenter(SignalBus signalBus, UITemplateSettingDataController uiTemplateSettingDataController) : base(signalBus)
        {
            this.uiTemplateSettingDataController = uiTemplateSettingDataController;
        }

        protected override void OnViewReady()
        {
            this.View.SoundButton.Button.onClick.AddListener(this.OnClickSoundButton);
            this.View.MusicButton.Button.onClick.AddListener(this.OnClickMusicButton);
            this.View.VibrationButton.Button.onClick.AddListener(this.OnVibrationButton);
        }

        private void OnClickSoundButton()
        {
            this.uiTemplateSettingDataController.SetSoundOnOff();
            this.InitButton();
        }

        private void OnClickMusicButton()
        {
            this.uiTemplateSettingDataController.SetMusicOnOff();
            this.InitButton();
        }

        private void OnVibrationButton()
        {
            this.uiTemplateSettingDataController.SetVibrationOnOff();
            this.InitButton();
        }

        private void InitButton()
        {
            this.View.SoundButton.SetOnOff(this.uiTemplateSettingDataController.IsSoundOn);
            this.View.MusicButton.SetOnOff(this.uiTemplateSettingDataController.IsMusicOn);
            this.View.VibrationButton.SetOnOff(this.uiTemplateSettingDataController.IsVibrationOn);
        }

        public override void BindData()
        {
            this.InitButton();
        }
    }
}