// Copyright (c) Meta Platforms, Inc. and affiliates. 

using UnityEngine;

namespace Lofelt.NiceVibrations
{
    public class BallDemoBall : MonoBehaviour
    {
        public bool           HapticsEnabled = true;
        public ParticleSystem HitParticles;
        public ParticleSystem HitPusherParticles;
        public LayerMask      WallMask;
        public LayerMask      PusherMask;
        public MMUIShaker     LogoShaker;
        public AudioSource    EmphasisAudioSource;

        protected Rigidbody2D _rigidBody;
        protected float       _lastRaycastTimestamp = 0f;
        protected Animator    _ballAnimator;
        protected int         _hitAnimationParameter;

        protected virtual void Awake()
        {
            this._rigidBody             = this.gameObject.GetComponent<Rigidbody2D>();
            this._ballAnimator          = this.gameObject.GetComponent<Animator>();
            this._hitAnimationParameter = Animator.StringToHash("Hit");
        }

        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {
            if (this.WallMask == (this.WallMask | (1 << collision.gameObject.layer))) this.HitWall();
        }

        protected virtual void Update()
        {
            var raycastLength = 5f;

            Debug.DrawLine(this.transform.position, Vector3.down * raycastLength, Color.red);

            if (Time.time - this._lastRaycastTimestamp > 1f)
            {
                this._lastRaycastTimestamp = Time.time;
                var hit = Physics2D.Raycast(this.transform.position, Vector2.down, raycastLength, this.WallMask);
                if (hit.collider != null) this.HitBottom();
            }
        }

        protected virtual void HitBottom()
        {
            this._rigidBody.AddForce(Vector2.up * 2500f);
            this.StartCoroutine(this.LogoShaker.Shake(0.2f));
        }

        protected virtual void HitWall()
        {
            var amplitude = this._rigidBody.velocity.magnitude / 100f;
            HapticPatterns.PlayEmphasis(amplitude, 0.7f);
            this.EmphasisAudioSource.volume = amplitude;
            this.StartCoroutine(this.LogoShaker.Shake(0.2f));
            this.EmphasisAudioSource.Play();
            this._ballAnimator.SetTrigger(this._hitAnimationParameter);
        }

        public virtual void HitPusher()
        {
            this.HitPusherParticles.Play();
            HapticController.fallbackPreset = HapticPatterns.PresetType.Selection;
            HapticPatterns.PlayEmphasis(0.85f, 0.05f);
            this.EmphasisAudioSource.volume = 0.1f;
            this.StartCoroutine(this.LogoShaker.Shake(0.2f));
            this.EmphasisAudioSource.Play();
            this._ballAnimator.SetTrigger(this._hitAnimationParameter);
        }
    }
}