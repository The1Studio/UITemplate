#if UNITY_ANDROID && STORE_RATING
namespace TheOneStudio.UITemplate.UITemplate.Services.StoreRating
{
    using Cysharp.Threading.Tasks;
    using Google.Play.Review;
    using UnityEngine;

    public class AndroidStoreRatingService : IStoreRatingService
    {
        private ReviewManager  reviewManager;
        private PlayReviewInfo playReviewInfo;

        private readonly UniTask initRatingTask;

        public AndroidStoreRatingService() { this.initRatingTask = this.InitRating(); }

        private async UniTask InitRating(bool force = false)
        {
            this.reviewManager ??= new ReviewManager();

            var requestFlowOperation = this.reviewManager.RequestReviewFlow();
            await requestFlowOperation;
            if (requestFlowOperation.Error != ReviewErrorCode.NoError)
            {
                if (force) this.DirectlyOpen();
                return;
            }

            this.playReviewInfo = requestFlowOperation.GetResult();
        }

        public async UniTask LaunchStoreRating()
        {
            if (this.playReviewInfo == null)
            {
                if (this.initRatingTask.Status != UniTaskStatus.Succeeded)
                {
                    this.initRatingTask.Forget();
                }

                await this.InitRating(true);
            }

            var launchFlowOperation = this.reviewManager.LaunchReviewFlow(this.playReviewInfo);
            await launchFlowOperation;
            this.playReviewInfo = null;
            if (launchFlowOperation.Error != ReviewErrorCode.NoError)
            {
                this.DirectlyOpen();
            }
        }

        private void DirectlyOpen() { Application.OpenURL($"https://play.google.com/store/apps/details?id={Application.identifier}"); }
    }
}
#endif