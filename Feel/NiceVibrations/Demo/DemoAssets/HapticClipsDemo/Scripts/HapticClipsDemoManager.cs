// Copyright (c) Meta Platforms, Inc. and affiliates. 

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lofelt.NiceVibrations
{
    [Serializable]
    public class HapticClipsDemoItem
    {
        public string      Name;
        public HapticClip  HapticClip;
        public Sprite      AssociatedSprite;
        public AudioSource AssociatedSound;
    }

    public class HapticClipsDemoManager : DemoManager
    {
        [Header("Image")] public Image                     IconImage;
        public                   Animator                  IconImageAnimator;
        public                   List<HapticClipsDemoItem> DemoItems;
        protected                WaitForSeconds            _iconChangeDelay;
        protected                int                       _idleAnimationParameter;

        protected virtual void Awake()
        {
            this._iconChangeDelay        = new(0.02f);
            this._idleAnimationParameter = Animator.StringToHash("Idle");
            this.IconImageAnimator.SetBool(this._idleAnimationParameter, true);
        }

        // Haptic Clip -----------------------------------------------------------------------------

        public virtual void PlayHapticClip(int index)
        {
            this.Logo.Shaking = true;

            HapticController.fallbackPreset = HapticPatterns.PresetType.LightImpact;
            HapticController.Play(this.DemoItems[index].HapticClip);
            this.DemoItems[index].AssociatedSound.Play();
            this.StopAllCoroutines();
            this.StartCoroutine(this.ChangeIcon(this.DemoItems[index].AssociatedSprite));
        }

        // ICON ------------------------------------------------------------------------------------

        protected virtual IEnumerator ChangeIcon(Sprite newSprite)
        {
            this.IconImageAnimator.SetBool(this._idleAnimationParameter, false);
            yield return this._iconChangeDelay;
            this.IconImage.sprite = newSprite;
        }

        // CALLBACKS -------------------------------------------------------------------------------

        protected virtual IEnumerator BackToIdle()
        {
            this.Logo.Shaking = false;
            this.IconImageAnimator.SetBool(this._idleAnimationParameter, true);
            yield return this._iconChangeDelay;
            this.IconImage.sprite = this.DemoItems[0].AssociatedSprite;
        }

        private void OnHapticsStopped()
        {
            this.StartCoroutine(this.BackToIdle());
        }

        private void OnDisable()
        {
            HapticController.PlaybackStopped -= this.OnHapticsStopped;
            if (HapticController.IsPlaying()) HapticController.Stop();
        }

        private void OnEnable()
        {
            HapticController.PlaybackStopped += this.OnHapticsStopped;
            this.StartCoroutine(this.BackToIdle());
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus) this.StartCoroutine(this.BackToIdle());
        }
    }
}