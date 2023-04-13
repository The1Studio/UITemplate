namespace TheOneStudio.UITemplate.UITemplate.Installers
{
    using TheOneStudio.UITemplate.UITemplate.FTUE.Signal;
    using TheOneStudio.UITemplate.UITemplate.Scripts.Signals;
    using TheOneStudio.UITemplate.UITemplate.Services.RewardHandle;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using Zenject;

    public class UITemplateInitSignalInstaller : Installer<UITemplateInitSignalInstaller>
    {
        public override void InstallBindings()
        {
            //FTUE
            this.Container.DeclareSignal<FTUEButtonClickSignal>();
            this.Container.DeclareSignal<FTUEManualTriggerSignal>();
            this.Container.DeclareSignal<TutorialCompletionSignal>();

            //Signal
            this.Container.DeclareSignal<RewardedAdEligibleSignal>();
            this.Container.DeclareSignal<RewardedAdCalledSignal>();
            this.Container.DeclareSignal<RewardedAdOfferSignal>();
            this.Container.DeclareSignal<UpdateCurrencySignal>();
            this.Container.DeclareSignal<LevelStartedSignal>();
            this.Container.DeclareSignal<LevelEndedSignal>();
            this.Container.DeclareSignal<LevelSkippedSignal>();
            this.Container.DeclareSignal<InterstitialAdCalledSignal>();
            this.Container.DeclareSignal<InterstitialAdEligibleSignal>();
            this.Container.DeclareSignal<FirebaseInitializeSucceededSignal>();
            this.Container.DeclareSignal<ScaleDecoration2DItem>();
            this.Container.DeclareSignal<UITemplateAddRewardsSignal>();
        }
    }
}