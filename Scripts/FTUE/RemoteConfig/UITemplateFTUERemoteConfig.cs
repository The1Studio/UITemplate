namespace TheOneStudio.UITemplate.UITemplate.FTUE.RemoteConfig
{
    using GameFoundation.DI;
    using GameFoundation.Signals;
    using Newtonsoft.Json;
    using ServiceImplementation.FireBaseRemoteConfig;
    using TheOneStudio.UITemplate.UITemplate.Models.Core.Interface;

    public class UITemplateFTUERemoteConfig : FTUEConfig, IInitializable, IUITemplateBlueprintRemoteConfig
    {
        private const string RemoteConfigKey = "UITemplateFTUE";

        private readonly SignalBus signalBus;

        public UITemplateFTUERemoteConfig(SignalBus signalBus)
        {
            this.signalBus = signalBus;
        }

        public void Initialize()
        {
            this.signalBus.Subscribe<RemoteConfigFetchedSucceededSignal>(this.LoadData);
        }

        private void LoadData(RemoteConfigFetchedSucceededSignal signal)
        {
            var remoteConfigValue = signal.RemoteConfig.GetRemoteConfigStringValue(RemoteConfigKey);
            if (string.IsNullOrEmpty(remoteConfigValue))
            {
                return;
            }
            var records = JsonConvert.DeserializeObject<FTUEConfig>(remoteConfigValue);
            foreach (var record in records)
            {
                this[record.Key] = record.Value;
            }
        }
    }
}