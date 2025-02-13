namespace TheOneStudio.UITemplate.UITemplate.FTUE.RemoteConfig
{
    using GameFoundation.DI;
    using ServiceImplementation.RemoteConfig;

    public class UITemplateFTUERemoteConfig : IInitializable
    {
        private const string RemoteConfigKey = "UITemplateFTUE";

        private readonly IInGameRemoteConfig remoteConfig;

        public UITemplateFTUERemoteConfig(IInGameRemoteConfig remoteConfig)
        {
            this.remoteConfig = remoteConfig;
        }

        public void Initialize()
        {
            var remoteConfigValue = this.remoteConfig.GetRemoteConfigStringValue(RemoteConfigKey);
        }
    }
}