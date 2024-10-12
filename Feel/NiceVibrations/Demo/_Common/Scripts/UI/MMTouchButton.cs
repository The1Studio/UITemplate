// Copyright (c) Meta Platforms, Inc. and affiliates. 

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Lofelt.NiceVibrations
{
    [RequireComponent(typeof(Rect))]
    [RequireComponent(typeof(CanvasGroup))]
    /// <summary>
    /// Add this component to a GUI Image to have it act as a button.
    /// Bind pressed down, pressed continually and released actions to it from the inspector
    /// Handles mouse and multi touch
    /// </summary>
    public class MMTouchButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler, ISubmitHandler
    {
        /// The different possible states for the button :
        /// Off (default idle state), ButtonDown (button pressed for the first time), ButtonPressed (button being pressed), ButtonUp (button being released), Disabled (unclickable but still present on screen)
        /// ButtonDown and ButtonUp will only last one frame, the others will last however long you press them / disable them / do nothing
        public enum ButtonStates
        {
            Off,
            ButtonDown,
            ButtonPressed,
            ButtonUp,
            Disabled,
        }

        [Header("Binding")]
        /// The method(s) to call when the button gets pressed down
        public UnityEvent ButtonPressedFirstTime;

        /// The method(s) to call when the button gets released
        public UnityEvent ButtonReleased;

        /// The method(s) to call while the button is being pressed
        public UnityEvent ButtonPressed;

        [Header("Sprite Swap")] public Sprite DisabledSprite;
        public                         Sprite PressedSprite;
        public                         Sprite HighlightedSprite;

        [Header("Color Changes")] public bool           PressedChangeColor = false;
        public                           Color          PressedColor       = Color.white;
        public                           bool           LerpColor          = true;
        public                           float          LerpColorDuration  = 0.2f;
        public                           AnimationCurve LerpColorCurve;

        [Header("Opacity")]
        /// the new opacity to apply to the canvas group when the button is pressed
        public float PressedOpacity = 1f;

        public float IdleOpacity     = 1f;
        public float DisabledOpacity = 1f;

        [Header("Delays")] public float PressedFirstTimeDelay = 0f;
        public                    float ReleasedDelay         = 0f;

        [Header("Buffer")] public float BufferDuration = 0f;

        [Header("Animation")] public Animator Animator;
        public                       string   IdleAnimationParameterName     = "Idle";
        public                       string   DisabledAnimationParameterName = "Disabled";
        public                       string   PressedAnimationParameterName  = "Pressed";

        [Header("Mouse Mode")]
        /// If you set this to true, you'll need to actually press the button for it to be triggered, otherwise a simple hover will trigger it (better for touch input).
        public bool MouseMode = false;

        public bool ReturnToInitialSpriteAutomatically { get; set; }

        /// the current state of the button (off, down, pressed or up)
        public ButtonStates CurrentState { get; protected set; }

        protected bool        _zonePressed = false;
        protected CanvasGroup _canvasGroup;
        protected float       _initialOpacity;
        protected Animator    _animator;
        protected Image       _image;
        protected Sprite      _initialSprite;
        protected Color       _initialColor;
        protected float       _lastClickTimestamp = 0f;
        protected Selectable  _selectable;
        protected float       _lastStateChangeAt = -50f;

        protected Color _imageColor;
        protected Color _fromColor;
        protected Color _toColor;

        /// <summary>
        /// On Start, we get our canvasgroup and set our initial alpha
        /// </summary>
        protected virtual void Awake()
        {
            this.Initialization();
        }

        protected virtual void Initialization()
        {
            this.ReturnToInitialSpriteAutomatically = true;

            this._selectable = this.GetComponent<Selectable>();

            this._image = this.GetComponent<Image>();
            if (this._image != null)
            {
                this._initialColor  = this._image.color;
                this._initialSprite = this._image.sprite;
            }

            this._animator = this.GetComponent<Animator>();
            if (this.Animator != null) this._animator = this.Animator;

            this._canvasGroup = this.GetComponent<CanvasGroup>();
            if (this._canvasGroup != null)
            {
                this._initialOpacity    = this.IdleOpacity;
                this._canvasGroup.alpha = this._initialOpacity;
                this._initialOpacity    = this._canvasGroup.alpha;
            }
            this.ResetButton();
        }

        /// <summary>
        /// Every frame, if the touch zone is pressed, we trigger the OnPointerPressed method, to detect continuous press
        /// </summary>
        protected virtual void Update()
        {
            switch (this.CurrentState)
            {
                case ButtonStates.Off:
                    this.SetOpacity(this.IdleOpacity);
                    if (this._image != null && this.ReturnToInitialSpriteAutomatically) this._image.sprite = this._initialSprite;
                    if (this._selectable != null)
                    {
                        this._selectable.interactable = true;
                        if (EventSystem.current.currentSelectedGameObject == this.gameObject)
                            if (this.HighlightedSprite != null)
                                this._image.sprite = this.HighlightedSprite;
                    }
                    break;

                case ButtonStates.Disabled:
                    this.SetOpacity(this.DisabledOpacity);
                    if (this._image != null)
                        if (this.DisabledSprite != null)
                            this._image.sprite = this.DisabledSprite;
                    if (this._selectable != null) this._selectable.interactable = false;
                    break;

                case ButtonStates.ButtonDown: break;

                case ButtonStates.ButtonPressed:
                    this.SetOpacity(this.PressedOpacity);
                    this.OnPointerPressed();
                    if (this._image != null)
                    {
                        if (this.PressedSprite != null) this._image.sprite = this.PressedSprite;
                        if (this.PressedChangeColor) this._image.color     = this.PressedColor;
                    }
                    break;

                case ButtonStates.ButtonUp: break;
            }

            if (this._image != null && this.PressedChangeColor)
                if (Time.time - this._lastStateChangeAt < this.LerpColorDuration)
                {
                    var t = this.LerpColorCurve.Evaluate(this.Remap(Time.time - this._lastStateChangeAt, 0f, this.LerpColorDuration, 0f, 1f));
                    this._image.color = Color.Lerp(this._fromColor, this._toColor, t);
                }

            this.UpdateAnimatorStates();
        }

        /// <summary>
        /// At the end of every frame, we change our button's state if needed
        /// </summary>
        protected virtual void LateUpdate()
        {
            if (this.CurrentState == ButtonStates.ButtonUp)
            {
                this._lastStateChangeAt = Time.time;
                this._fromColor         = this.PressedColor;
                this._toColor           = this._initialColor;
                this.CurrentState       = ButtonStates.Off;
            }
            if (this.CurrentState == ButtonStates.ButtonDown)
            {
                this._lastStateChangeAt = Time.time;
                this._fromColor         = this._initialColor;
                this._toColor           = this.PressedColor;
                this.CurrentState       = ButtonStates.ButtonPressed;
            }
        }

        /// <summary>
        /// Triggers the bound pointer down action
        /// </summary>
        public virtual void OnPointerDown(PointerEventData data)
        {
            if (Time.time - this._lastClickTimestamp < this.BufferDuration) return;

            if (this.CurrentState != ButtonStates.Off) return;
            this.CurrentState        = ButtonStates.ButtonDown;
            this._lastClickTimestamp = Time.time;
            if (Time.timeScale != 0 && this.PressedFirstTimeDelay > 0)
                this.Invoke("InvokePressedFirstTime", this.PressedFirstTimeDelay);
            else
                this.ButtonPressedFirstTime.Invoke();
        }

        protected virtual void InvokePressedFirstTime()
        {
            if (this.ButtonPressedFirstTime != null) this.ButtonPressedFirstTime.Invoke();
        }

        /// <summary>
        /// Triggers the bound pointer up action
        /// </summary>
        public virtual void OnPointerUp(PointerEventData data)
        {
            if (this.CurrentState != ButtonStates.ButtonPressed && this.CurrentState != ButtonStates.ButtonDown) return;

            this.CurrentState = ButtonStates.ButtonUp;
            if (Time.timeScale != 0 && this.ReleasedDelay > 0)
                this.Invoke("InvokeReleased", this.ReleasedDelay);
            else
                this.ButtonReleased.Invoke();
        }

        protected virtual void InvokeReleased()
        {
            if (this.ButtonReleased != null) this.ButtonReleased.Invoke();
        }

        /// <summary>
        /// Triggers the bound pointer pressed action
        /// </summary>
        public virtual void OnPointerPressed()
        {
            this.CurrentState = ButtonStates.ButtonPressed;
            if (this.ButtonPressed != null) this.ButtonPressed.Invoke();
        }

        /// <summary>
        /// Resets the button's state and opacity
        /// </summary>
        protected virtual void ResetButton()
        {
            this.SetOpacity(this._initialOpacity);
            this.CurrentState = ButtonStates.Off;
        }

        /// <summary>
        /// Triggers the bound pointer enter action when touch enters zone
        /// </summary>
        public virtual void OnPointerEnter(PointerEventData data)
        {
            if (!this.MouseMode) this.OnPointerDown(data);
        }

        /// <summary>
        /// Triggers the bound pointer exit action when touch is out of zone
        /// </summary>
        public virtual void OnPointerExit(PointerEventData data)
        {
            if (!this.MouseMode) this.OnPointerUp(data);
        }

        /// <summary>
        /// OnEnable, we reset our button state
        /// </summary>
        protected virtual void OnEnable()
        {
            this.ResetButton();
        }

        public virtual void DisableButton()
        {
            this.CurrentState = ButtonStates.Disabled;
        }

        public virtual void EnableButton()
        {
            if (this.CurrentState == ButtonStates.Disabled) this.CurrentState = ButtonStates.Off;
        }

        protected virtual void SetOpacity(float newOpacity)
        {
            if (this._canvasGroup != null) this._canvasGroup.alpha = newOpacity;
        }

        protected virtual void UpdateAnimatorStates()
        {
            if (this._animator == null) return;
            if (this.DisabledAnimationParameterName != null) this._animator.SetBool(this.DisabledAnimationParameterName, this.CurrentState == ButtonStates.Disabled);
            if (this.PressedAnimationParameterName != null) this._animator.SetBool(this.PressedAnimationParameterName, this.CurrentState == ButtonStates.ButtonPressed);
            if (this.IdleAnimationParameterName != null) this._animator.SetBool(this.IdleAnimationParameterName, this.CurrentState == ButtonStates.Off);
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
            if (this.ButtonPressedFirstTime != null) this.ButtonPressedFirstTime.Invoke();
            if (this.ButtonReleased != null) this.ButtonReleased.Invoke();
        }

        protected virtual float Remap(float x, float A, float B, float C, float D)
        {
            var remappedValue = C + (x - A) / (B - A) * (D - C);
            return remappedValue;
        }
    }
}