namespace TheOneStudio.UITemplate.UITemplate.Installers
{
    using ServiceImplementation.FireBaseRemoteConfig;
    using TheOneStudio.UITemplate.UITemplate.Scripts.Signals;
    using TheOneStudio.UITemplate.UITemplate.Services.RewardHandle;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using Zenject;

    /// <summary>
    /// Installer for declaring signals.
    /// We use this installer to declare signals in the container.
    /// We don't you reflection to declare signals.
    /// </summary>
    public class UITemplateDeclareSignalInstaller : Installer<UITemplateDeclareSignalInstaller>
    {
        public override void InstallBindings()
        {
            //FTUE
            this.Container.DeclareSignal<TutorialCompletionSignal>();

            //Signal
            this.Container.DeclareSignal<UpdateCurrencySignal>();
            this.Container.DeclareSignal<LevelStartedSignal>();
            this.Container.DeclareSignal<LevelEndedSignal>();
            this.Container.DeclareSignal<LevelSkippedSignal>();
            this.Container.DeclareSignal<RemoteConfigInitializeSucceededSignal>();
            this.Container.DeclareSignal<ScaleDecoration2DItem>();
            this.Container.DeclareSignal<UITemplateAddRewardsSignal>();
            this.Container.DeclareSignal<BuildingOnMouseDownSignal>();
            this.Container.DeclareSignal<UITemplateUnlockBuildingSignal>();
        }
    }
}