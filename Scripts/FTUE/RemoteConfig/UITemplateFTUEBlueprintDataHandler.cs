namespace TheOneStudio.UITemplate.UITemplate.FTUE.RemoteConfig
{
    using GameFoundation.DI;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;

    public class UITemplateFTUEBlueprintDataHandler : IInitializable
    {
        private readonly UITemplateFTUERemoteConfig remoteConfig;
        private readonly UITemplateFTUEBlueprint    blueprint;

        public UITemplateFTUEBlueprintDataHandler(
            UITemplateFTUERemoteConfig remoteConfig,
            UITemplateFTUEBlueprint    blueprint
        )
        {
            this.remoteConfig = remoteConfig;
            this.blueprint    = blueprint;
        }

        public void Initialize()
        {
        }
    }
}