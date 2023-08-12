#if UNITY_ANDROID && STORE_RATING
namespace TheOneStudio.UITemplate.UITemplate.Services.StoreRating
{
    using System.Collections;
    using Google.Play.Review;
    using UnityEngine;

    public class AndroidStoreRatingService : MonoBehaviour, IStoreRatingService
    {
        private ReviewManager  reviewManager;
        private PlayReviewInfo playReviewInfo;
        private Coroutine      coroutine;

        private void Start() { this.coroutine = this.StartCoroutine(this.InitReview()); }

        public void LaunchStoreRating() { this.StartCoroutine(this.LaunchReview()); }

        private IEnumerator InitReview(bool force = false)
        {
            this.reviewManager ??= new ReviewManager();

            var requestFlowOperation = this.reviewManager.RequestReviewFlow();
            yield return requestFlowOperation;
            if (requestFlowOperation.Error != ReviewErrorCode.NoError)
            {
                if (force) this.DirectlyOpen();
                yield break;
            }

            this.playReviewInfo = requestFlowOperation.GetResult();
        }

        private IEnumerator LaunchReview()
        {
            if (this.playReviewInfo == null)
            {
                if (this.coroutine != null) this.StopCoroutine(this.coroutine);
                yield return this.StartCoroutine(this.InitReview(true));
            }

            var launchFlowOperation = this.reviewManager.LaunchReviewFlow(this.playReviewInfo);
            yield return launchFlowOperation;
            this.playReviewInfo = null;
            if (launchFlowOperation.Error == ReviewErrorCode.NoError) yield break;
            this.DirectlyOpen();
        }

        private void DirectlyOpen() { Application.OpenURL($"https://play.google.com/store/apps/details?id={Application.identifier}"); }
    }
}
#endif