namespace TheOneStudio.UITemplate.UITemplate.Scenes.Utils
{
    using DG.Tweening;
    using Sirenix.OdinInspector;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UITemplateLuckyBarView : MonoBehaviour
    {
        [SerializeField]                private Slider   pointer;
        [SerializeField]                private TMP_Text txtCoin;
        [SerializeField] [MinValue(0f)] private float    duration;

        [SerializeField] [MinMaxSlider(0f, 1f, true)]
        private Vector2 sliderRange;

        private Tweener sliderTweener;

        public float Coin { get; private set; }

        public void BindData(float minCoin, float maxCoin, float coinUpdateInterval = 1f, Ease ease = Ease.InOutQuart)
        {
            var min   = this.sliderRange.x;
            var max   = this.sliderRange.y;
            var mid   = (min + max) / 2;
            var range = max - min;
            this.sliderTweener = DOTween.To(
                getter: () => min,
                setter: value =>
                {
                    this.pointer.value = value;
                    var coin = maxCoin - (maxCoin - minCoin) * Mathf.Abs(value - mid) / (range / 2);
                    this.Coin = Mathf.Round(coin / coinUpdateInterval) * coinUpdateInterval;
                    if (this.txtCoin) this.txtCoin.text = $"+{this.Coin:N0}";
                },
                endValue: max,
                duration: this.duration
            ).SetLoops(-1, LoopType.Yoyo).SetEase(ease);
        }

        public void Dispose()
        {
            this.sliderTweener.Kill();
        }
        public void Pause()
        {
            this.sliderTweener.Pause();
        }
        public void Resume()
        {
            this.sliderTweener.Play();
        }
    }
}