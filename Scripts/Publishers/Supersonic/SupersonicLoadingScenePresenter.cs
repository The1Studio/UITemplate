namespace UITemplate.Scripts.Publishers.Supersonic
{
    using BlueprintFlow.BlueprintControlFlow;
    using Core.AnalyticServices;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.Utilities.LogService;
    using GameFoundation.Scripts.Utilities.ObjectPool;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Loading;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using TheOneStudio.UITemplate.UITemplate.UserData;
#if SUPERSONIC_WISDOM
    using SupersonicWisdomSDK;
#endif

    public class SupersonicLoadingScenePresenter : UITemplateLoadingScreenPresenter
    {
        protected SupersonicLoadingScenePresenter(SignalBus signalBus,
                                                ILogService logger,
                                                UITemplateAdServiceWrapper adService,
                                                BlueprintReaderManager blueprintManager,
                                                UserDataManager userDataManager,
                                                IGameAssets gameAssets,
                                                ObjectPoolManager objectPoolManager,
                                                UITemplateAdServiceWrapper uiTemplateAdServiceWrapper,
                                                IAnalyticServices analyticServices)
            : base(signalBus, logger, adService, blueprintManager, userDataManager, gameAssets, objectPoolManager, uiTemplateAdServiceWrapper, analyticServices)
        {
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.InitializeSuperSonicWisdom();
        }
        
        private void InitializeSuperSonicWisdom()
        {
#if SUPERSONIC_WISDOM
            var tcs = new UniTaskCompletionSource();
            SupersonicWisdom.Api.AddOnReadyListener(() => tcs.TrySetResult());
            SupersonicWisdom.Api.Initialize();
            this.TrackProgress(tcs.Task);
#endif
        }
    }
}