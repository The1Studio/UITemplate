namespace TheOneStudio.UITemplate.UITemplate.Scenes.Utils
{
    using System;
    using DG.Tweening;
    using UnityEngine;
    using UnityEngine.UI;

    public class UITemplateBlendButton : MonoBehaviour
    {
        public Sprite OnSprite;
        public Sprite OffSprite;
        
        [SerializeField] private Image  handle;
        [SerializeField] private Button button;
        [SerializeField] private Slider slider;
        [SerializeField] private Image  slideBackgroundImage;
        [SerializeField] private Sprite BgOnSprite;
        [SerializeField] private Sprite BgOffSprite;

        [SerializeField] private float  duration = 0.5f;
        public                   Button Button => this.button;

        private void Awake() { this.button.onClick.AddListener(this.OnClick); }

        private void OnClick()
        {
            this.button.interactable = false;
            DOTween.To(
                    () => this.slider.value,
                    x => this.OnValueChanged(this.slider.value = x),
                    this.slider.value > 0.5f ? 0 : 1,
                    this.duration)
                .OnComplete(() => this.button.interactable = true).SetUpdate(true);
        }

        private void OnValueChanged(float arg0)
        {
            var color = this.handle.color;
            this.handle.color  = new(color.r, color.g, color.b, Mathf.Abs(arg0 - 0.5f) + 0.5f);
            this.handle.sprite = arg0 > 0.5f ? this.OnSprite : this.OffSprite;

            this.slideBackgroundImage.color  = new(color.r, color.g, color.b, Mathf.Abs(arg0 - 0.5f) + 0.5f);
            this.slideBackgroundImage.sprite = arg0 > 0.5f ? this.BgOnSprite : this.BgOffSprite;
            this.ChangeColorText(arg0);
        }

        public void Init(bool isOn)
        {
            var color = this.handle.color;
            this.handle.color  = new(color.r, color.g, color.b, 1);
            this.handle.sprite = isOn ? this.OnSprite : this.OffSprite;

            this.slideBackgroundImage.color  = new(color.r, color.g, color.b, 1);
            this.slideBackgroundImage.sprite = isOn ? this.BgOnSprite : this.BgOffSprite;

            this.slider.value = isOn ? 1 : 0;
            this.InitColorText(isOn);
        }

        protected virtual void ChangeColorText(float handleValue) {}
        
        protected virtual void InitColorText(bool isOn) {}
    }
}