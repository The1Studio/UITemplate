// Copyright (c) Meta Platforms, Inc. and affiliates. 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Lofelt.NiceVibrations
{
    public class MMUIShaker : MonoBehaviour
    {
        public float Amplitude;
        public float Frequency;
        public bool  Shaking = false;

        protected Vector3       _initialPosition;
        protected Vector3       _shakePosition;
        protected RectTransform _rectTransform;

        protected virtual void Start()
        {
            this._rectTransform   = this.gameObject.GetComponent<RectTransform>();
            this._initialPosition = this._rectTransform.localPosition;
        }

        public virtual IEnumerator Shake(float duration)
        {
            this.Shaking = true;
            yield return new WaitForSeconds(duration);
            this.Shaking = false;
        }

        protected virtual void Update()
        {
            if (!this.Shaking)
            {
                this._rectTransform.localPosition = this._initialPosition;
                return;
            }
            else
            {
                this._shakePosition.x             = Mathf.PerlinNoise(-Time.time * this.Frequency, Time.time * this.Frequency) * this.Amplitude - this.Amplitude / 2f;
                this._shakePosition.y             = Mathf.PerlinNoise(-(Time.time + 0.25f) * this.Frequency, Time.time * this.Frequency) * this.Amplitude - this.Amplitude / 2f;
                this._shakePosition.z             = Mathf.PerlinNoise(-(Time.time + 0.5f) * this.Frequency, Time.time * this.Frequency) * this.Amplitude - this.Amplitude / 2f;
                this._rectTransform.localPosition = this._initialPosition + this._shakePosition;
            }
        }
    }
}