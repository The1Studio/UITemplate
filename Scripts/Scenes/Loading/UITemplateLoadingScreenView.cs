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
    using DG.Tweening;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Signals;
    using GameFoundation.Scripts.Utilities;
    using GameFoundation.Scripts.Utilities.ObjectPool;
    using ServiceImplementation.Configs;
    using ServiceImplementation.Configs.Ads;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using TheOneStudio.UITemplate.UITemplate.UserData;
    using TMPro;
    using UnityEngine;
    using UnityEngine.ResourceManagement.AsyncOperations;
    using UnityEngine.ResourceManagement.ResourceProviders;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;
    using Zenject;
    using Object = UnityEngine.Object;

    public class UITemplateLoadingScreenView : BaseView
    {
        [SerializeField] private Slider          LoadingSlider;
        [SerializeField] private TextMeshProUGUI loadingProgressTxt;

        private            Tween  tween;
        private            float  trueProgress;
        protected string loadingText = "Loading {0}";

        private void Start() { this.LoadingSlider.value = 0f; }

        public void SetProgress(float progress)
        {
            if (!this.LoadingSlider) return;
            if (progress <= this.trueProgress) return;
            this.tween.Kill();
            this.tween = DOTween.To(
                getter: () => this.LoadingSlider.value,
                setter: value =>
                {
                    this.LoadingSlider.value     = value;
                    if (this.loadingProgressTxt != null)
                        this.loadingProgressTxt.text = string.Format(this.loadingText, value * 100); 
                },
                endValue: this.trueProgress = progress,
                duration: 0.5f
            );
        }

        public UniTask CompleteLoading()
        {
            this.SetProgress(1f);

            return this.tween.AsyncWaitForCompletion().AsUniTask();
        }
    }

    [ScreenInfo(nameof(UITemplateLoadingScreenView))]
    public class UITemplateLoadingScreenPresenter : UITemplateBaseScreenPresenter<UITemplateLoadingScreenView>
    {
        #region Inject

        protected readonly UITemplateAdServiceWrapper adService;
        protected readonly BlueprintReaderManager     blueprintManager;
        protected readonly UserDataManager            userDataManager;
        protected readonly IGameAssets                gameAssets;
        private readonly   ObjectPoolManager          objectPoolManager;
        private readonly   UITemplateAdServiceWrapper uiTemplateAdServiceWrapper;

        protected UITemplateLoadingScreenPresenter(
            SignalBus signalBus,
            UITemplateAdServiceWrapper adService,
            BlueprintReaderManager blueprintManager,
            UserDataManager userDataManager,
            IGameAssets gameAssets,
            ObjectPoolManager objectPoolManager,
            UITemplateAdServiceWrapper uiTemplateAdServiceWrapper
        ) : base(signalBus)
        {
            this.adService                  = adService;
            this.blueprintManager           = blueprintManager;
            this.userDataManager            = userDataManager;
            this.gameAssets                 = gameAssets;
            this.objectPoolManager          = objectPoolManager;
            this.uiTemplateAdServiceWrapper = uiTemplateAdServiceWrapper;
        }

        #endregion

        protected virtual string NextSceneName => "1.MainScene";

        private bool IsClosedFirstOpen { get; set; }

        private float _loadingProgress;
        private int   loadingSteps;

        private float loadingProgress
        {
            get => this._loadingProgress;
            set
            {
                this._loadingProgress = value;
                this.View.SetProgress(value / this.loadingSteps);
            }
        }

        private GameObject objectPoolContainer;

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.OpenViewAsync().Forget();
        }

        public override UniTask BindData()
        {
            this.ShowFirstBannerAd(BannerLoadStrategy.Instantiate);
            this.SignalBus.Subscribe<AppOpenFullScreenContentClosedSignal>(this.OnAOAClosedHandler);
            this.SignalBus.Subscribe<AppOpenFullScreenContentFailedSignal>(this.OnAOAClosedHandler);

            this.objectPoolContainer = new(nameof(this.objectPoolContainer));
            Object.DontDestroyOnLoad(this.objectPoolContainer);

            this.loadingProgress = 0f;
            this.loadingSteps    = 1;

            UniTask.WhenAll(
                this.CreateObjectPool(AudioService.AudioSourceKey, 3),
                this.Preload(),
                #if ADMOB || APPLOVIN
                this.WaitForAoa(),
                #endif
                UniTask.WhenAll(
                    this.LoadBlueprint().ContinueWith(this.OnBlueprintLoaded),
                    this.LoadUserData().ContinueWith(this.OnUserDataLoaded)
                ).ContinueWith(this.OnBlueprintAndUserDataLoaded)
            ).ContinueWith(this.OnLoadingCompleted).ContinueWith(this.LoadNextScene);

            return UniTask.CompletedTask;
        }

        public override void Dispose()
        {
            base.Dispose();
            this.SignalBus.Unsubscribe<AppOpenFullScreenContentClosedSignal>(this.OnAOAClosedHandler);
            this.SignalBus.Unsubscribe<AppOpenFullScreenContentFailedSignal>(this.OnAOAClosedHandler);
        }

        private void OnAOAClosedHandler() { this.IsClosedFirstOpen = true; }

        protected virtual async UniTask LoadNextScene()
        {
            SceneDirector.CurrentSceneName = this.NextSceneName;

            this.SignalBus.Fire<StartLoadingNewSceneSignal>();
            var nextScene = await this.TrackProgress(this.LoadSceneAsync());
            await this.View.CompleteLoading();
            await nextScene.ActivateAsync();
            this.SignalBus.Fire<FinishLoadingNewSceneSignal>();

            Resources.UnloadUnusedAssets().ToUniTask().Forget();
            this.ShowFirstBannerAd(BannerLoadStrategy.AfterLoading);
            this.OnAfterLoading();
        }

        protected virtual void ShowFirstBannerAd(BannerLoadStrategy strategy)
        {
            if (strategy != this.adService.BannerLoadStrategy) return;
            this.adService.ShowBannerAd();
        }

        protected virtual void OnAfterLoading() { }

        protected virtual AsyncOperationHandle<SceneInstance> LoadSceneAsync() { return this.gameAssets.LoadSceneAsync(this.NextSceneName, LoadSceneMode.Single, false); }

        private UniTask LoadBlueprint()
        {
            this.TrackProgress<LoadBlueprintDataProgressSignal>();
            this.TrackProgress<ReadBlueprintProgressSignal>();

            return this.blueprintManager.LoadBlueprint();
        }

        private UniTask LoadUserData() { return this.TrackProgress(this.userDataManager.LoadUserData()); }

        private UniTask WaitForAoa()
        {
            var startWaitingAoaTime = DateTime.Now;

            // sometimes AOA delay when shown, we need 0.5s to wait for it
            return this.TrackProgress(UniTask.WaitUntil(() =>
                (this.uiTemplateAdServiceWrapper.IsOpenedAOAFirstOpen && this.IsClosedFirstOpen) ||
                (!this.uiTemplateAdServiceWrapper.IsOpenedAOAFirstOpen && (DateTime.Now - startWaitingAoaTime).TotalSeconds > this.uiTemplateAdServiceWrapper.LoadingTimeToShowAOA)));
        }

        protected virtual UniTask OnBlueprintLoaded() { return UniTask.CompletedTask; }

        protected virtual UniTask OnUserDataLoaded() { return UniTask.CompletedTask; }

        protected virtual UniTask OnBlueprintAndUserDataLoaded() { return UniTask.CompletedTask; }

        protected virtual UniTask OnLoadingCompleted() { return UniTask.CompletedTask; }

        protected virtual UniTask Preload() { return UniTask.CompletedTask; }

        protected UniTask PreloadAssets<T>(params object[] keys)
        {
            return UniTask.WhenAll(this.gameAssets.PreloadAsync<T>(this.NextSceneName, keys)
                .Select(this.TrackProgress));
        }

        protected UniTask CreateObjectPool(string prefabName, int initialPoolSize = 1)
        {
            return this.TrackProgress(
                this.objectPoolManager.CreatePool(prefabName, initialPoolSize, this.objectPoolContainer));
        }

        protected UniTask TrackProgress(UniTask task)
        {
            ++this.loadingSteps;

            return task.ContinueWith(() => ++this.loadingProgress);
        }

        protected UniTask<T> TrackProgress<T>(AsyncOperationHandle<T> aoh)
        {
            ++this.loadingSteps;
            var localLoadingProgress = 0f;

            void UpdateProgress(float progress)
            {
                this.loadingProgress += progress - localLoadingProgress;
                localLoadingProgress =  progress;
            }

            return aoh.ToUniTask(Progress.CreateOnlyValueChanged<float>(UpdateProgress))
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
                this.loadingProgress += progress.Percent - localLoadingProgress;
                localLoadingProgress =  progress.Percent;
                if (progress.Percent >= 1f)
                {
                    this.SignalBus.Unsubscribe<T>(UpdateProgress);
                }
            }
        }
    }
}