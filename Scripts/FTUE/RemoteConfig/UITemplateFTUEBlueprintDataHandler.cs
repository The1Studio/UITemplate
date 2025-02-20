namespace TheOneStudio.UITemplate.UITemplate.FTUE.RemoteConfig
{
    using System.Collections.Generic;
    using System.Linq;
    using GameFoundation.DI;
    using GameFoundation.Signals;
    using ServiceImplementation.FireBaseRemoteConfig;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using UnityEngine.Scripting;

    public class UITemplateFTUEBlueprintDataHandler : FTUEConfig, IInitializable
    {
        private readonly UITemplateFTUERemoteConfig remoteConfig;
        private readonly UITemplateFTUEBlueprint    blueprint;
        private readonly SignalBus                  signalBus;

        [Preserve]
        public UITemplateFTUEBlueprintDataHandler(
            UITemplateFTUERemoteConfig remoteConfig,
            UITemplateFTUEBlueprint    blueprint,
            SignalBus                  signalBus
        )
        {
            this.remoteConfig = remoteConfig;
            this.blueprint    = blueprint;
            this.signalBus    = signalBus;
        }

        public UITemplateFTUERecord GetDataById(string id)
        {
            return this.GetValueOrDefault(id);
        }

        public void Initialize()
        {
            this.signalBus.Subscribe<RemoteConfigFetchedSucceededSignal>(this.LoadData);
        }

        private void LoadData(RemoteConfigFetchedSucceededSignal signal)
        {
            #if !UNITY_EDITOR
            this.remoteConfig.LoadData(signal);
            foreach (var record in this.remoteConfig)
            {
                this[record.Key] = record.Value;
            }
            #endif

            foreach (var record in this.blueprint.Where(record => !this.Keys.Contains(record.Key)))
            {
                this[record.Key] = record.Value;
            }
        }
    }
}