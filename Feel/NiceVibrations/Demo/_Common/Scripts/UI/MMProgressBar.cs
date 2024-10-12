// Copyright (c) Meta Platforms, Inc. and affiliates. 

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Lofelt.NiceVibrations
{
    /// <summary>
    /// Add this bar to an object and link it to a bar (possibly the same object the script is on), and you'll be able to resize the bar object based on a current value, located between a min and max value.
    /// See the HealthBar.cs script for a use case
    /// </summary>
    public class MMProgressBar : MonoBehaviour
    {
        /// the possible fill modes
        public enum FillModes
        {
            LocalScale,
            FillAmount,
            Width,
            Height,
        }

        /// the possible directions for the fill (for local scale and fill amount only)
        public enum BarDirections
        {
            LeftToRight,
            RightToLeft,
            UpToDown,
            DownToUp,
        }

        /// the possible timescales the bar can work on
        public enum TimeScales
        {
            UnscaledTime,
            Time,
        }

        [Header("General Settings")]
        /// the local scale or fillamount value to reach when the bar is empty
        public float StartValue = 0f;

        /// the local scale or fillamount value to reach when the bar is full
        public float EndValue = 1f;

        /// the direction this bar moves to
        public BarDirections BarDirection = BarDirections.LeftToRight;

        /// the foreground bar's fill mode
        public FillModes FillMode = FillModes.LocalScale;

        /// defines whether the bar will work on scaled or unscaled time (whether or not it'll keep moving if time is slowed down for example)
        public TimeScales TimeScale = TimeScales.UnscaledTime;

        [Header("Foreground Bar Settings")]
        /// whether or not the foreground bar should lerp
        public bool LerpForegroundBar = true;

        /// the speed at which to lerp the foreground bar
        public float LerpForegroundBarSpeed = 15f;

        [Header("Delayed Bar Settings")]
        /// the delay before the delayed bar moves (in seconds)
        public float Delay = 1f;

        /// whether or not the delayed bar's animation should lerp
        public bool LerpDelayedBar = true;

        /// the speed at which to lerp the delayed bar
        public float LerpDelayedBarSpeed = 15f;

        [Header("Bindings")]
        /// optional - the ID of the player associated to this bar
        public string PlayerID;

        /// the delayed bar
        public Transform DelayedBar;

        /// the main, foreground bar
        public Transform ForegroundBar;

        [Header("Bump")]
        /// whether or not the bar should "bump" when changing value
        public bool BumpScaleOnChange = true;

        /// whether or not the bar should bump when its value increases
        public bool BumpOnIncrease = false;

        /// the duration of the bump animation
        public float BumpDuration = 0.2f;

        /// whether or not the bar should flash when bumping
        public bool ChangeColorWhenBumping = true;

        /// the color to apply to the bar when bumping
        public Color BumpColor = Color.white;

        /// the curve to map the bump animation on
        public AnimationCurve BumpAnimationCurve = new(new Keyframe(1, 1), new Keyframe(0.3f, 1.05f), new Keyframe(1, 1));

        /// the curve to map the bump animation color animation on
        public AnimationCurve BumpColorAnimationCurve = new(new Keyframe(0, 0), new Keyframe(0.3f, 1f), new Keyframe(1, 0));

        /// whether or not the bar is bumping right now
        public bool Bumping { get; protected set; }

        [Header("Realtime")]
        /// whether or not this progress bar should update itself every update (if not, you'll have to update it using the UpdateBar method
        public bool AutoUpdating = false;

        /// the current progress of the bar
        [Range(0f, 1f)]
        public float BarProgress;

        protected float   _targetFill;
        protected Vector3 _targetLocalScale = Vector3.one;
        protected float   _newPercent;
        protected float   _lastPercent;
        protected float   _lastUpdateTimestamp;
        protected bool    _bump = false;
        protected Color   _initialColor;
        protected Vector3 _initialScale;
        protected Vector3 _newScale;
        protected Image   _foregroundImage;
        protected Image   _delayedImage;
        protected bool    _initialized;
        protected Vector2 _initialFrontBarSize;

        /// <summary>
        /// On start we store our image component
        /// </summary>
        protected virtual void Start()
        {
            this._initialScale = this.transform.localScale;

            if (this.ForegroundBar != null)
            {
                this._foregroundImage     = this.ForegroundBar.GetComponent<Image>();
                this._initialFrontBarSize = this._foregroundImage.rectTransform.sizeDelta;
            }
            if (this.DelayedBar != null) this._delayedImage = this.DelayedBar.GetComponent<Image>();
            this._initialized = true;
        }

        /// <summary>
        /// On Update we update our bars
        /// </summary>
        protected virtual void Update()
        {
            this.AutoUpdate();
            this.UpdateFrontBar();
            this.UpdateDelayedBar();
        }

        protected virtual void AutoUpdate()
        {
            if (!this.AutoUpdating) return;

            this._newPercent          = this.Remap(this.BarProgress, 0f, 1f, this.StartValue, this.EndValue);
            this._targetFill          = this._newPercent;
            this._lastUpdateTimestamp = this.TimeScale == TimeScales.Time ? Time.time : Time.unscaledTime;
        }

        /// <summary>
        /// Updates the front bar's scale
        /// </summary>
        protected virtual void UpdateFrontBar()
        {
            var currentDeltaTime = this.TimeScale == TimeScales.Time ? Time.deltaTime : Time.unscaledTime;

            if (this.ForegroundBar != null)
                switch (this.FillMode)
                {
                    case FillModes.LocalScale:
                        this._targetLocalScale = Vector3.one;
                        switch (this.BarDirection)
                        {
                            case BarDirections.LeftToRight:
                                this._targetLocalScale.x = this._targetFill;
                                break;
                            case BarDirections.RightToLeft:
                                this._targetLocalScale.x = 1f - this._targetFill;
                                break;
                            case BarDirections.DownToUp:
                                this._targetLocalScale.y = this._targetFill;
                                break;
                            case BarDirections.UpToDown:
                                this._targetLocalScale.y = 1f - this._targetFill;
                                break;
                        }

                        if (this.LerpForegroundBar)
                            this._newScale = Vector3.Lerp(this.ForegroundBar.localScale, this._targetLocalScale, currentDeltaTime * this.LerpForegroundBarSpeed);
                        else
                            this._newScale = this._targetLocalScale;

                        this.ForegroundBar.localScale = this._newScale;
                        break;

                    case FillModes.Width:
                        if (this._foregroundImage == null) return;
                        var newSizeX = this.Remap(this._targetFill, 0f, 1f, 0, this._initialFrontBarSize.x);
                        newSizeX = Mathf.Lerp(this._foregroundImage.rectTransform.sizeDelta.x, newSizeX, currentDeltaTime * this.LerpForegroundBarSpeed);
                        this._foregroundImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newSizeX);
                        break;

                    case FillModes.Height:
                        if (this._foregroundImage == null) return;
                        var newSizeY = this.Remap(this._targetFill, 0f, 1f, 0, this._initialFrontBarSize.y);
                        newSizeY = Mathf.Lerp(this._foregroundImage.rectTransform.sizeDelta.x, newSizeY, currentDeltaTime * this.LerpForegroundBarSpeed);
                        this._foregroundImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newSizeY);
                        break;

                    case FillModes.FillAmount:
                        if (this._foregroundImage == null) return;
                        if (this.LerpForegroundBar)
                            this._foregroundImage.fillAmount = Mathf.Lerp(this._foregroundImage.fillAmount, this._targetFill, currentDeltaTime * this.LerpForegroundBarSpeed);
                        else
                            this._foregroundImage.fillAmount = this._targetFill;
                        break;
                }
        }

        /// <summary>
        /// Updates the delayed bar's scale
        /// </summary>
        protected virtual void UpdateDelayedBar()
        {
            var currentDeltaTime = this.TimeScale == TimeScales.Time ? Time.deltaTime : Time.unscaledDeltaTime;
            var currentTime      = this.TimeScale == TimeScales.Time ? Time.time : Time.unscaledTime;

            if (this.DelayedBar != null)
                if (currentTime - this._lastUpdateTimestamp > this.Delay)
                {
                    if (this.FillMode == FillModes.LocalScale)
                    {
                        this._targetLocalScale = Vector3.one;

                        switch (this.BarDirection)
                        {
                            case BarDirections.LeftToRight:
                                this._targetLocalScale.x = this._targetFill;
                                break;
                            case BarDirections.RightToLeft:
                                this._targetLocalScale.x = 1f - this._targetFill;
                                break;
                            case BarDirections.DownToUp:
                                this._targetLocalScale.y = this._targetFill;
                                break;
                            case BarDirections.UpToDown:
                                this._targetLocalScale.y = 1f - this._targetFill;
                                break;
                        }

                        if (this.LerpDelayedBar)
                            this._newScale = Vector3.Lerp(this.DelayedBar.localScale, this._targetLocalScale, currentDeltaTime * this.LerpDelayedBarSpeed);
                        else
                            this._newScale = this._targetLocalScale;
                        this.DelayedBar.localScale = this._newScale;
                    }

                    if (this.FillMode == FillModes.FillAmount && this._delayedImage != null)
                    {
                        if (this.LerpDelayedBar)
                            this._delayedImage.fillAmount = Mathf.Lerp(this._delayedImage.fillAmount, this._targetFill, currentDeltaTime * this.LerpDelayedBarSpeed);
                        else
                            this._delayedImage.fillAmount = this._targetFill;
                    }
                }
        }

        /// <summary>
        /// Updates the bar's values based on the specified parameters
        /// </summary>
        /// <param name="currentValue">Current value.</param>
        /// <param name="minValue">Minimum value.</param>
        /// <param name="maxValue">Max value.</param>
        public virtual void UpdateBar(float currentValue, float minValue, float maxValue)
        {
            this._newPercent = this.Remap(currentValue, minValue, maxValue, this.StartValue, this.EndValue);
            if (this._newPercent != this.BarProgress && !this.Bumping) this.Bump();
            this.BarProgress          = this._newPercent;
            this._targetFill          = this._newPercent;
            this._lastUpdateTimestamp = this.TimeScale == TimeScales.Time ? Time.time : Time.unscaledTime;
            this._lastPercent         = this._newPercent;
        }

        /// <summary>
        /// Triggers a camera bump
        /// </summary>
        public virtual void Bump()
        {
            if (!this.BumpScaleOnChange || !this._initialized) return;
            if (!this.BumpOnIncrease && this._lastPercent < this._newPercent) return;
            if (this.gameObject.activeInHierarchy) this.StartCoroutine(this.BumpCoroutine());
        }

        /// <summary>
        /// A coroutine that (usually quickly) changes the scale of the bar
        /// </summary>
        /// <returns>The coroutine.</returns>
        protected virtual IEnumerator BumpCoroutine()
        {
            var journey          = 0f;
            var currentDeltaTime = this.TimeScale == TimeScales.Time ? Time.deltaTime : Time.unscaledDeltaTime;

            this.Bumping = true;
            if (this._foregroundImage != null) this._initialColor = this._foregroundImage.color;

            while (journey <= this.BumpDuration)
            {
                journey = journey + currentDeltaTime;
                var percent           = Mathf.Clamp01(journey / this.BumpDuration);
                var curvePercent      = this.BumpAnimationCurve.Evaluate(percent);
                var colorCurvePercent = this.BumpColorAnimationCurve.Evaluate(percent);
                this.transform.localScale = curvePercent * this._initialScale;

                if (this.ChangeColorWhenBumping && this._foregroundImage != null) this._foregroundImage.color = Color.Lerp(this._initialColor, this.BumpColor, colorCurvePercent);

                yield return null;
            }
            this._foregroundImage.color = this._initialColor;
            this.Bumping                = false;
            yield return null;
        }

        protected virtual float Remap(float x, float A, float B, float C, float D)
        {
            var remappedValue = C + (x - A) / (B - A) * (D - C);
            return remappedValue;
        }
    }
}