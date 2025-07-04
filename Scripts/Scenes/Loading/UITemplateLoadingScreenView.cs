namespace TheOneStudio.UITemplate.UITemplate.Scenes.Loading
{
    using System;
    using System.Diagnostics;
    using BlueprintFlow.BlueprintControlFlow;
    using BlueprintFlow.Signals;
    using Core.AdsServices.Signals;
    using Core.AnalyticServices;
    using Core.AnalyticServices.CommonEvents;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Signals;
    using GameFoundation.Scripts.Utilities;
    using GameFoundation.Scripts.Utilities.ObjectPool;
    using GameFoundation.Signals;
    using ServiceImplementation.AdsServices.ConsentInformation;
    using ServiceImplementation.Configs.Ads;
    using TheOne.Logging;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using TheOneStudio.UITemplate.UITemplate.UserData;
    using TMPro;
    using UnityEngine;
    using UnityEngine.ResourceManagement.AsyncOperations;
    using UnityEngine.ResourceManagement.ResourceProviders;
    using UnityEngine.Scripting;
    using UnityEngine.UI;
    using Utilities.Utils;
    using Debug = UnityEngine.Debug;
    using Object = UnityEngine.Object;

    public class UITemplateLoadingScreenView : BaseView
    {
        [SerializeField] private Slider          LoadingSlider;
        [SerializeField] private TextMeshProUGUI loadingProgressTxt;

        private float visibleProgress;

        public float  Progress    { get; set; }
        public string LoadingText { get; set; }

        private void Update()
        {
            this.visibleProgress = Mathf.Lerp(this.visibleProgress, this.Progress, Time.unscaledDeltaTime * 5f);
            if (this.LoadingSlider is { })
            {
                this.LoadingSlider.value = this.visibleProgress;
            }
            if (this.loadingProgressTxt is { } && this.LoadingText is { })
            {
                this.loadingProgressTxt.text = string.Format(this.LoadingText, Mathf.RoundToInt(this.visibleProgress * 100));
            }
        }

        public UniTask CompleteLoading()
        {
            this.Progress = 1f;
            return UniTask.WaitUntil(() => this.visibleProgress >= .999f);
        }
    }

    [ScreenInfo(nameof(UITemplateLoadingScreenView))]
    public class UITemplateLoadingScreenPresenter : UITemplateBaseScreenPresenter<UITemplateLoadingScreenView>
    {
        private float _loadingProgress;
        private int   loadingSteps;

        private GameObject objectPoolContainer;

        protected virtual string NextSceneName => "1.MainScene";

        private bool IsClosedFirstOpen { get; set; }

        private float LoadingProgress
        {
            get => this._loadingProgress;
            set
            {
                this._loadingProgress = value;
                if (value / this.loadingSteps <= this.View.Progress) return;
                this.View.Progress = value / this.loadingSteps;
            }
        }

        /// <summary>
        /// Please fill loading text with format "Text {0}" where {0} is the value position."
        /// </summary>
        /// <param name="text"></param>
        protected virtual string GetLoadingText()
        {
            return "Loading {0}%";
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.View.LoadingText = this.GetLoadingText();
            this.OpenViewAsync().Forget();
        }

        private Stopwatch loadingStopwatch;

        public override UniTask BindData()
        {
            this.ShowFirstBannerAd(BannerLoadStrategy.Instantiate);
            this.SignalBus.Subscribe<AppOpenFullScreenContentClosedSignal>(this.OnAOAClosedHandler);
            this.SignalBus.Subscribe<AppOpenFullScreenContentFailedSignal>(this.OnAOAClosedHandler);

            this.objectPoolContainer = new(nameof(this.objectPoolContainer));
            Object.DontDestroyOnLoad(this.objectPoolContainer);

            this.LoadingProgress = 0f;
            this.loadingSteps    = 1;

            this.loadingStopwatch = Stopwatch.StartNew();
            UniTask.WhenAll(
                this.CreateObjectPool(AudioService.AudioSourceKey, 3),
                this.PreloadLabelAssets<Sprite>("PreloadSprite"),
                this.PreloadLabelAssets<GameObject>("PreloadGameObject"),
                this.Preload(),
                #if ADMOB || APPLOVIN
                this.WaitForAoa(),
                #endif
                UniTask.WhenAll(
                    this.LoadBlueprint().ContinueWith(this.OnBlueprintLoaded),
                    this.LoadUserData().ContinueWith(this.OnUserDataLoaded)
                ).ContinueWith(this.OnBlueprintAndUserDataLoaded)
            ).ContinueWith(this.OnLoadingCompleted).ContinueWith(this.LoadNextScene).Forget();

            return UniTask.CompletedTask;
        }

        public override void Dispose()
        {
            base.Dispose();
            this.SignalBus.Unsubscribe<AppOpenFullScreenContentClosedSignal>(this.OnAOAClosedHandler);
            this.SignalBus.Unsubscribe<AppOpenFullScreenContentFailedSignal>(this.OnAOAClosedHandler);
        }

        private void OnAOAClosedHandler()
        {
            this.IsClosedFirstOpen = true;
        }

        protected virtual async UniTask LoadNextScene()
        {
            SceneDirector.CurrentSceneName = this.NextSceneName;

            await this.View.CompleteLoading();

            var stopWatch = Stopwatch.StartNew();

            this.SignalBus.Fire<StartLoadingNewSceneSignal>();
            await this.TrackProgress(this.LoadSceneAsync());
            this.SignalBus.Fire<FinishLoadingNewSceneSignal>();

            stopWatch.Stop();
            this.Logger.Info("Loading Main Scene Time: " + stopWatch.ElapsedMilliseconds + "ms");
            this.analyticServices.Track(new CustomEvent()
            {
                EventName = "LoadingMainSceneTime",
                EventProperties = new()
                {
                    { "timeMilis", stopWatch.ElapsedMilliseconds },
                },
            });

            this.ShowFirstBannerAd(BannerLoadStrategy.AfterLoading);
            this.OnAfterLoading();

            this.loadingStopwatch.Stop();
            this.Logger.Info("Game Loading Time: " + this.loadingStopwatch.ElapsedMilliseconds + "ms");
            this.analyticServices.Track(new CustomEvent()
            {
                EventName = "GameLoadingTime",
                EventProperties = new()
                {
                    { "timeMilis", this.loadingStopwatch.ElapsedMilliseconds },
                },
            });

            await Resources.UnloadUnusedAssets();
            GCUtils.ForceGCWithLOH();
        }

        protected virtual void ShowFirstBannerAd(BannerLoadStrategy strategy)
        {
            if (strategy != this.adService.BannerLoadStrategy) return;
            this.adService.ShowBannerAd();
        }

        protected virtual AsyncOperationHandle<SceneInstance> LoadSceneAsync()
        {
            return this.gameAssets.LoadSceneAsync(this.NextSceneName);
        }

        private UniTask LoadBlueprint()
        {
            this.TrackProgress<LoadBlueprintDataProgressSignal>();
            this.TrackProgress<ReadBlueprintProgressSignal>();

            return this.blueprintManager.LoadBlueprint();
        }

        private UniTask LoadUserData()
        {
            return this.TrackProgress(this.userDataManager.LoadUserData());
        }

        private UniTask WaitForAoa()
        {
            var startWaitingAoaTime = DateTime.Now;

            // sometimes AOA delay when shown, we need 0.5s to wait for it
            return this.TrackProgress(UniTask.WaitUntil(() =>
                    ((this.uiTemplateAdServiceWrapper.IsOpenedAOAFirstOpen && this.IsClosedFirstOpen)
                        || (!this.uiTemplateAdServiceWrapper.IsOpenedAOAFirstOpen && (DateTime.Now - startWaitingAoaTime).TotalSeconds > this.uiTemplateAdServiceWrapper.LoadingTimeToShowAOA))
                    && !this.consentInformation.IsRequestingConsent()
                )
            );
        }

        // Preload assets by label
        protected async UniTask PreloadLabelAssets<T>(string label)
        {
            await UniTask.WhenAll((await this.gameAssets.PreLoadAssetsByLabelAsync<T>(label, this.NextSceneName)).Select(this.TrackProgress));
        }

        protected virtual UniTask Preload()
        {
            return UniTask.CompletedTask;
        }

        protected virtual UniTask OnBlueprintLoaded()
        {
            return UniTask.CompletedTask;
        }

        protected virtual UniTask OnUserDataLoaded()
        {
            return UniTask.CompletedTask;
        }

        protected virtual UniTask OnBlueprintAndUserDataLoaded()
        {
            return UniTask.CompletedTask;
        }

        protected virtual UniTask OnLoadingCompleted()
        {
            return UniTask.CompletedTask;
        }

        protected virtual void OnAfterLoading() { }

        protected UniTask PreloadAssets<T>(params object[] keys)
        {
            return UniTask.WhenAll(this.gameAssets.PreloadAsync<T>(this.NextSceneName, keys).Select(this.TrackProgress));
        }

        protected UniTask CreateObjectPool(string prefabName, int initialPoolSize = 1)
        {
            return this.TrackProgress(this.objectPoolManager.CreatePool(prefabName, initialPoolSize, this.objectPoolContainer));
        }

        protected UniTask TrackProgress(UniTask task)
        {
            ++this.loadingSteps;
            return task.ContinueWith(() => ++this.LoadingProgress);
        }

        protected UniTask<T> TrackProgress<T>(AsyncOperationHandle<T> handle)
        {
            ++this.loadingSteps;
            var localLoadingProgress = 0f;

            void UpdateProgress(float progress)
            {
                this.LoadingProgress += progress - localLoadingProgress;
                localLoadingProgress =  progress;
            }

            return handle.ToUniTask(Progress.CreateOnlyValueChanged<float>(UpdateProgress))
                .ContinueWith(result =>
                {
                    UpdateProgress(1f);
                    return result;
                });
        }

        protected void TrackProgress<T>() where T : IProgressPercent
        {
            ++this.loadingSteps;
            var localLoadingProgress = 0f;

            this.SignalBus.Subscribe<T>(UpdateProgress);

            void UpdateProgress(T progress)
            {
                this.LoadingProgress += progress.Percent - localLoadingProgress;
                localLoadingProgress =  progress.Percent;
                if (progress.Percent >= 1f) this.SignalBus.Unsubscribe<T>(UpdateProgress);
            }
        }

        #region Inject

        protected readonly UITemplateAdServiceWrapper adService;
        protected readonly BlueprintReaderManager     blueprintManager;
        protected readonly UserDataManager            userDataManager;
        protected readonly IGameAssets                gameAssets;
        private readonly   ObjectPoolManager          objectPoolManager;
        private readonly   UITemplateAdServiceWrapper uiTemplateAdServiceWrapper;
        private readonly   IAnalyticServices          analyticServices;
        private readonly   IConsentInformation        consentInformation;

        [Preserve]
        protected UITemplateLoadingScreenPresenter(
            SignalBus                  signalBus,
            ILoggerManager             loggerManager,
            UITemplateAdServiceWrapper adService,
            BlueprintReaderManager     blueprintManager,
            UserDataManager            userDataManager,
            IGameAssets                gameAssets,
            ObjectPoolManager          objectPoolManager,
            UITemplateAdServiceWrapper uiTemplateAdServiceWrapper,
            IAnalyticServices          analyticServices,
            IConsentInformation        consentInformation
        ) : base(signalBus, loggerManager)
        {
            this.adService                  = adService;
            this.blueprintManager           = blueprintManager;
            this.userDataManager            = userDataManager;
            this.gameAssets                 = gameAssets;
            this.objectPoolManager          = objectPoolManager;
            this.uiTemplateAdServiceWrapper = uiTemplateAdServiceWrapper;
            this.analyticServices           = analyticServices;
            this.consentInformation         = consentInformation;
        }

        #endregion
    }
}