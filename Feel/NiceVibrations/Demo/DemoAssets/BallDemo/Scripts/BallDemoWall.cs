// Copyright (c) Meta Platforms, Inc. and affiliates. 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lofelt.NiceVibrations
{
    public class BallDemoWall : MonoBehaviour
    {
        protected RectTransform _rectTransform;
        protected BoxCollider2D _boxCollider2D;

        protected virtual void OnEnable()
        {
            this._rectTransform = this.gameObject.GetComponent<RectTransform>();
            this._boxCollider2D = this.gameObject.GetComponent<BoxCollider2D>();

            this._boxCollider2D.size = new(this._rectTransform.rect.size.x, this._rectTransform.rect.size.y);
        }
    }
}