#if GDK_VCONTAINER
#nullable enable
namespace TheOneStudio.UITemplate
{
    using GameFoundation.Signals;
    using ServiceImplementation.FireBaseRemoteConfig;
    using TheOneStudio.UITemplate.UITemplate.Creative.CheatLevel;
    using TheOneStudio.UITemplate.UITemplate.FTUE.Signal;
    using TheOneStudio.UITemplate.UITemplate.Others.StateMachine.Signals;
    using TheOneStudio.UITemplate.UITemplate.Scripts.Signals;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using VContainer;

    public static class UITemplateSignalVContainer
    {
        public static void DeclareUITemplateSignals(this IContainerBuilder builder)
        {
            //FTUE
            builder.DeclareSignal<TutorialCompletionSignal>();
            builder.DeclareSignal<FTUEButtonClickSignal>();
            builder.DeclareSignal<FTUEDoActionSignal>();
            builder.DeclareSignal<FTUETriggerSignal>();

            //Signal
            builder.DeclareSignal<OnNotEnoughCurrencySignal>();
            builder.DeclareSignal<OnUpdateCurrencySignal>();
            builder.DeclareSignal<OnFinishCurrencyAnimationSignal>();
            builder.DeclareSignal<LevelStartedSignal>();
            builder.DeclareSignal<LevelEndedSignal>();
            builder.DeclareSignal<LevelSkippedSignal>();
            builder.DeclareSignal<RemoteConfigFetchedSucceededSignal>();
            builder.DeclareSignal<OnRemoveAdsSucceedSignal>();
            builder.DeclareSignal<UITemplateOnUpdateBannerStateSignal>();

            //State machine
            builder.DeclareSignal<OnStateEnterSignal>();
            builder.DeclareSignal<OnStateExitSignal>();

            //Creative, Cheat
            builder.DeclareSignal<ChangeLevelCreativeSignal>();
        }
    }
}
#endif