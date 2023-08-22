#if UNITY_IOS && STORE_RATING
namespace TheOneStudio.UITemplate.UITemplate.Services.StoreRating
{
    using Cysharp.Threading.Tasks;
    using UnityEngine.iOS;

    public class IosStoreRatingService : IStoreRatingService
    {
        public UniTask LaunchStoreRating()
        {
            Device.RequestStoreReview();
            return UniTask.CompletedTask;
        }
    }
}
#endif