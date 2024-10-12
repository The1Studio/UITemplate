// Copyright (c) Meta Platforms, Inc. and affiliates. 

using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Lofelt.NiceVibrations
{
    /// <summary>
    /// A component to handle switches
    /// </summary>
    public class MMSwitch : MMTouchButton
    {
        [Header("Switch")]
        /// a SpriteReplace to represent the switch knob
        public Image SwitchKnob;

        /// the possible states of the switch
        public enum SwitchStates
        {
            Off,
            On,
        }

        /// the current state of the switch
        public SwitchStates CurrentSwitchState { get; set; }

        [Header("Knob")]
        /// the state the switch should start in
        public SwitchStates InitialState = SwitchStates.Off;

        public Transform      OffPosition;
        public Transform      OnPosition;
        public AnimationCurve KnobMovementCurve    = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        public float          KnobMovementDuration = 0.2f;

        [Header("Binding")]
        /// the methods to call when the switch is turned on
        public UnityEvent SwitchOn;

        /// the methods to call when the switch is turned off
        public UnityEvent SwitchOff;

        protected float _knobMovementStartedAt = -50f;

        /// <summary>
        /// On init, we set our current switch state
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            this.CurrentSwitchState = this.InitialState;
            this.InitializeState();
        }

        public virtual void InitializeState()
        {
            if (this.CurrentSwitchState == SwitchStates.Off)
            {
                if (this._animator != null) this._animator.Play("RollLeft");
                this.SwitchKnob.transform.position = this.OffPosition.transform.position;
            }
            else
            {
                if (this._animator != null) this._animator.Play("RollRight");
                this.SwitchKnob.transform.position = this.OnPosition.transform.position;
            }
        }

        protected override void Update()
        {
            base.Update();
            if (Time.time - this._knobMovementStartedAt < this.KnobMovementDuration)
            {
                var time  = this.Remap(Time.time - this._knobMovementStartedAt, 0f, this.KnobMovementDuration, 0f, 1f);
                var value = this.KnobMovementCurve.Evaluate(time);

                if (this.CurrentSwitchState == SwitchStates.Off)
                    this.SwitchKnob.transform.position = Vector3.Lerp(this.OnPosition.transform.position, this.OffPosition.transform.position, value);
                else
                    this.SwitchKnob.transform.position = Vector3.Lerp(this.OffPosition.transform.position, this.OnPosition.transform.position, value);
            }
        }

        /// <summary>
        /// Use this method to go from one state to the other
        /// </summary>
        public virtual void SwitchState()
        {
            this._knobMovementStartedAt = Time.time;
            if (this.CurrentSwitchState == SwitchStates.Off)
            {
                this.CurrentSwitchState = SwitchStates.On;
                if (this._animator != null) this._animator?.SetTrigger("Right");
                if (this.SwitchOn != null) this.SwitchOn.Invoke();
            }
            else
            {
                this.CurrentSwitchState = SwitchStates.Off;
                if (this._animator != null) this._animator?.SetTrigger("Left");
                if (this.SwitchOff != null) this.SwitchOff.Invoke();
            }
        }
    }
}