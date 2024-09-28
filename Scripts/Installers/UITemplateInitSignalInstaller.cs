#if GDK_ZENJECT
namespace TheOneStudio.UITemplate.UITemplate.Installers
{
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Creative.CheatLevel;
    using ServiceImplementation.FireBaseRemoteConfig;
    using TheOneStudio.UITemplate.UITemplate.FTUE.Signal;
    using TheOneStudio.UITemplate.UITemplate.Others.StateMachine.Signals;
    using TheOneStudio.UITemplate.UITemplate.Scripts.Signals;
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
            this.Container.DeclareSignal<FTUEButtonClickSignal>();
            this.Container.DeclareSignal<FTUEDoActionSignal>();
            this.Container.DeclareSignal<FTUETriggerSignal>();

            //Signal
            this.Container.DeclareSignal<OnNotEnoughCurrencySignal>();
            this.Container.DeclareSignal<OnUpdateCurrencySignal>();
            this.Container.DeclareSignal<OnFinishCurrencyAnimationSignal>();
            this.Container.DeclareSignal<LevelStartedSignal>();
            this.Container.DeclareSignal<LevelEndedSignal>();
            this.Container.DeclareSignal<LevelSkippedSignal>();
            this.Container.DeclareSignal<RemoteConfigFetchedSucceededSignal>();
            this.Container.DeclareSignal<OnRemoveAdsSucceedSignal>();
            this.Container.DeclareSignal<UITemplateOnUpdateBannerStateSignal>();

            //State machine
            this.Container.DeclareSignal<OnStateEnterSignal>();
            this.Container.DeclareSignal<OnStateExitSignal>();

            //Creative, Cheat
            this.Container.DeclareSignal<ChangeLevelCreativeSignal>();
        }
    }
}
#endif