// Copyright (c) Meta Platforms, Inc. and affiliates. 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lofelt.NiceVibrations
{
    public class PowerBarElement : MonoBehaviour
    {
        public float          BumpDuration = 0.15f;
        public Color          NormalColor;
        public Color          InactiveColor;
        public AnimationCurve Curve;

        protected Image _image;
        protected float _bumpDuration    = 0f;
        protected bool  _active          = false;
        protected bool  _activeLastFrame = false;

        protected virtual void Awake()
        {
            this._image = this.gameObject.GetComponent<Image>();
        }

        public virtual void SetActive(bool status)
        {
            this._active      = status;
            this._image.color = status ? this.NormalColor : this.InactiveColor;
        }

        protected virtual void Update()
        {
            if (this._active && !this._activeLastFrame) this.StartCoroutine(this.ColorBump());
            this._activeLastFrame = this._active;
        }

        protected virtual IEnumerator ColorBump()
        {
            this._bumpDuration = 0f;
            while (this._bumpDuration < this.BumpDuration)
            {
                var curveValue = this.Curve.Evaluate(this._bumpDuration / this.BumpDuration);
                this._image.color = Color.Lerp(this.NormalColor, Color.white, curveValue);

                this._bumpDuration += Time.deltaTime;
                yield return null;
            }

            this._image.color = this.NormalColor;
        }
    }
}