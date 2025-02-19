namespace TheOneStudio.UITemplate.UITemplate.FTUE.RemoteConfig
{
    using System.Collections.Generic;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using UnityEngine.Scripting;

    public class UITemplateFTUEBlueprintDataHandler : FTUEConfig
    {
        private readonly UITemplateFTUERemoteConfig remoteConfig;
        private readonly UITemplateFTUEBlueprint    blueprint;

        [Preserve]
        public UITemplateFTUEBlueprintDataHandler(
            UITemplateFTUERemoteConfig remoteConfig,
            UITemplateFTUEBlueprint    blueprint
        )
        {
            this.remoteConfig = remoteConfig;
            this.blueprint    = blueprint;
        }

        public UITemplateFTUERecord GetDataById(string id)
        {
            if (this.ContainsKey(id)) return this.GetValueOrDefault(id);
            if (this.remoteConfig.TryGetValue(id, out var byId))
            {
                this.Add(byId.Id, byId);
                return byId;
            }
            if (this.blueprint.TryGetValue(id, out var byIdBlueprint))
            {
                this.Add(byIdBlueprint.Id, byIdBlueprint);
                return byIdBlueprint;
            }
            return null;
        }
    }
}