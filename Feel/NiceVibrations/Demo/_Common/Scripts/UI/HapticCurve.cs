// Copyright (c) Meta Platforms, Inc. and affiliates. 

using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Lofelt.NiceVibrations
{
    public class HapticCurve : MonoBehaviour
    {
        [Range(0f, 1f)] public  float         Amplitude       = 1f;
        [Range(0f, 1f)] public  float         Frequency       = 0f;
        public                  int           PointsCount     = 50;
        public                  float         AmplitudeFactor = 3;
        [Range(1f, 4f)] private float         Period          = 1;
        public                  RectTransform StartPoint;
        public                  RectTransform EndPoint;

        [Header("Movement")] public bool  Move          = false;
        public                      float MovementSpeed = 1f;

        protected LineRenderer  _targetLineRenderer;
        protected List<Vector3> Points;

        protected Canvas _canvas;
        protected Camera _camera;

        protected Vector3 _startPosition;
        protected Vector3 _endPosition;
        protected Vector3 _workPoint;

        protected virtual void Awake()
        {
            this.Initialization();
        }

        protected virtual void Initialization()
        {
            this.Points              = new();
            this._canvas             = this.gameObject.GetComponentInParent<Canvas>();
            this._targetLineRenderer = this.gameObject.GetComponent<LineRenderer>();
            this._camera             = this._canvas.worldCamera;
            this.DrawCurve();
        }

        protected virtual void DrawCurve()
        {
            this._startPosition   =  this.StartPoint.transform.position;
            this._startPosition.z -= 0.1f;
            this._endPosition     =  this.EndPoint.transform.position;
            this._endPosition.z   -= 0.1f;

            this.Points.Clear();

            for (var i = 0; i < this.PointsCount; i++)
            {
                var t        = NiceVibrationsDemoHelpers.Remap(i, 0, this.PointsCount, 0f, 1f);
                var sinValue = MMSignal.GetValue(t, MMSignal.SignalType.Sine, 1f, this.AmplitudeFactor, this.Period, 0f, false);

                if (this.Move) sinValue = MMSignal.GetValue(t + Time.time * this.MovementSpeed, MMSignal.SignalType.Sine, 1f, this.AmplitudeFactor, this.Period, 0f, false);

                this._workPoint.x = Mathf.Lerp(this._startPosition.x, this._endPosition.x, t);
                this._workPoint.y = sinValue * this.Amplitude + this._startPosition.y;
                this._workPoint.z = this._startPosition.z;
                this.Points.Add(this._workPoint);
            }

            this._targetLineRenderer.positionCount = this.PointsCount;
            this._targetLineRenderer.SetPositions(this.Points.ToArray());
        }

        protected virtual void Update()
        {
            this.UpdateCurve(this.Amplitude, this.Frequency);
        }

        public virtual void UpdateCurve(float amplitude, float frequency)
        {
            this.Amplitude = amplitude;
            this.Frequency = frequency;
            this.Period    = NiceVibrationsDemoHelpers.Remap(frequency, 0f, 1f, 1f, 4f);
            this.DrawCurve();
        }
    }
}