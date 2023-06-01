namespace TheOneStudio.UITemplate.UITemplate.Services.LoadingTransition
{
    using Cysharp.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.Playables;

    [RequireComponent(typeof(CanvasGroup))]
    public class UITemplateLoadingTransitionServices : MonoBehaviour
    {
        [SerializeField] private CanvasGroup      canvasGroup;
        [SerializeField] private PlayableDirector introAnimation;
        [SerializeField] private PlayableDirector outroAnimation;

        public UniTaskCompletionSource AnimationTask => this.animationTask;

        private UniTaskCompletionSource animationTask;

        private void Awake()
        {
            this.canvasGroup ??= this.GetComponent<CanvasGroup>();
            if (this.introAnimation.playableAsset)
            {
                this.introAnimation.playOnAwake =  false;
                this.introAnimation.played      += this.OnAnimationPlay;
                this.introAnimation.stopped     += _ => this.OnAnimCompleted();
            }

            if (this.outroAnimation.playableAsset)
            {
                this.outroAnimation.playOnAwake =  false;
                this.outroAnimation.stopped     += this.OnAnimationStopped;
                this.outroAnimation.stopped     += _ => this.OnAnimCompleted();
            }
        }

        public async UniTask PlayIntroAnimation() { await this.PlayAnimation(this.introAnimation); }

        public async UniTask PlayOutroAnimation() { await this.PlayAnimation(this.outroAnimation); }

        private async UniTask PlayAnimation(PlayableDirector anim)
        {
            this.animationTask = new UniTaskCompletionSource();
            anim.Play();
            await this.animationTask.Task;
        }

        private void OnAnimationStopped(PlayableDirector obj)
        {
            this.canvasGroup.alpha          = 0;
            this.canvasGroup.interactable   = false;
            this.canvasGroup.blocksRaycasts = false;
        }

        private void OnAnimCompleted() { this.animationTask.TrySetResult(); }

        private void OnAnimationPlay(PlayableDirector obj)
        {
            this.canvasGroup.alpha          = 1;
            this.canvasGroup.interactable   = true;
            this.canvasGroup.blocksRaycasts = true;
        }
    }
}