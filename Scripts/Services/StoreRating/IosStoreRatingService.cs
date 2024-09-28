#if UNITY_IOS && THEONE_STORE_RATING
namespace TheOneStudio.UITemplate.UITemplate.Services.StoreRating
{
    using Cysharp.Threading.Tasks;
    using UnityEngine.iOS;
    using UnityEngine.Scripting;

    [Preserve]
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