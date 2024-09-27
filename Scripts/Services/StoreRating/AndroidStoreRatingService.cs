#if UNITY_ANDROID && THEONE_STORE_RATING
namespace TheOneStudio.UITemplate.UITemplate.Services.StoreRating
{
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using Google.Play.Review;
    using UnityEngine;
    using UnityEngine.Scripting;

    [Preserve]
    public class AndroidStoreRatingService : IStoreRatingService
    {
        private ReviewManager           reviewManager;
        private PlayReviewInfo          playReviewInfo;
        private CancellationTokenSource initReviewTaskSource;

        public async UniTask LaunchStoreRating() { await this.LaunchReview(); }

        private async UniTask InitReview(bool force = false)
        {
            this.initReviewTaskSource = new CancellationTokenSource();
            this.reviewManager ??= new ReviewManager();

            var requestFlowOperation = this.reviewManager.RequestReviewFlow();
            await requestFlowOperation.ToUniTask(cancellationToken: this.initReviewTaskSource.Token);
            if (requestFlowOperation.Error != ReviewErrorCode.NoError)
            {
                if (force) this.DirectlyOpen();
                return;
            }

            this.playReviewInfo = requestFlowOperation.GetResult();
            this.initReviewTaskSource?.Dispose();
        }

        private async UniTask LaunchReview()
        {
            if (this.playReviewInfo == null)
            {
                this.initReviewTaskSource?.Dispose();
                await this.InitReview(true);
            }

            var launchFlowOperation = this.reviewManager.LaunchReviewFlow(this.playReviewInfo);
            await launchFlowOperation;
            this.playReviewInfo = null;
            if (launchFlowOperation.Error == ReviewErrorCode.NoError) return;
            this.DirectlyOpen();
        }

        private void DirectlyOpen() { Application.OpenURL($"https://play.google.com/store/apps/details?id={Application.identifier}"); }
    }
}
#endif