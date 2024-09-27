namespace TheOneStudio.UITemplate.UITemplate.Scenes.FeaturesConfig
{
    using GameFoundation.DI;
    using ServiceImplementation.FireBaseRemoteConfig;
    using UnityEngine.Scripting;

    public class UITemplateFeatureConfig : IInitializable
    {
        private const string IsDailyRewardEnableKey = "IsDailyRewardEnable";
        private const string IsLeaderboardEnableKey = "IsLeaderboardEnable";
        private const string IsLuckySpinEnableKey   = "IsLuckySpinEnable";
        private const string IsChestRoomEnableKey   = "IsChestRoomEnable";
        private const string IsIAPEnableKey         = "IsIAPEnable";
        private const string IsRemoveAdsEnableKey   = "IsRemoveAdsEnable";
        private const string IsSuggestionEnableKey  = "IsSuggestionEnable";
        private const string IsBuildingEnableKey    = "IsBuildingEnable";
        private const string IsVFXEnableKey         = "IsVFXEnable";
        private const string IsSFXEnableKey         = "IsSFXEnable";

        private readonly IRemoteConfig uiTemplateRemoteConfig;

        [Preserve]
        public UITemplateFeatureConfig(IRemoteConfig uiTemplateRemoteConfig)
        {
            this.uiTemplateRemoteConfig = uiTemplateRemoteConfig;
        }

        public bool IsDailyRewardEnable { get; set; } = true;
        public bool IsLeaderboardEnable { get; set; } = true;
        public bool IsLuckySpinEnable   { get; set; } = true;
        public bool IsChestRoomEnable   { get; set; } = true;
        public bool IsIAPEnable         { get; set; } = true;
        public bool IsRemoveAdsEnable   { get; set; } = true;
        public bool IsSuggestionEnable  { get; set; } = true;
        public bool IsBuildingEnable    { get; set; } = true;
        public bool IsVFXEnable         { get; set; } = true;
        public bool IsSFXEnable         { get; set; } = true;

        public void Initialize()
        {
            this.uiTemplateRemoteConfig.GetRemoteConfigBoolValueAsync(IsDailyRewardEnableKey, remoteValue => this.IsDailyRewardEnable = remoteValue, true);
            this.uiTemplateRemoteConfig.GetRemoteConfigBoolValueAsync(IsLeaderboardEnableKey, remoteValue => this.IsLeaderboardEnable = remoteValue, true);
            this.uiTemplateRemoteConfig.GetRemoteConfigBoolValueAsync(IsLuckySpinEnableKey, remoteValue => this.IsLuckySpinEnable     = remoteValue, true);
            this.uiTemplateRemoteConfig.GetRemoteConfigBoolValueAsync(IsChestRoomEnableKey, remoteValue => this.IsChestRoomEnable     = remoteValue, true);
            this.uiTemplateRemoteConfig.GetRemoteConfigBoolValueAsync(IsIAPEnableKey, remoteValue => this.IsIAPEnable                 = remoteValue, true);
            this.uiTemplateRemoteConfig.GetRemoteConfigBoolValueAsync(IsRemoveAdsEnableKey, remoteValue => this.IsRemoveAdsEnable     = remoteValue, true);
            this.uiTemplateRemoteConfig.GetRemoteConfigBoolValueAsync(IsSuggestionEnableKey, remoteValue => this.IsSuggestionEnable   = remoteValue, true);
            this.uiTemplateRemoteConfig.GetRemoteConfigBoolValueAsync(IsBuildingEnableKey, remoteValue => this.IsBuildingEnable       = remoteValue, true);
            this.uiTemplateRemoteConfig.GetRemoteConfigBoolValueAsync(IsVFXEnableKey, remoteValue => this.IsVFXEnable                 = remoteValue, true);
            this.uiTemplateRemoteConfig.GetRemoteConfigBoolValueAsync(IsSFXEnableKey, remoteValue => this.IsSFXEnable                 = remoteValue, true);
        }
    }
}