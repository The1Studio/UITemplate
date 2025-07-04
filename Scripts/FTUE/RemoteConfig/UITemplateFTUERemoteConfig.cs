﻿namespace TheOneStudio.UITemplate.UITemplate.FTUE.RemoteConfig
{
    using GameFoundation.Signals;
    using Newtonsoft.Json;
    using ServiceImplementation.FireBaseRemoteConfig;
    using UnityEngine.Scripting;

    public class UITemplateFTUERemoteConfig : FTUEConfig
    {
        private const string RemoteConfigKey = "UITemplateFTUE";

        private readonly SignalBus signalBus;

        [Preserve]
        public UITemplateFTUERemoteConfig(SignalBus signalBus)
        {
            this.signalBus = signalBus;
        }

        public void LoadData(RemoteConfigFetchedSucceededSignal signal)
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