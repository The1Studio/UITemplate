// Copyright (c) Meta Platforms, Inc. and affiliates. 

using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

namespace Lofelt.NiceVibrations
{
    /// <summary>
    /// A class to add to an Image or SpriteRenderer to have it act like a button with a different sprite for on and off states
    /// </summary>
    public class MMSpriteReplace : MonoBehaviour
    {
        [Header("Sprites")]
        /// the sprite to use when in the "on" state
        public Sprite OnSprite;

        /// the sprite to use when in the "off" state
        public Sprite OffSprite;

        [Header("Start settings")]
        /// if this is true, the button will start if "on" state
        public bool StartsOn = true;

        /// the current state of the button
        public bool CurrentValue => this._image.sprite == this.OnSprite;

        protected Image          _image;
        protected SpriteRenderer _spriteRenderer;
        protected MMTouchButton  _mmTouchButton;

        /// <summary>
        /// On Start we initialize our button
        /// </summary>
        protected virtual void Start()
        {
            this.Initialization();
        }

        /// <summary>
        /// On init, we grab our image component, and set our sprite in its initial state
        /// </summary>
        protected virtual void Initialization()
        {
            // grabs components
            this._image          = this.GetComponent<Image>();
            this._spriteRenderer = this.GetComponent<SpriteRenderer>();

            // grabs button
            this._mmTouchButton = this.GetComponent<MMTouchButton>();
            if (this._mmTouchButton != null) this._mmTouchButton.ReturnToInitialSpriteAutomatically = false;

            // handles start
            if (this.OnSprite == null || this.OffSprite == null) return;

            if (this._image != null)
            {
                if (this.StartsOn)
                    this._image.sprite = this.OnSprite;
                else
                    this._image.sprite = this.OffSprite;
            }

            if (this._spriteRenderer != null)
            {
                if (this.StartsOn)
                    this._spriteRenderer.sprite = this.OnSprite;
                else
                    this._spriteRenderer.sprite = this.OffSprite;
            }
        }

        /// <summary>
        /// A public method to change the sprite
        /// </summary>
        public virtual void Swap()
        {
            if (this._image != null)
            {
                if (this._image.sprite != this.OnSprite)
                    this.SwitchToOnSprite();
                else
                    this.SwitchToOffSprite();
            }

            if (this._spriteRenderer != null)
            {
                if (this._spriteRenderer.sprite != this.OnSprite)
                    this.SwitchToOnSprite();
                else
                    this.SwitchToOffSprite();
            }
        }

        /// <summary>
        /// a public method to switch to off sprite directly
        /// </summary>
        public virtual void SwitchToOffSprite()
        {
            if (this._image == null && this._spriteRenderer == null) return;
            if (this.OffSprite == null) return;

            this.SpriteOff();
        }

        /// <summary>
        /// sets the image's sprite to off
        /// </summary>
        protected virtual void SpriteOff()
        {
            if (this._image != null) this._image.sprite                   = this.OffSprite;
            if (this._spriteRenderer != null) this._spriteRenderer.sprite = this.OffSprite;
        }

        /// <summary>
        /// a public method to switch to on sprite directly
        /// </summary>
        public virtual void SwitchToOnSprite()
        {
            if (this._image == null && this._spriteRenderer == null) return;
            if (this.OnSprite == null) return;

            this.SpriteOn();
        }

        /// <summary>
        /// sets the image's sprite to on
        /// </summary>
        protected virtual void SpriteOn()
        {
            if (this._image != null) this._image.sprite                   = this.OnSprite;
            if (this._spriteRenderer != null) this._spriteRenderer.sprite = this.OnSprite;
        }
    }
}