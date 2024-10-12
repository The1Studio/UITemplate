// Copyright (c) Meta Platforms, Inc. and affiliates. 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lofelt.NiceVibrations
{
    public class ContinuousHapticsDemoManager : DemoManager
    {
        [Header("Texts")] public     float         ContinuousAmplitude = 1f;
        public                       float         ContinuousFrequency = 1f;
        public                       float         ContinuousDuration  = 3f;
        public                       Text          ContinuousAmplitudeText;
        public                       Text          ContinuousFrequencyText;
        public                       Text          ContinuousDurationText;
        public                       Text          ContinuousButtonText;
        [Header("Interface")] public MMTouchButton ContinuousButton;
        public                       MMProgressBar AmplitudeProgressBar;
        public                       MMProgressBar FrequencyProgressBar;
        public                       MMProgressBar DurationProgressBar;
        public                       MMProgressBar ContinuousProgressBar;
        public                       HapticCurve   TargetCurve;
        public                       Slider        DurationSlider;

        protected float _timeLeft;
        protected Color _continuousButtonOnColor  = new Color32(216, 85, 85, 255);
        protected Color _continuousButtonOffColor = new Color32(242, 27, 80, 255);
        protected bool  _continuousActive         = false;
        protected float _amplitudeLastFrame       = -1f;
        protected float _frequencyLastFrame       = -1f;

        protected virtual void Awake()
        {
            this.ContinuousButton.ReturnToInitialSpriteAutomatically = false;

            this.ContinuousAmplitudeText.text = this.ContinuousAmplitude.ToString();
            this.ContinuousFrequencyText.text = this.ContinuousFrequency.ToString();
            this.ContinuousDurationText.text  = this.ContinuousDuration.ToString();

            this.AmplitudeProgressBar.UpdateBar(this.ContinuousAmplitude, 0f, 1f);
            this.FrequencyProgressBar.UpdateBar(this.ContinuousFrequency, 0f, 1f);
            this.DurationProgressBar.UpdateBar(this.ContinuousDuration, 0f, 5f);
        }

        protected virtual void Update()
        {
            this.UpdateContinuousDemo();
        }

        protected virtual void UpdateContinuousDemo()
        {
            if (this._timeLeft > 0f)
            {
                this.ContinuousProgressBar.UpdateBar(this._timeLeft, 0f, this.ContinuousDuration);
                this._timeLeft        -= Time.deltaTime;
                this.Logo.Shaking     =  true;
                this.TargetCurve.Move =  true;
                this.Logo.Amplitude   =  NiceVibrationsDemoHelpers.Remap(this.ContinuousAmplitude, 0f, 1f, 1f, 8f);
                this.Logo.Frequency   =  NiceVibrationsDemoHelpers.Remap(this.ContinuousFrequency, 0f, 1f, 10f, 25f);
            }
            else
            {
                this.ContinuousProgressBar.UpdateBar(0f, 0f, this.ContinuousDuration);
                this.Logo.Shaking     = false;
                this.TargetCurve.Move = false;
                if (this._continuousActive) HapticController.Stop();
            }
            if (this._frequencyLastFrame != this.ContinuousFrequency || this._amplitudeLastFrame != this.ContinuousAmplitude) this.TargetCurve.UpdateCurve(this.ContinuousAmplitude, this.ContinuousFrequency);
            this._amplitudeLastFrame = this.ContinuousAmplitude;
            this._frequencyLastFrame = this.ContinuousFrequency;
        }

        public virtual void UpdateContinuousAmplitude(float newAmplitude)
        {
            this.ContinuousAmplitude = newAmplitude;
            this.AmplitudeProgressBar.UpdateBar(this.ContinuousAmplitude, 0f, 1f);
            this.ContinuousAmplitudeText.text = NiceVibrationsDemoHelpers.Round(newAmplitude, 2).ToString();
            this.UpdateContinuous();
        }

        public virtual void UpdateContinuousFrequency(float newFrequency)
        {
            this.ContinuousFrequency = newFrequency;
            this.FrequencyProgressBar.UpdateBar(this.ContinuousFrequency, 0f, 1f);
            this.ContinuousFrequencyText.text = NiceVibrationsDemoHelpers.Round(newFrequency, 2).ToString();
            this.UpdateContinuous();
        }

        public virtual void UpdateContinuousDuration(float newDuration)
        {
            this.ContinuousDuration = newDuration;
            this.DurationProgressBar.UpdateBar(this.ContinuousDuration, 0f, 5f);
            this.ContinuousDurationText.text = NiceVibrationsDemoHelpers.Round(newDuration, 2).ToString();
        }

        protected virtual void UpdateContinuous()
        {
            if (this._continuousActive)
            {
                HapticController.clipLevel          = this.ContinuousAmplitude;
                HapticController.clipFrequencyShift = this.ContinuousFrequency;
                this.DebugAudioContinuous.volume    = this.ContinuousAmplitude;
                this.DebugAudioContinuous.pitch     = 0.5f + this.ContinuousFrequency / 2f;
            }
        }

        public virtual void ContinuousHapticsButton()
        {
            if (!this._continuousActive)
            {
                // START
                HapticController.fallbackPreset = HapticPatterns.PresetType.LightImpact;
                HapticPatterns.PlayConstant(this.ContinuousAmplitude, this.ContinuousFrequency, this.ContinuousDuration);
                this._timeLeft                   = this.ContinuousDuration;
                this.ContinuousButtonText.text   = "Stop continuous haptic pattern";
                this.DurationSlider.interactable = false;
                this._continuousActive           = true;
                this.DebugAudioContinuous.Play();
            }
            else
            {
                // STOP
                HapticController.Stop();
                this.ResetPlayState();
            }
        }

        protected virtual void OnHapticsStopped()
        {
            this.ResetPlayState();
        }

        protected virtual void ResetPlayState()
        {
            this._timeLeft                 = 0f;
            this.ContinuousButtonText.text = "Play continuous haptic pattern";
            this._continuousActive         = false;
            this.DebugAudioContinuous?.Stop();
            this.DurationSlider.interactable = true;
        }

        protected virtual void OnEnable()
        {
            HapticController.PlaybackStopped += this.OnHapticsStopped;
        }

        protected virtual void OnDisable()
        {
            HapticController.PlaybackStopped -= this.OnHapticsStopped;
        }
    }
}