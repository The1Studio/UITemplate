namespace TheOneStudio.UITemplate.UITemplate.Scenes.FeaturesConfig
{
    using TheOneStudio.UITemplate.UITemplate.Interfaces;
    using Zenject;

    public class UITemplateFeatureConfig : IInitializable
    {
        private const string IsDailyRewardEnableKey = "IsDailyRewardEnable";
        private const string IsLeaderboardEnableKey = "IsLeaderboardEnable";
        private const string IsChestRoomEnableKey   = "IsChestRoomEnable";
        private const string IsIAPEnableKey         = "IsIAPEnable";
        private const string IsSuggestionEnableKey  = "IsSuggestionEnable";
        
        private readonly IUITemplateRemoteConfig uiTemplateRemoteConfig;

        public UITemplateFeatureConfig(IUITemplateRemoteConfig uiTemplateRemoteConfig)
        {
            this.uiTemplateRemoteConfig = uiTemplateRemoteConfig;
        }
        
        public bool IsDailyRewardEnable { get; set; }
        public bool IsLeaderboardEnable { get; set; }
        public bool IsChestRoomEnable   { get; set; }
        public bool IsIAPEnable         { get; set; }
        public bool IsSuggestionEnable  { get; set; }
        

        public void Initialize()
        {
            this.uiTemplateRemoteConfig.GetRemoteConfigBoolValueAsync(IsDailyRewardEnableKey, remoteValue => this.IsDailyRewardEnable = remoteValue, true);
            this.uiTemplateRemoteConfig.GetRemoteConfigBoolValueAsync(IsLeaderboardEnableKey, remoteValue => this.IsLeaderboardEnable = remoteValue, true);
            this.uiTemplateRemoteConfig.GetRemoteConfigBoolValueAsync(IsChestRoomEnableKey, remoteValue => this.IsChestRoomEnable     = remoteValue, true);
            this.uiTemplateRemoteConfig.GetRemoteConfigBoolValueAsync(IsIAPEnableKey, remoteValue => this.IsIAPEnable                 = remoteValue, true);
            this.uiTemplateRemoteConfig.GetRemoteConfigBoolValueAsync(IsSuggestionEnableKey, remoteValue => this.IsSuggestionEnable   = remoteValue, true);
        }
    }
}