namespace TheOneStudio.UITemplate.UITemplate.Services.AppTracking
{
    using Cysharp.Threading.Tasks;
    using TheOneStudio.UITemplate.UITemplate.Configs.GameEvents;
    using Zenject;

    public class AppTrackingServices : IInitializable
    {
        public int DelayRequestTrackingMillisecond { get; set; } = 100;

        private readonly GameFeaturesSetting gameFeaturesSetting;

        public AppTrackingServices(GameFeaturesSetting gameFeaturesSetting) { this.gameFeaturesSetting = gameFeaturesSetting; }

        public async void Initialize()
        {
            await UniTask.Delay(this.DelayRequestTrackingMillisecond);
            if (this.gameFeaturesSetting.autoRequestATT)
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