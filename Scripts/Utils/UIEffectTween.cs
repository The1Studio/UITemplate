#if UI_EFFECT
namespace TheOneStudio.UITemplate.UITemplate.Utils
{
    using System;
    using Coffee.UIEffects;
    using DG.Tweening;
    using Sirenix.OdinInspector;
    using UnityEngine;

    public class UIEffectTween : MonoBehaviour
    {
        #region Inspectors

        [ShowIf(nameof(HasUIEffectComponent))] [SerializeField]
        private bool effectFactorShift;

        [ShowIf(nameof(effectFactorShift))] [SerializeField] [BoxGroup("Effect Factor")]
        private Option effectFactorOption = new Option(0f, 1f);

        [ShowIf(nameof(HasUIEffectComponent))] [SerializeField]
        private bool blurFactorShift;

        [ShowIf(nameof(blurFactorShift))] [SerializeField] [BoxGroup ("Blur Factor")]
        private Option blurFactorOption = new Option(0f, 1f);

        [ShowIf(nameof(HasUIHsvModifierComponent))] [SerializeField]
        private bool hueShift;

        [ShowIf(nameof(hueShift))] [SerializeField] [BoxGroup ("Hue")]
        private Option hueOption = new Option(-0.5f, 0.5f);

        [ShowIf(nameof(HasUIHsvModifierComponent))] [SerializeField]
        private bool saturationShift;

        [ShowIf(nameof(saturationShift))] [SerializeField] [BoxGroup ("Saturation")]
        private Option saturationOption = new Option(-0.5f, 0.5f);

        [ShowIf(nameof(HasUIHsvModifierComponent))] [SerializeField]
        private bool valueShift;

        [ShowIf(nameof(valueShift))] [SerializeField] [BoxGroup ("Value")]
        private Option valueOption = new Option(-0.5f, 0.5f);

        #endregion

        private UIEffect      uiEffect;
        private UIHsvModifier uiHsvModifier;

        private bool HasUIEffectComponent()      => this.GetComponent<UIEffect>() != null;
        private bool HasUIHsvModifierComponent() => this.GetComponent<UIHsvModifier>() != null;

        private void Awake()
        {
            this.uiEffect      = this.GetComponent<UIEffect>();
            this.uiHsvModifier = this.GetComponent<UIHsvModifier>();
            
            if (this.uiEffect != null)
            {
                if (this.uiEffect.effectMode != EffectMode.None && this.effectFactorShift)
                {
                    this.AddTween(this.uiEffect, "effectFactor", this.effectFactorOption);
                }

                if (this.uiEffect.blurMode != BlurMode.None && this.blurFactorShift)
                {
                    this.AddTween(this.uiEffect, "blurFactor", this.blurFactorOption);
                }
            }

            if (this.uiHsvModifier != null)
            {
                if (this.hueShift)
                {
                    this.AddTween(this.uiHsvModifier, "hue", this.hueOption);
                }

                if (this.saturationShift)
                {
                    this.AddTween(this.uiHsvModifier, "saturation", this.saturationOption);
                }

                if (this.valueShift)
                {
                    this.AddTween(this.uiHsvModifier, "value", this.valueOption);
                }
            }
        }

        private void AddTween<T>(T target, string propertyName, Option option)
        {
            DOTween.To(() => option.ValueFrom, x => target.GetType().GetProperty(propertyName)?.SetValue(target, x), option.ValueTo, option.Duration)
                .SetEase(option.Ease)
                .SetLoops(option.LoopTimes, option.LoopType)
                .SetUpdate(true);
        }

        [Serializable]
        public class Option
        {
            public float    Duration  = 1f;
            public Ease     Ease      = Ease.Linear;
            public LoopType LoopType  = LoopType.Restart;
            public int      LoopTimes = -1;
            public float    ValueFrom;
            public float    ValueTo;

            public Option(float valueFrom, float valueTo)
            {
                this.ValueFrom = valueFrom;
                this.ValueTo   = valueTo;
            }
        }
    }
}
#endif