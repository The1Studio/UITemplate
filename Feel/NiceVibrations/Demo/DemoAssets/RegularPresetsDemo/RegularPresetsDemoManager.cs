// Copyright (c) Meta Platforms, Inc. and affiliates. 

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Lofelt.NiceVibrations
{
    public class RegularPresetsDemoManager : DemoManager
    {
        [Header("Image")] public Image    IconImage;
        public                   Animator IconImageAnimator;

        [Header("Sprites")] public Sprite IdleSprite;

        public Sprite SelectionSprite;
        public Sprite SuccessSprite;
        public Sprite WarningSprite;
        public Sprite FailureSprite;
        public Sprite RigidSprite;
        public Sprite SoftSprite;
        public Sprite LightSprite;
        public Sprite MediumSprite;
        public Sprite HeavySprite;

        protected WaitForSeconds _turnDelay;
        protected WaitForSeconds _shakeDelay;
        protected int            _idleAnimationParameter;

        protected virtual void Awake()
        {
            this._turnDelay              = new(0.02f);
            this._shakeDelay             = new(0.3f);
            this._idleAnimationParameter = Animator.StringToHash("Idle");
            this.IconImageAnimator.SetBool(this._idleAnimationParameter, true);
            this.IconImageAnimator.speed = 2f;
        }

        protected virtual void ChangeImage(Sprite newSprite)
        {
            this.StartCoroutine(this.ChangeImageCoroutine(newSprite));
        }

        protected virtual IEnumerator ChangeImageCoroutine(Sprite newSprite)
        {
            this.DebugAudioEmphasis.Play();
            this.IconImageAnimator.SetBool(this._idleAnimationParameter, false);
            yield return this._turnDelay;
            this.IconImage.sprite = newSprite;
            yield return this._shakeDelay;
            this.IconImageAnimator.SetBool(this._idleAnimationParameter, true);
            yield return this._turnDelay;
            this.IconImage.sprite = this.IdleSprite;
        }

        public virtual void SelectionButton()
        {
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
            this.ChangeImage(this.SelectionSprite);
        }

        public virtual void SuccessButton()
        {
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.Success);
            this.ChangeImage(this.SuccessSprite);
        }

        public virtual void WarningButton()
        {
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.Warning);
            this.ChangeImage(this.WarningSprite);
        }

        public virtual void FailureButton()
        {
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.Failure);
            this.ChangeImage(this.FailureSprite);
        }

        public virtual void RigidButton()
        {
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.RigidImpact);
            this.ChangeImage(this.RigidSprite);
        }

        public virtual void SoftButton()
        {
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.SoftImpact);
            this.ChangeImage(this.SoftSprite);
        }

        public virtual void LightButton()
        {
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);
            this.ChangeImage(this.LightSprite);
        }

        public virtual void MediumButton()
        {
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.MediumImpact);
            this.ChangeImage(this.MediumSprite);
        }

        public virtual void HeavyButton()
        {
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.HeavyImpact);
            this.ChangeImage(this.HeavySprite);
        }
    }
}