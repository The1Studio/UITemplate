namespace TheOneStudio.HyperCasual.Runtime.Common
{
    using System.Collections.Generic;
    using DG.Tweening;
    using UnityEngine;
    using UnityEngine.UI;

    [ExecuteInEditMode]
    public class UITemplateChoiceSlider : MonoBehaviour
    {
        [SerializeField]
        private Slider slider;

        [SerializeField]
        private HorizontalLayoutGroup layoutGroup;

        [SerializeField]
        private float speed = 1f;

        private void Start()
        {
            // tween the slider from 0 to 1 and back to 0 looping forever
            DOTween.To(() => this.slider.value, value => this.slider.value = value, 1f, this.speed)
                   .SetEase(Ease.Linear)
                   .SetLoops(-1, LoopType.Yoyo);
        }

#if UNITY_EDITOR
        private void LateUpdate()
        {
            var rect = this.slider.GetComponent<RectTransform>();
            rect.offsetMin = new Vector2(this.layoutGroup.padding.left, rect.offsetMin.y);
            rect.offsetMax = new Vector2(-this.layoutGroup.padding.right, rect.offsetMax.y);
        }
#endif

        private int GetCurrentChoiceIndex()
        {
            var choiceTransforms = new List<RectTransform>();

            var totalWidth = 0f;

            foreach (RectTransform rectTransform in this.layoutGroup.transform)
            {
                if (rectTransform.GetComponent<LayoutElement>() is { ignoreLayout: true }) continue;
                totalWidth += rectTransform.rect.width + (int)this.layoutGroup.spacing;
                choiceTransforms.Add(rectTransform);
            }

            if (totalWidth == 0)
            {
                return -1;
            }

            totalWidth -= this.layoutGroup.spacing;

            var currentWidth = 0f;
            for (var i = 0; i < choiceTransforms.Count; i++)
            {
                currentWidth += choiceTransforms[i].rect.width + this.layoutGroup.spacing;

                if (currentWidth >= totalWidth * this.slider.value)
                {
                    return i;
                }
            }

            return -1;
        }

        public int ChoiceIndex => this.GetCurrentChoiceIndex();
    }
}