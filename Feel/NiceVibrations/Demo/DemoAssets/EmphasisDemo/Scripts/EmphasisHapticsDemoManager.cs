// Copyright (c) Meta Platforms, Inc. and affiliates. 

using UnityEngine;
using UnityEngine.UI;

namespace Lofelt.NiceVibrations
{
    public class EmphasisHapticsDemoManager : DemoManager
    {
        [Header("Emphasis Haptics")] public MMProgressBar AmplitudeProgressBar;
        public                              MMProgressBar FrequencyProgressBar;
        public                              HapticCurve   TargetCurve;
        public                              float         EmphasisAmplitude = 1f;
        public                              float         EmphasisFrequency = 1f;
        public                              Text          EmphasisAmplitudeText;
        public                              Text          EmphasisFrequencyText;

        protected virtual void Start()
        {
            this.FrequencyProgressBar.UpdateBar(1f, 0f, 1f);
            this.AmplitudeProgressBar.UpdateBar(1f, 0f, 1f);
            this.TargetCurve.UpdateCurve(this.EmphasisAmplitude, this.EmphasisFrequency);

            HapticController.fallbackPreset = HapticPatterns.PresetType.RigidImpact;
        }

        public virtual void UpdateEmphasisAmplitude(float newAmplitude)
        {
            this.EmphasisAmplitude          = newAmplitude;
            this.EmphasisAmplitudeText.text = NiceVibrationsDemoHelpers.Round(newAmplitude, 2).ToString();
            this.AmplitudeProgressBar.UpdateBar(this.EmphasisAmplitude, 0f, 1f);
            this.TargetCurve.UpdateCurve(this.EmphasisAmplitude, this.EmphasisFrequency);
        }

        public virtual void UpdateEmphasisFrequency(float newFrequency)
        {
            this.EmphasisFrequency          = newFrequency;
            this.EmphasisFrequencyText.text = NiceVibrationsDemoHelpers.Round(newFrequency, 2).ToString();
            this.FrequencyProgressBar.UpdateBar(this.EmphasisFrequency, 0f, 1f);
            this.TargetCurve.UpdateCurve(this.EmphasisAmplitude, this.EmphasisFrequency);
        }

        public virtual void EmphasisHapticsButton()
        {
            HapticPatterns.PlayEmphasis(this.EmphasisAmplitude, this.EmphasisFrequency);
            this.StartCoroutine(this.Logo.Shake(0.2f));
            this.DebugAudioEmphasis.volume = this.EmphasisAmplitude;
            this.DebugAudioEmphasis.pitch  = 0.5f + this.EmphasisFrequency / 2f;
            this.DebugAudioEmphasis.Play();
        }
    }
}