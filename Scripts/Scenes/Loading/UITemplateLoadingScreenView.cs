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
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Signals;
    using GameFoundation.Scripts.Utilities.ObjectPool;
    using GameFoundation.Scripts.Utilities.UserData;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using TheOneStudio.UITemplate.UITemplate.UserData;
    using UnityEngine;
    using UnityEngine.ResourceManagement.AsyncOperations;
    using UnityEngine.ResourceManagement.ResourceProviders;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;
    using Zenject;
    using Object = UnityEngine.Object;

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

        #region Inject

        private readonly BlueprintReaderManager     blueprintReaderManager;
        private readonly SceneDirector              sceneDirector;
        private readonly IAOAAdService              aoaAdService;
        private readonly UITemplateAdServiceWrapper uiTemplateAdServiceWrapper;
        private readonly UserDataManager            userDataManager;
        private readonly ObjectPoolManager          objectPoolManager;
        private readonly IGameAssets                gameAssets;

        public UITemplateLoadingScreenPresenter(SignalBus                  signalBus, BlueprintReaderManager blueprintReaderManager, SceneDirector sceneDirector, IAOAAdService aoaAdService,
                                                UITemplateAdServiceWrapper uiTemplateAdServiceWrapper, UserDataManager userDataManager, ObjectPoolManager objectPoolManager, IGameAssets gameAssets) : base(signalBus)
        {
            this.blueprintReaderManager     = blueprintReaderManager;
            this.sceneDirector              = sceneDirector;
            this.aoaAdService               = aoaAdService;
            this.uiTemplateAdServiceWrapper = uiTemplateAdServiceWrapper;
            this.userDataManager            = userDataManager;
            this.objectPoolManager          = objectPoolManager;
            this.gameAssets                 = gameAssets;
        }

        #endregion

        private Dictionary<object, float> loadingTypeToProgressPercent;
        private DateTime                startedLoadingTime;
        private DateTime                startedShowingAOATime;
        private bool                    isUserDataLoaded;
        private bool                    isBlueprintDataLoaded;
        private bool                    isLoaded;

        private GameObject                          poolContainer;
        private List<UniTask>                       creatingPoolTask = new();
        private AsyncOperationHandle<SceneInstance> nextSceneLoadingTask;

        protected virtual string NextSceneName => "1.UITemplateMainScene";

        protected virtual float MinimumLoadingBlueprintTime { get; set; } //seconds
        protected virtual int   MinLoadingTime              => 2;

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.OpenViewAsync().Forget();

            this.CreatePoolContainer();
            this.blueprintReaderManager.LoadBlueprint().Forget();
        }

        public override UniTask BindData()
        {
            if (this.uiTemplateAdServiceWrapper.IsRemovedAds)
            {
                this.MinimumLoadingBlueprintTime = this.MinLoadingTime;
            }
            else
            {
                this.MinimumLoadingBlueprintTime = Math.Max(this.aoaAdService.LoadingTimeToShowAOA, this.MinLoadingTime);
            }
            
            this.startedLoadingTime = DateTime.Now;
            this.isUserDataLoaded   = false;
            this.SignalBus.Subscribe<LoadBlueprintDataProgressSignal>(this.OnLoadProgress);
            this.SignalBus.Subscribe<ReadBlueprintProgressSignal>(this.OnLoadProgress);
            this.SignalBus.Subscribe<LoadBlueprintDataSucceedSignal>(this.OnLoadBlueprintDataSucceed);
            this.SignalBus.Subscribe<AppOpenFullScreenContentOpenedSignal>(this.OnAppOpenFullScreenContentOpened);
            this.SignalBus.Subscribe<UserDataLoadedSignal>(this.OnUserDataLoaded);

            this.loadingTypeToProgressPercent = new Dictionary<object, float> { { typeof(LoadBlueprintDataProgressSignal), 0f }, { typeof(ReadBlueprintProgressSignal), 0f }, {typeof(SceneInstance), 0f} };

            this.ShowLoadingProgress(LoadingBlueprintStepName);
            this.userDataManager.LoadUserData();
            return UniTask.CompletedTask;
        }
        
        protected virtual void OnLoadBlueprintDataSucceed()
        {
            this.isBlueprintDataLoaded = true;
            if (this.isUserDataLoaded)
            {
                this.OnUserDataAndBlueprintLoaded();
            }
            this.nextSceneLoadingTask  = this.gameAssets.LoadSceneAsync(this.NextSceneName, LoadSceneMode.Single, false);
            this.UpdateNextSceneLoadingProgress();
        }

        private async void UpdateNextSceneLoadingProgress()
        {
            this.loadingTypeToProgressPercent[typeof(SceneInstance)] = this.nextSceneLoadingTask.PercentComplete;
            if (this.nextSceneLoadingTask.IsDone) return;
            await UniTask.Yield();
            this.UpdateNextSceneLoadingProgress();
        }

        public override void Dispose()
        {
            base.Dispose();
            this.SignalBus.Unsubscribe<LoadBlueprintDataProgressSignal>(this.OnLoadProgress);
            this.SignalBus.Unsubscribe<ReadBlueprintProgressSignal>(this.OnLoadProgress);
            this.SignalBus.Unsubscribe<LoadBlueprintDataSucceedSignal>(this.OnLoadBlueprintDataSucceed);
            this.SignalBus.Unsubscribe<AppOpenFullScreenContentOpenedSignal>(this.OnAppOpenFullScreenContentOpened);
            this.SignalBus.Unsubscribe<UserDataLoadedSignal>(this.OnUserDataLoaded);
        }

        private void OnAppOpenFullScreenContentOpened() { this.MinimumLoadingBlueprintTime = this.MinLoadingTime; }

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
            await UniTask.WhenAll(this.creatingPoolTask);
            
            SceneDirector.CurrentSceneName = this.NextSceneName;
            this.SignalBus.Fire<StartLoadingNewSceneSignal>();
            var screenInstance = await this.nextSceneLoadingTask;
            await screenInstance.ActivateAsync();
            _ = Resources.UnloadUnusedAssets();
            this.SignalBus.Fire<FinishLoadingNewSceneSignal>();
            
            this.uiTemplateAdServiceWrapper.ShowBannerAd();
        }

        protected virtual bool IsLoadingFinished() => this.isLoaded && this.isUserDataLoaded;

        private void OnLoadProgress(IProgressPercent obj) { this.loadingTypeToProgressPercent[obj.GetType()] = obj.Percent; }

        private void OnUserDataLoaded()
        {
            this.isUserDataLoaded = true;
            if (this.isBlueprintDataLoaded)
            {
                this.OnUserDataAndBlueprintLoaded();
            }
        }

        private void CreatePoolContainer()
        {
            this.poolContainer = new GameObject("InLoadingObjectPoolContainer");
            Object.DontDestroyOnLoad(this.poolContainer);
        }
        
        /// <summary>
        /// Create a object pool for asset and add it to pool container
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="count"></param>
        protected void CreatePoolDontDestroy(string asset, int count = 1)
        {
            this.creatingPoolTask.Add(this.objectPoolManager.CreatePool(asset, count, this.poolContainer.gameObject).AsUniTask());
        }

        /// <summary>
        /// User to preload asset for next scene (1.MainScene for example)
        /// </summary>
        /// <param name="keys">addressable key</param>
        /// <typeparam name="T">Type of assets</typeparam>
        protected void PreloadAssetForNextScene<T>(params object[] keys)
        {
            var progress = this.gameAssets.PreloadAsync<T>(this.NextSceneName, keys);
            this.loadingTypeToProgressPercent.Add(keys.GetHashCode(), 0);
            this.OnUpdatePreloadAssetProgress(progress, keys.GetHashCode());
        }

        private async void OnUpdatePreloadAssetProgress<T>(List<AsyncOperationHandle<T>> operationHandles, object key)
        {
            var progress = operationHandles.Average(handle => handle.PercentComplete);
            this.loadingTypeToProgressPercent[key] = progress;
            if (operationHandles.All(operationHandle => operationHandle.IsDone)) return;
            await UniTask.Yield();
            this.OnUpdatePreloadAssetProgress(operationHandles, key);
        }

        /// <summary>
        /// Was called when user data and blueprint were loaded
        /// </summary>
        protected virtual void OnUserDataAndBlueprintLoaded()
        {
        }
    }
}