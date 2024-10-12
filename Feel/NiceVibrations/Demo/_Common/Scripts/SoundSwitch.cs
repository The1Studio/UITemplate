// Copyright (c) Meta Platforms, Inc. and affiliates. 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lofelt.NiceVibrations
{
    public class SoundSwitch : MonoBehaviour
    {
        public V2DemoManager DemoManager;

        protected MMSwitch _switch;

        protected virtual void Awake()
        {
            this._switch = this.gameObject.GetComponent<MMSwitch>();
        }

        protected virtual void OnEnable()
        {
            if (this.DemoManager.SoundActive)
            {
                this._switch.CurrentSwitchState = MMSwitch.SwitchStates.On;
                this._switch.InitializeState();
            }
            else
            {
                this._switch.CurrentSwitchState = MMSwitch.SwitchStates.Off;
                this._switch.InitializeState();
            }
        }
    }
}