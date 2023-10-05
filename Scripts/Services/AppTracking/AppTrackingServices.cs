namespace TheOneStudio.UITemplate.UITemplate.Services.AppTracking
{
    using Cysharp.Threading.Tasks;
    using Zenject;

    public class AppTrackingServices : IInitializable
    {
        public bool AutoRequestTracking             { get; set; } = true;
        public int  DelayRequestTrackingMillisecond { get; set; } = 100;

        public async void Initialize()
        {
            await UniTask.Delay(this.DelayRequestTrackingMillisecond);
            if (this.AutoRequestTracking)
            {
                await RequestTracking();
            }
        }

        public static async UniTask RequestTracking()
        {
            if (IsRequestTrackingComplete()) return;

#if UNITY_IOS
            Unity.Advertisement.IosSupport.ATTrackingStatusBinding.RequestAuthorizationTracking();
            await UniTask.WaitUntil(IsRequestTrackingComplete);
#endif
        }

        private static bool IsRequestTrackingComplete()
        {
#if UNITY_IOS && !UNITY_EDITOR
                return Unity.Advertisement.IosSupport.ATTrackingStatusBinding.GetAuthorizationTrackingStatus() != Unity.Advertisement.IosSupport.ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED;
#endif

            return true;
        }
    }

}