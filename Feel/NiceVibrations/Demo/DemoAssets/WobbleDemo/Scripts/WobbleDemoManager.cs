// Copyright (c) Meta Platforms, Inc. and affiliates. 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lofelt.NiceVibrations
{
    public class WobbleDemoManager : DemoManager
    {
        public Camera        ButtonCamera;
        public RectTransform ContentZone;
        public WobbleButton  WobbleButtonPrefab;
        public Vector2       PrefabSize = new(200f, 200f);
        public float         Margin     = 20f;
        public float         Padding    = 20f;

        protected List<WobbleButton> Buttons;
        protected Canvas             _canvas;
        protected Vector3            _position = Vector3.zero;

        protected virtual void Start()
        {
            this._canvas = this.GetComponentInParent<Canvas>();

            var horizontalF = (this.ContentZone.rect.width - 2 * this.Padding) / (this.PrefabSize.x + this.Margin);
            var verticalF   = (this.ContentZone.rect.height - 2 * this.Padding) / (this.PrefabSize.y + this.Margin);
            var horizontal  = Mathf.FloorToInt(horizontalF);
            var vertical    = Mathf.FloorToInt(verticalF);

            var centerH = (this.ContentZone.rect.width - this.Padding * 2 - horizontal * this.PrefabSize.x - (horizontal - 1) * this.Margin) / 2f;
            var centerV = (this.ContentZone.rect.height - this.Padding * 2 - vertical * this.PrefabSize.x - (vertical - 1) * this.Margin) / 2f;

            this.Buttons = new();

            for (var i = 0; i < horizontal; i++)
            for (var j = 0; j < vertical; j++)
            {
                this._position.x = centerH + this.Padding + this.PrefabSize.x / 2f + i * (this.PrefabSize.x + this.Margin);
                this._position.y = centerV + this.Padding + this.PrefabSize.y / 2f + j * (this.PrefabSize.y + this.Margin);
                this._position.z = 0f;

                var button = Instantiate(this.WobbleButtonPrefab);
                button.transform.SetParent(this.ContentZone.transform);
                this.Buttons.Add(button);

                var rectTransform = button.GetComponent<RectTransform>();
                rectTransform.anchorMin     = Vector2.zero;
                rectTransform.anchorMax     = Vector2.zero;
                button.name                 = "WobbleButton" + i + j;
                button.transform.localScale = Vector3.one;

                rectTransform.anchoredPosition3D = this._position;
                button.TargetCamera              = this.ButtonCamera;
                button.Initialization();
            }

            var counter = 0;
            foreach (var wbutton in this.Buttons)
            {
                var newPitch = NiceVibrationsDemoHelpers.Remap(counter, 0f, this.Buttons.Count, 0.3f, 1f);
                wbutton.SetPitch(newPitch);
                counter++;
            }
        }
    }
}