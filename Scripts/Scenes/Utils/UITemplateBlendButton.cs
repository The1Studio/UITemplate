namespace TheOneStudio.UITemplate.UITemplate.Scenes.Utils
{
    using System;
    using DG.Tweening;
    using UnityEngine;
    using UnityEngine.UI;

    public class UITemplateBlendButton : MonoBehaviour
    {
        [SerializeField] private Image  handle;
        [SerializeField] private Button button;
        [SerializeField] private Slider slider;
        [SerializeField] private Image  slideBackgroundImage;

        [SerializeField] private Sprite OnSprite;
        [SerializeField] private Sprite OffSprite;

        [SerializeField] private Sprite BgOnSprite;
        [SerializeField] private Sprite BgOffSprite;

        public Button Button => this.button;

        private void Awake() { this.button.onClick.AddListener(this.OnClick); }

        private void OnClick() { DOTween.To(() => this.slider.value, x => this.OnValueChanged(this.slider.value = x), this.slider.value > 0.5f ? 0 : 1, 0.5f); }

        private void OnValueChanged(float arg0)
        {
            var color = this.handle.color;
            this.handle.color  = new Color(color.r, color.g, color.b, Mathf.Abs(arg0 - 0.5f) + 0.5f);
            this.handle.sprite = arg0 > 0.5f ? this.OnSprite : this.OffSprite;

            this.slideBackgroundImage.color  = new Color(color.r, color.g, color.b, Mathf.Abs(arg0 - 0.5f) + 0.5f);
            this.slideBackgroundImage.sprite = arg0 > 0.5f ? this.BgOnSprite : this.BgOffSprite;
        }

        public void Init(bool isOn)
        {
            var color = this.handle.color;
            this.handle.color  = new Color(color.r, color.g, color.b, 1);
            this.handle.sprite = isOn ? this.OnSprite : this.OffSprite;
            
            this.slideBackgroundImage.color  = new Color(color.r, color.g, color.b, 1);
            this.slideBackgroundImage.sprite = isOn ? this.BgOnSprite : this.BgOffSprite;
            
            this.slider.value = isOn ? 1 : 0;
        }
    }
}