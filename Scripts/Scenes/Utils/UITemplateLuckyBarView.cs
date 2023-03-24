namespace TheOneStudio.UITemplate.UITemplate.Scenes.Utils
{
    using DG.Tweening;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UITemplateLuckyBarView : MonoBehaviour
    {
        [SerializeField] private Slider   pointer;
        [SerializeField] private TMP_Text txtCoin;

        private Tweener sliderTweener;

        public float Coin { get; private set; }

        public void BindData(float minCoin, float maxCoin)
        {
            const float min   = .05f;
            const float max   = .95f;
            const float mid   = (min + max) / 2;
            const float range = max - min;
            this.sliderTweener = DOTween.To(
                getter: () => min,
                setter: value =>
                {
                    this.pointer.value = value;
                    this.Coin          = maxCoin - (maxCoin - minCoin) * Mathf.Abs(value - mid) / (range / 2);
                    this.txtCoin.text  = $"+{this.Coin:N0}";
                },
                endValue: max,
                duration: 1f
            ).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuart);
        }

        public void Dispose()
        {
            this.sliderTweener.Kill();
        }
    }
}