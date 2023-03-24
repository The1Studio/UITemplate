namespace TheOneStudio.HyperCasual.Runtime.Common
{
    using System;
    using System.Collections.Generic;
    using DG.Tweening;
    using DG.Tweening.Core;
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

        private int     lastChoiceIndex = -1;
        private Tweener tween;

        public Action<int> OnChoiceChanged { get; set; }

        private void Start()
        {
            // tween the slider from 0 to 1 and back to 0 looping forever
            this.tween = DOTween.To(() => this.slider.value, value => this.slider.value = value, 1f, this.speed)
                                .SetEase(Ease.Linear)
                                .SetLoops(-1, LoopType.Yoyo);
        }

        private void LateUpdate()
        {
#if UNITY_EDITOR
            var rect = this.slider.GetComponent<RectTransform>();
            rect.offsetMin = new Vector2(this.layoutGroup.padding.left, rect.offsetMin.y);
            rect.offsetMax = new Vector2(-this.layoutGroup.padding.right, rect.offsetMax.y);
#endif
            // Call OnChoiceChanged event when the slider value changes
            if (this.OnChoiceChanged is not null)
            {
                var currentChoiceIndex = this.GetCurrentChoiceIndex();

                if (currentChoiceIndex != this.lastChoiceIndex)
                {
                    this.OnChoiceChanged(currentChoiceIndex);
                    this.lastChoiceIndex = currentChoiceIndex;
                }
            }
        }

        public void StartSlider()
        {
            this.tween.Play();
        }

        public void StopSlider()
        {
            this.tween.Pause();
        }

        public void RestartSlider()
        {
            this.tween.Restart();
        }

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