// Copyright (c) Meta Platforms, Inc. and affiliates. 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Lofelt.NiceVibrations
{
    public class WobbleButton : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
    {
        public RenderMode ParentCanvasRenderMode { get; protected set; }

        [Header("Bindings")] public Camera      TargetCamera;
        public                      AudioSource SpringAudioSource;
        public                      Animator    TargetAnimator;

        [Header("Haptics")] public HapticSource SpringHapticSource;

        [Header("Colors")] public Image TargetModel;

        [Header("Wobble")] public float          OffDuration = 0.1f;
        public                    float          MaxRange;
        public                    AnimationCurve WobbleCurve;
        public                    float          DragResetDuration = 4f;
        public                    float          WobbleFactor      = 2f;

        protected Vector3          _neutralPosition;
        protected Canvas           _canvas;
        protected Vector3          _newTargetPosition;
        protected Vector3          _eventPosition;
        protected Vector2          _workPosition;
        protected float            _initialZPosition;
        protected bool             _dragging;
        protected int              _pointerID;
        protected PointerEventData _pointerEventData;
        protected RectTransform    _rectTransform;

        protected Vector3 _dragEndedPosition;
        protected float   _dragEndedAt;
        protected Vector3 _dragResetDirection;
        protected bool    _pointerOn   = false;
        protected bool    _draggedOnce = false;
        protected int     _sparkAnimationParameter;

        protected long[] _wobbleAndroidPattern   = { 0, 40, 40, 80 };
        protected int[]  _wobbleAndroidAmplitude = { 0, 40, 0, 80 };

        protected virtual void Start()
        {
        }

        public virtual void SetPitch(float newPitch)
        {
            this.SpringAudioSource.pitch           = newPitch;
            this.SpringHapticSource.frequencyShift = NiceVibrationsDemoHelpers.Remap(newPitch, 0.3f, 1f, -1.0f, 1.0f);
        }

        public virtual void Initialization()
        {
            this._sparkAnimationParameter = Animator.StringToHash("Spark");
            this.ParentCanvasRenderMode   = this.GetComponentInParent<Canvas>().renderMode;
            this._canvas                  = this.GetComponentInParent<Canvas>();
            this._initialZPosition        = this.transform.position.z;
            this._rectTransform           = this.gameObject.GetComponent<RectTransform>();
            this.SetNeutralPosition();
        }

        public virtual void SetNeutralPosition()
        {
            this._neutralPosition = this._rectTransform.transform.position;
        }

        protected virtual Vector3 GetWorldPosition(Vector3 testPosition)
        {
            if (this.ParentCanvasRenderMode == RenderMode.ScreenSpaceCamera)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(this._canvas.transform as RectTransform, testPosition, this._canvas.worldCamera, out this._workPosition);
                return this._canvas.transform.TransformPoint(this._workPosition);
            }
            else
            {
                return testPosition;
            }
        }

        protected virtual void Update()
        {
            if (this._pointerOn && !this._dragging)
            {
                this._newTargetPosition = this.GetWorldPosition(this._pointerEventData.position);

                var distance = (this._newTargetPosition - this._neutralPosition).magnitude;

                if (distance < this.MaxRange)
                    this._dragging = true;
                else
                    this._dragging = false;
            }

            if (this._dragging)
                this.StickToPointer();
            else
                this.GoBackToInitialPosition();
        }

        protected virtual void StickToPointer()
        {
            this._draggedOnce   = true;
            this._eventPosition = this._pointerEventData.position;

            this._newTargetPosition = this.GetWorldPosition(this._eventPosition);

            // We clamp the stick's position to let it move only inside its defined max range
            this._newTargetPosition   = Vector2.ClampMagnitude(this._newTargetPosition - this._neutralPosition, this.MaxRange);
            this._newTargetPosition   = this._neutralPosition + this._newTargetPosition;
            this._newTargetPosition.z = this._initialZPosition;

            this.transform.position = this._newTargetPosition;
        }

        protected virtual void GoBackToInitialPosition()
        {
            if (!this._draggedOnce) return;

            if (Time.time - this._dragEndedAt < this.DragResetDuration)
            {
                var time  = this.Remap(Time.time - this._dragEndedAt, 0f, this.DragResetDuration, 0f, 1f);
                var value = this.WobbleCurve.Evaluate(time) * this.WobbleFactor;
                this._newTargetPosition   = Vector3.LerpUnclamped(this._neutralPosition, this._dragEndedPosition, value);
                this._newTargetPosition.z = this._initialZPosition;
            }
            else
            {
                this._newTargetPosition   = this._neutralPosition;
                this._newTargetPosition.z = this._initialZPosition;
            }
            this.transform.position = this._newTargetPosition;
        }

        public virtual void OnPointerEnter(PointerEventData data)
        {
            this._pointerID        = data.pointerId;
            this._pointerEventData = data;
            this._pointerOn        = true;
        }

        public virtual void OnPointerExit(PointerEventData data)
        {
            this._eventPosition = this._pointerEventData.position;

            this._newTargetPosition   = this.GetWorldPosition(this._eventPosition);
            this._newTargetPosition   = Vector2.ClampMagnitude(this._newTargetPosition - this._neutralPosition, this.MaxRange);
            this._newTargetPosition   = this._neutralPosition + this._newTargetPosition;
            this._newTargetPosition.z = this._initialZPosition;

            this._dragging           = false;
            this._dragEndedPosition  = this._newTargetPosition;
            this._dragEndedAt        = Time.time;
            this._dragResetDirection = this._dragEndedPosition - this._neutralPosition;
            this._pointerOn          = false;

            this.TargetAnimator.SetTrigger(this._sparkAnimationParameter);
            this.SpringAudioSource.Play();
            this.SpringHapticSource.Play();
        }

        protected virtual float Remap(float x, float A, float B, float C, float D)
        {
            var remappedValue = C + (x - A) / (B - A) * (D - C);
            return remappedValue;
        }
    }
}