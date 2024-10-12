// Copyright (c) Meta Platforms, Inc. and affiliates. 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lofelt.NiceVibrations
{
    public class V2DemoManager : MonoBehaviour
    {
        public List<RectTransform> Pages;
        public int                 CurrentPage            = 0;
        public float               PageTransitionDuration = 1f;
        public AnimationCurve      TransitionCurve;
        public Color               ActiveColor;
        public Color               InactiveColor;
        public bool                SoundActive = true;

        protected Vector3          _position;
        protected List<Pagination> _paginations;
        protected Coroutine        _transitionCoroutine;

        protected virtual void Start()
        {
            this.Initialization();
        }

        protected virtual void Initialization()
        {
            Application.targetFrameRate = 60;
            this._paginations           = new();
            foreach (var page in this.Pages)
            {
                this._paginations.Add(page.GetComponentInChildren<Pagination>());
                page.gameObject.SetActive(false);
            }
            foreach (var pagination in this._paginations)
            {
                pagination.InitializePagination(this.Pages.Count);
                pagination.ActiveColor   = this.ActiveColor;
                pagination.InactiveColor = this.InactiveColor;
                pagination.SetCurrentPage(this.Pages.Count, 0);
            }
            this.Pages[0].gameObject.SetActive(true);
            if (this.SoundActive)
            {
                AudioListener.volume = 1f;
                this.SoundActive     = true;
            }
            else
            {
                AudioListener.volume = 0f;
                this.SoundActive     = false;
            }
        }

        public virtual void PreviousPage()
        {
            if (this.CurrentPage > 0)
            {
                this.CurrentPage--;
                this.Transition(this.CurrentPage + 1, this.CurrentPage, false);
                this.SetCurrentPage();
            }
        }

        public virtual void NextPage()
        {
            if (this.CurrentPage < this.Pages.Count - 1)
            {
                this.CurrentPage++;
                this.Transition(this.CurrentPage - 1, this.CurrentPage, true);
                this.SetCurrentPage();
            }
        }

        protected virtual void SetCurrentPage()
        {
            foreach (var pagination in this._paginations) pagination.SetCurrentPage(this.Pages.Count, this.CurrentPage);
        }

        protected virtual void Transition(int previous, int next, bool goingRight)
        {
            HapticController.Reset();

            if (this._transitionCoroutine != null) this.StopCoroutine(this._transitionCoroutine);
            this._transitionCoroutine = this.StartCoroutine(this.TransitionCoroutine(previous, next, goingRight));
        }

        protected virtual IEnumerator TransitionCoroutine(int previous, int next, bool goingRight)
        {
            this._position.y = this.Pages[previous].localPosition.y;
            this._position.z = this.Pages[previous].localPosition.z;

            foreach (var page in this.Pages)
            {
                this._position.x   = 1200f;
                page.localPosition = this._position;
            }

            this.Pages[next].gameObject.SetActive(true);

            var timeSpent = 0f;
            while (timeSpent < this.PageTransitionDuration)
            {
                if (goingRight)
                {
                    this._position.x                   = Mathf.Lerp(0f, -1200f, this.TransitionCurve.Evaluate(NiceVibrationsDemoHelpers.Remap(timeSpent, 0f, this.PageTransitionDuration, 0f, 1f)));
                    this.Pages[previous].localPosition = this._position;
                    this._position.x                   = Mathf.Lerp(1200f, 0f, this.TransitionCurve.Evaluate(NiceVibrationsDemoHelpers.Remap(timeSpent, 0f, this.PageTransitionDuration, 0f, 1f)));
                    this.Pages[next].localPosition     = this._position;
                }
                else
                {
                    this._position.x                   = Mathf.Lerp(0f, 1200f, this.TransitionCurve.Evaluate(NiceVibrationsDemoHelpers.Remap(timeSpent, 0f, this.PageTransitionDuration, 0f, 1f)));
                    this.Pages[previous].localPosition = this._position;
                    this._position.x                   = Mathf.Lerp(-1200f, 0f, this.TransitionCurve.Evaluate(NiceVibrationsDemoHelpers.Remap(timeSpent, 0f, this.PageTransitionDuration, 0f, 1f)));
                    this.Pages[next].localPosition     = this._position;
                }

                timeSpent += Time.deltaTime;
                yield return null;
            }

            this.Pages[previous].gameObject.SetActive(false);
        }

        public virtual void TurnHapticsOn()
        {
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.Success);
        }

        public virtual void TurnHapticsOff()
        {
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.Warning);
        }

        public virtual void TurnSoundsOn()
        {
            AudioListener.volume = 1f;
            this.SoundActive     = true;
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.Success);
        }

        public virtual void TurnSoundsOff()
        {
            AudioListener.volume = 0f;
            this.SoundActive     = false;
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.Warning);
        }
    }
}