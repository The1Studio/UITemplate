// Copyright (c) Meta Platforms, Inc. and affiliates. 

using System.Collections.Generic;
using UnityEngine;

namespace Lofelt.NiceVibrations
{
    public class CarDemoManager : DemoManager
    {
        [Header("Control")] public MMKnob      Knob;
        public                     float       MinimumKnobValue     = 0.1f;
        public                     float       MaximumPowerDuration = 10f;
        public                     float       ChargingSpeed        = 2f;
        public                     float       CarSpeed             = 0f;
        public                     float       Power;
        public                     float       StartClickDuration = 0.2f;
        public                     float       DentDuration       = 0.10f;
        public                     List<float> Dents;

        [Header("Car")] public AudioSource   CarEngineAudioSource;
        public                 Transform     LeftWheel;
        public                 Transform     RightWheel;
        public                 RectTransform CarBody;
        public                 Vector3       WheelRotationSpeed = new(0f, 0f, 50f);

        [Header("UI")] public GameObject            ReloadingPrompt;
        public                AnimationCurve        StartClickCurve;
        public                MMProgressBar         PowerBar;
        public                List<PowerBarElement> SpeedBars;
        public                Color                 ActiveColor;
        public                Color                 InactiveColor;

        [Header("Debug")] public bool  _carStarted       = false;
        public                   float _carStartedAt     = 0f;
        public                   float _lastStartClickAt = 0f;

        protected float   _knobValueLastFrame;
        protected float   _lastDentAt = 0f;
        protected float   _knobValue;
        protected Vector3 _initialCarPosition;
        protected Vector3 _carPosition;

        protected virtual void Awake()
        {
            this.Power = this.MaximumPowerDuration;
            this.ReloadingPrompt.SetActive(false);
            this._initialCarPosition = this.CarBody.localPosition;
        }

        protected virtual void Update()
        {
            this.HandlePower();
            this.UpdateCar();
            this.UpdateUI();

            this._knobValueLastFrame = this.Knob.Value;
        }

        protected virtual void HandlePower()
        {
            this._knobValue = this.Knob.Active ? this.Knob.Value : 0f;

            if (!this._carStarted)
            {
                if (this._knobValue > this.MinimumKnobValue && this.Knob.Active)
                {
                    this._carStarted       = true;
                    this._carStartedAt     = Time.time;
                    this._lastStartClickAt = Time.time;

                    HapticPatterns.PlayConstant(this._knobValue, this._knobValue, this.MaximumPowerDuration);
                    this.CarEngineAudioSource.Play();
                }
                else
                {
                    this.Power += Time.deltaTime * this.ChargingSpeed;
                    this.Power =  Mathf.Clamp(this.Power, 0f, this.MaximumPowerDuration);

                    if (this.Power == this.MaximumPowerDuration)
                    {
                        this.Knob.SetActive(true);
                        this.Knob._rectTransform.localScale = Vector3.one;
                        this.ReloadingPrompt.SetActive(false);
                    }
                    else
                    {
                        if (!this.Knob.Active) this.Knob.SetValue(this.CarSpeed);
                    }
                }
            }
            else
            {
                if (Time.time - this._carStartedAt > this.MaximumPowerDuration)
                {
                    this._carStarted = false;
                    this.Knob.SetActive(false);
                    this.Knob._rectTransform.localScale = Vector3.one * 0.9f;
                    this.ReloadingPrompt.SetActive(true);
                }
                else
                {
                    if (this._knobValue > this.MinimumKnobValue)
                    {
                        this.Power -= Time.deltaTime;
                        this.Power =  Mathf.Clamp(this.Power, 0f, this.MaximumPowerDuration);

                        HapticController.clipLevel          = this._knobValue;
                        HapticController.clipFrequencyShift = this._knobValue;

                        if (this.Power <= 0f)
                        {
                            this._carStarted = false;
                            this.Knob.SetActive(false);
                            this.Knob._rectTransform.localScale = Vector3.one * 0.9f;
                            this.ReloadingPrompt.SetActive(true);
                            HapticController.Stop();
                        }
                    }
                    else
                    {
                        this._carStarted       = false;
                        this._lastStartClickAt = Time.time;
                        HapticController.Stop();
                    }
                }
            }
        }

        protected virtual void UpdateCar()
        {
            var targetSpeed = this._carStarted ? NiceVibrationsDemoHelpers.Remap(this.Knob.Value, this.MinimumKnobValue, 1f, 0f, 1f) : 0f;
            this.CarSpeed = Mathf.Lerp(this.CarSpeed, targetSpeed, Time.deltaTime * 1f);

            this.CarEngineAudioSource.volume = this.CarSpeed;
            this.CarEngineAudioSource.pitch  = NiceVibrationsDemoHelpers.Remap(this.CarSpeed, 0f, 1f, 0.5f, 1.25f);

            this.LeftWheel.Rotate(this.CarSpeed * Time.deltaTime * this.WheelRotationSpeed, Space.Self);
            this.RightWheel.Rotate(this.CarSpeed * Time.deltaTime * this.WheelRotationSpeed, Space.Self);

            this._carPosition.x        = this._initialCarPosition.x + 0f;
            this._carPosition.y        = this._initialCarPosition.y + 10 * this.CarSpeed * Mathf.PerlinNoise(Time.time * 10f, this.CarSpeed * 10f);
            this._carPosition.z        = 0f;
            this.CarBody.localPosition = this._carPosition;
        }

        protected virtual void UpdateUI()
        {
            if (this.Knob.Active)
            {
                // start dent
                if (Time.time - this._lastStartClickAt < this.StartClickDuration)
                {
                    var elapsedTime = this.StartClickCurve.Evaluate((Time.time - this._lastStartClickAt) * (1 / this.StartClickDuration));
                    this.Knob._rectTransform.localScale = Vector3.one + Vector3.one * elapsedTime * 0.05f;
                    this.Knob._image.color              = Color.Lerp(this.ActiveColor, Color.white, elapsedTime);
                }

                // other dents
                foreach (var f in this.Dents)
                {
                    if ((this._knobValue >= f && this._knobValueLastFrame < f) || (this._knobValue <= f && this._knobValueLastFrame > f))
                    {
                        this._lastDentAt = Time.time;
                        break;
                    }
                }
                if (Time.time - this._lastDentAt < this.DentDuration)
                {
                    var elapsedTime = this.StartClickCurve.Evaluate((Time.time - this._lastDentAt) * (1 / this.DentDuration));
                    this.Knob._rectTransform.localScale = Vector3.one + Vector3.one * elapsedTime * 0.02f;
                    this.Knob._image.color              = Color.Lerp(this.ActiveColor, Color.white, elapsedTime * 0.05f);
                }
            }

            // gas bar
            this.PowerBar.UpdateBar(this.Power, 0f, this.MaximumPowerDuration);

            // power bars
            if (this.CarSpeed <= 0.1f)
            {
                for (var i = 0; i < this.SpeedBars.Count; i++) this.SpeedBars[i].SetActive(false);
            }
            else
            {
                var barsAmount = (int)(this.CarSpeed * 5f);
                for (var i = 0; i < this.SpeedBars.Count; i++)
                {
                    if (i <= barsAmount)
                        this.SpeedBars[i].SetActive(true);
                    else
                        this.SpeedBars[i].SetActive(false);
                }
            }
        }
    }
}