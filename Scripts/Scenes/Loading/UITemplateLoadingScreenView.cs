namespace TheOneStudio.UITemplate.UITemplate.Scenes.Loading
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BlueprintFlow.BlueprintControlFlow;
    using BlueprintFlow.Signals;
    using Core.AdsServices;
    using Core.AdsServices.Signals;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.Utilities.ObjectPool;
    using GameFoundation.Scripts.Utilities.UserData;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using TheOneStudio.UITemplate.UITemplate.UserData;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateLoadingScreenView : BaseView
    {
        [SerializeField] private Slider LoadingSlider;

        public void SetLoadingProgressValue(float value)
        {
            if (this.LoadingSlider == null) return;

            this.LoadingSlider.value = value;
        }
    }

    [ScreenInfo(nameof(UITemplateLoadingScreenView))]
    public class UITemplateLoadingScreenPresenter : UITemplateBaseScreenPresenter<UITemplateLoadingScreenView>
    {
        private const string LoadingBlueprintStepName = "Loading static data...";
        private const int    MinLoadingTime           = 1;

        #region Inject

        private readonly BlueprintReaderManager     blueprintReaderManager;
        private readonly SceneDirector              sceneDirector;
        private readonly IAOAAdService              aoaAdService;
        private readonly UITemplateAdServiceWrapper uiTemplateAdServiceWrapper;
        private readonly UserDataManager            userDataManager;

        public UITemplateLoadingScreenPresenter(SignalBus                  signalBus, BlueprintReaderManager blueprintReaderManager, SceneDirector sceneDirector, IAOAAdService aoaAdService,
                                                UITemplateAdServiceWrapper uiTemplateAdServiceWrapper, UserDataManager userDataManager) : base(signalBus)
        {
            this.blueprintReaderManager     = blueprintReaderManager;
            this.sceneDirector              = sceneDirector;
            this.aoaAdService               = aoaAdService;
            this.uiTemplateAdServiceWrapper = uiTemplateAdServiceWrapper;
            this.userDataManager            = userDataManager;
        }

        #endregion

        private Dictionary<Type, float> loadingTypeToProgressPercent;
        private DateTime                startedLoadingTime;
        private DateTime                startedShowingAOATime;
        private bool                    isUserDataLoaded;
        private bool                    isLoaded;

        private static GameObject poolContainer;

        protected virtual string NextSceneName               => "1.UITemplateMainScene";
        
        protected virtual float  MinimumLoadingBlueprintTime { get; set; } //seconds

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.OpenViewAsync().Forget();

            this.CreatePoolContainer();
        }

        public override UniTask BindData()
        {
            if (this.uiTemplateAdServiceWrapper.IsRemovedAds)
            {
                this.MinimumLoadingBlueprintTime = MinLoadingTime;
            }
            else
            {
                this.MinimumLoadingBlueprintTime = this.aoaAdService.LoadingTimeToShowAOA;
            }

            this.startedLoadingTime = DateTime.Now;
            this.isUserDataLoaded   = false;
            this.SignalBus.Subscribe<LoadBlueprintDataProgressSignal>(this.OnLoadProgress);
            this.SignalBus.Subscribe<ReadBlueprintProgressSignal>(this.OnLoadProgress);
            this.SignalBus.Subscribe<AppOpenFullScreenContentOpenedSignal>(this.OnAppOpenFullScreenContentOpened);
            this.SignalBus.Subscribe<UserDataLoadedSignal>(this.OnUserDataLoaded);

            this.loadingTypeToProgressPercent = new Dictionary<Type, float> { { typeof(LoadBlueprintDataProgressSignal), 0f }, { typeof(ReadBlueprintProgressSignal), 0f } };

            this.ShowLoadingProgress(LoadingBlueprintStepName);
            this.blueprintReaderManager.LoadBlueprint().Forget();
            this.userDataManager.LoadUserData();
            return UniTask.CompletedTask;
        }

        public override void Dispose()
        {
            base.Dispose();
            this.SignalBus.Unsubscribe<LoadBlueprintDataProgressSignal>(this.OnLoadProgress);
            this.SignalBus.Unsubscribe<ReadBlueprintProgressSignal>(this.OnLoadProgress);
            this.SignalBus.Unsubscribe<AppOpenFullScreenContentOpenedSignal>(this.OnAppOpenFullScreenContentOpened);
            this.SignalBus.Unsubscribe<UserDataLoadedSignal>(this.OnUserDataLoaded);
        }

        private void OnAppOpenFullScreenContentOpened() { this.MinimumLoadingBlueprintTime = MinLoadingTime; }

        private async void ShowLoadingProgress(string loadingContent)
        {
            var progressInView = 0f;

            while (progressInView < 1f)
            {
                if (this.aoaAdService.IsShowingAd)
                {
                    if (this.startedShowingAOATime == default) this.startedShowingAOATime = DateTime.Now;

                    await UniTask.WaitUntil(() => !this.aoaAdService.IsShowingAd);
                    this.startedLoadingTime = this.startedLoadingTime.Add(DateTime.Now - this.startedShowingAOATime);
                }

                var currentProgress       = this.loadingTypeToProgressPercent.Values.ToList().Average();
                var maximumLoadingPercent = (float)(DateTime.Now - this.startedLoadingTime).TotalSeconds / this.MinimumLoadingBlueprintTime;
                progressInView = Mathf.Min(currentProgress, maximumLoadingPercent);
                this.View.SetLoadingProgressValue(progressInView);
                await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
            }
            this.isLoaded = true;
            
            await UniTask.WaitUntil(this.IsLoadingFinished);

            await this.sceneDirector.LoadSingleSceneAsync(this.NextSceneName);
            this.uiTemplateAdServiceWrapper.ShowBannerAd();
        }
        
        protected virtual bool IsLoadingFinished() => this.isLoaded && this.isUserDataLoaded;

        private void OnLoadProgress(IProgressPercent obj) { this.loadingTypeToProgressPercent[obj.GetType()] = obj.Percent; }

        private void OnUserDataLoaded() { this.isUserDataLoaded = true; }

        private void CreatePoolContainer()
        {
            poolContainer = new GameObject("ObjectPoolContainer");
            GameObject.DontDestroyOnLoad(poolContainer);
        }

        public void CreatePoolDontDestroy(GameObject asset, int count = 1)
        {
            if (poolContainer == null)
            {
                this.CreatePoolContainer();
            }
            asset.CreatePool(count, poolContainer.gameObject);
        }
    }
}