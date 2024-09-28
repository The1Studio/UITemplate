namespace TheOneStudio.UITemplate.UITemplate.Scenes.Utils
{
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using GameFoundation.DI;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Popups;
    using UnityEngine;
    using UnityEngine.UI;

    public class UITemplateSettingButtonView : MonoBehaviour
    {
        public UITemplateOnOffButton MusicButton;
        public UITemplateOnOffButton SoundButton;
        public UITemplateOnOffButton VibrateButton;

        [SerializeField] private Button SettingButton;
        [SerializeField] private bool   IsDropdown;

        /// <summary>
        ///     Dropdown animation
        /// </summary>
        [SerializeField]
        private RectTransform BG;

        [SerializeField] private List<RectTransform> ButtonList;

        private bool IsDropped;

        private List<RectTransform>             reverseButtonList;
        private IScreenManager                  screenManager;
        private UITemplateSettingDataController uiTemplateSettingDataController;

        private void Awake()
        {
            var container = this.GetCurrentContainer();
            this.screenManager                   = container.Resolve<IScreenManager>();
            this.uiTemplateSettingDataController = container.Resolve<UITemplateSettingDataController>();

            this.SettingButton.onClick.AddListener(this.OnClick);
            this.MusicButton.Button.onClick.AddListener(this.OnClickMusicButton);
            this.SoundButton.Button.onClick.AddListener(this.OnClickSoundButton);
            this.VibrateButton.Button.onClick.AddListener(this.OnVibrationButton);

            this.reverseButtonList = new(this.ButtonList);
            this.reverseButtonList.Reverse();

            this.InitDropdown();
            this.InitButton();
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
            this.SoundButton.SetOnOff(this.uiTemplateSettingDataController.IsSoundOn);
            this.MusicButton.SetOnOff(this.uiTemplateSettingDataController.IsMusicOn);
            this.VibrateButton.SetOnOff(this.uiTemplateSettingDataController.IsVibrationOn);
        }

        private void InitDropdown()
        {
            this.IsDropped     = false;
            this.BG.localScale = new Vector3(1, 0, 1);
            foreach (var rectTransform in this.ButtonList) rectTransform.localScale = Vector3.zero;
        }

        private async void OnClick()
        {
            if (this.IsDropdown)
            {
                //TODO need to to animation here
                const float duration          = 0.15f;
                const float endValue          = 1f;
                const float droppingTime      = 0.4f;
                const int   millisecondsDelay = 100;

                if (this.IsDropped)
                {
                    this.BG.DOScaleY(0, droppingTime).SetEase(Ease.InBack);
                    foreach (var buttonTransform in this.reverseButtonList)
                    {
                        buttonTransform.DOScale(0, duration).SetEase(Ease.InBack);
                        await UniTask.Delay(millisecondsDelay);
                    }
                }
                else
                {
                    this.InitDropdown();
                    this.BG.DOScaleY(1, droppingTime).SetEase(Ease.OutBack);
                    foreach (var buttonTransform in this.ButtonList)
                    {
                        await UniTask.Delay(millisecondsDelay);
                        buttonTransform.DOScale(endValue, duration).SetEase(Ease.OutBounce);
                    }
                }

                this.IsDropped = !this.IsDropped;
            }
            else
            {
                this.screenManager.OpenScreen<UITemplateSettingPopupPresenter>();
            }
        }
    }
}