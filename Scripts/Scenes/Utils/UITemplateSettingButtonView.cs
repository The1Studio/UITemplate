namespace TheOneStudio.UITemplate.UITemplate.Scenes.Utils
{
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Popups;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateSettingButtonView : MonoBehaviour
    {
        private IScreenManager        screenManager;
        private UITemplateSettingData uiTemplateSettingData;

        public UITemplateOnOffButton MusicButton;
        public UITemplateOnOffButton SoundButton;
        public UITemplateOnOffButton VibrateButton;

        [SerializeField] private Button SettingButton;
        [SerializeField] private bool   IsDropdown;

        private bool IsDropped;

        /// <summary>
        /// Dropdown animation
        /// </summary>
        [SerializeField] private RectTransform BG;

        [SerializeField] private List<RectTransform> ButtonList;

        private List<RectTransform> ReverseButtonList;

        private void Awake()
        {
            this.SettingButton.onClick.AddListener(this.OnClick);
            this.ReverseButtonList = new List<RectTransform>(this.ButtonList);
            this.ReverseButtonList.Reverse();
            this.MusicButton.Button.onClick.AddListener(this.OnClickMusicButton);
            this.SoundButton.Button.onClick.AddListener(this.OnClickSoundButton);
            this.VibrateButton.Button.onClick.AddListener(this.OnVibrationButton);
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

        [Inject]
        public void Init(IScreenManager screenManager, UITemplateSettingData uiTemplateSettingData)
        {
            this.screenManager         = screenManager;
            this.uiTemplateSettingData = uiTemplateSettingData;
            this.InitDropdown();
            this.InitButton();
        }

        private void InitButton()
        {
            this.SoundButton.SetOnOff(this.uiTemplateSettingData.IsSoundOn);
            this.MusicButton.SetOnOff(this.uiTemplateSettingData.IsMusicOn);
            this.VibrateButton.SetOnOff(this.uiTemplateSettingData.IsVibrationEnable);
        }

        private void InitDropdown()
        {
            this.IsDropped     = false;
            this.BG.localScale = new Vector3(1, 0, 1);
            foreach (var rectTransform in this.ButtonList)
            {
                rectTransform.localScale = Vector3.zero;
            }
        }

        private async void OnClick()
        {
            if (this.IsDropdown)
            {
                //TODO need to to animation here
                const float duration          = 0.15f;
                const float endValue          = 0.47f;
                const float droppingTime      = 0.4f;
                const int   millisecondsDelay = 100;

                if (this.IsDropped)
                {
                    this.BG.DOScaleY(0, droppingTime).SetEase(Ease.InBack);
                    var reverseList = new List<RectTransform>(this.ButtonList);
                    foreach (var buttonTransform in this.ReverseButtonList)
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