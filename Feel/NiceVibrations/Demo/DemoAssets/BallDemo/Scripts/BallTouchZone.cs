// Copyright (c) Meta Platforms, Inc. and affiliates. 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Lofelt.NiceVibrations
{
    public class BallTouchZone : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
    {
        public    RenderMode       ParentCanvasRenderMode { get; protected set; }
        public    RectTransform    BallMover;
        protected bool             _holding = false;
        protected PointerEventData _pointerEventData;
        protected Vector3          _newPosition;
        protected Canvas           _canvas;
        protected float            _initialZPosition;
        protected Vector2          _workPosition;

        protected virtual void Start()
        {
            this.Initialization();
        }

        protected virtual void Initialization()
        {
            this.ParentCanvasRenderMode = this.GetComponentInParent<Canvas>().renderMode;
            this._canvas                = this.GetComponentInParent<Canvas>();
            this._initialZPosition      = this.transform.position.z;
        }

        protected virtual void Update()
        {
            if (this._holding)
                this._newPosition = this.GetWorldPosition(this._pointerEventData.position);
            else
                this._newPosition = Vector3.one * 5000f;

            this._newPosition.z     = this._initialZPosition;
            this.BallMover.position = this._newPosition;
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

        public virtual void OnPointerEnter(PointerEventData data)
        {
            this._holding          = true;
            this._pointerEventData = data;
        }

        public virtual void OnPointerExit(PointerEventData data)
        {
            this._holding = false;
        }
    }
}