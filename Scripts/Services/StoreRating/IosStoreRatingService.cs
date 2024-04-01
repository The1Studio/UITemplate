#if UNITY_IOS && THEONE_RATE_US
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