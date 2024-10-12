// Copyright (c) Meta Platforms, Inc. and affiliates. 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Lofelt.NiceVibrations
{
    [RequireComponent(typeof(Rect))]
    public class MMKnob : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public RenderMode ParentCanvasRenderMode { get; protected set; }

        [Header("Bindings")] public Camera TargetCamera;

        [Header("Settings")] public float MinimumAngle    = 45f;
        public                      float MaximumAngle    = -225f;
        public                      float MaximumDistance = 50f;
        public                      Color ActiveColor;
        public                      Color InactiveColor;

        [Header("Output")] public bool  Dragging = false;
        public                    float Value    = 0f;
        public                    bool  Active   = true;

        public    Image            _image;
        protected PointerEventData _pointerEventData;
        protected float            _distance;
        public    RectTransform    _rectTransform;
        protected Vector3          _rotation = Vector3.zero;
        protected Canvas           _canvas;
        protected Vector2          _workPosition;

        protected virtual void Awake()
        {
            this._image                 = this.gameObject.GetComponent<Image>();
            this._canvas                = this.GetComponentInParent<Canvas>();
            this.ParentCanvasRenderMode = this.GetComponentInParent<Canvas>().renderMode;
            this._rectTransform         = this.GetComponent<RectTransform>();
            this.SetRotation(this.MinimumAngle);
        }

        protected virtual void Update()
        {
            if (!this.Active)
            {
                this.Dragging     = false;
                this._image.color = this.InactiveColor;
                return;
            }
            else
            {
                this._image.color = this.ActiveColor;
            }

            if (!this.Dragging) return;

            var     v1 = Vector2.down;
            Vector2 v2 = this.transform.position - this.GetWorldPosition(this._pointerEventData.position);

            var angle = Vector2.SignedAngle(v1, v2);

            angle = Mathf.Clamp(angle, -130f, 130f);

            this._rotation.z = NiceVibrationsDemoHelpers.Remap(angle, -130f, 130f, this.MaximumAngle, this.MinimumAngle);
            this._rectTransform.SetPositionAndRotation(this.transform.position, Quaternion.Euler(this._rotation));

            this.Value = NiceVibrationsDemoHelpers.Remap(angle, -130f, 130f, 1f, 0f);
        }

        protected virtual void SetRotation(float angle)
        {
            angle            = Mathf.Clamp(angle, this.MaximumAngle, this.MinimumAngle);
            this._rotation.z = angle;
            this._rectTransform.SetPositionAndRotation(this.transform.position, Quaternion.Euler(this._rotation));
        }

        public virtual void SetActive(bool status)
        {
            this.Active = status;
        }

        public virtual void SetValue(float value)
        {
            this.SetRotation(this.MinimumAngle);
            this.Value = value;
            var angle = NiceVibrationsDemoHelpers.Remap(value, 0f, 1f, this.MinimumAngle, this.MaximumAngle);

            this._rotation.z = angle;
            this._rectTransform.SetPositionAndRotation(this.transform.position, Quaternion.Euler(this._rotation));
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            this._pointerEventData = eventData;
            this.Dragging          = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            this._pointerEventData = null;
            this.Dragging          = false;
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
    }
}