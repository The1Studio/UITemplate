namespace TheOneStudio.UITemplate.UITemplate.Scenes.ATT
{
    public static class AttHelper
    {
        public static bool IsRequestTrackingComplete()
        {
#if UNITY_IOS && !UNITY_EDITOR
            return Unity.Advertisement.IosSupport.ATTrackingStatusBinding.GetAuthorizationTrackingStatus() != Unity.Advertisement.IosSupport.ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED;
#endif
            return true;
        }
    }
}