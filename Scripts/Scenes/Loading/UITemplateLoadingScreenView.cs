namespace TheOneStudio.UITemplate.UITemplate.Scenes.Loading
{
    using System;
    using System.Collections.Generic;
    using BlueprintFlow.BlueprintControlFlow;
    using BlueprintFlow.Signals;
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Signals;
    using GameFoundation.Scripts.Utilities;
    using GameFoundation.Scripts.Utilities.Extension;
    using GameFoundation.Scripts.Utilities.ObjectPool;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using TheOneStudio.UITemplate.UITemplate.UserData;
    using UnityEngine;
    using UnityEngine.ResourceManagement.AsyncOperations;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;
    using Zenject;
    using Object = UnityEngine.Object;

    public class UITemplateLoadingScreenView : BaseView
    {
        [SerializeField] private Slider LoadingSlider;

        private Tween tween;
        private float trueProgress;

        private void Start()
        {
            this.LoadingSlider.value = 0f;
        }

        public void SetProgress(float progress)
        {
            if (!this.LoadingSlider) return;
            if (progress <= this.trueProgress) return;
            this.tween.Kill();
            this.tween = DOTween.To(
                getter: () => this.LoadingSlider.value,
                setter: value => this.LoadingSlider.value = value,
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
    public abstract class UITemplateLoadingScreenPresenter : UITemplateBaseScreenPresenter<UITemplateLoadingScreenView>
    {
        #region Inject

        protected readonly UITemplateAdServiceWrapper adService;
        protected readonly BlueprintReaderManager     blueprintManager;
        protected readonly UserDataManager            userDataManager;
        protected readonly IGameAssets                gameAssets;
        private readonly   ObjectPoolManager          objectPoolManager;

        protected UITemplateLoadingScreenPresenter(
            SignalBus signalBus,
            UITemplateAdServiceWrapper adService,
            BlueprintReaderManager blueprintManager,
            UserDataManager userDataManager,
            IGameAssets gameAssets,
            ObjectPoolManager objectPoolManager
        ) : base(signalBus)
        {
            this.adService         = adService;
            this.blueprintManager  = blueprintManager;
            this.userDataManager   = userDataManager;
            this.gameAssets        = gameAssets;
            this.objectPoolManager = objectPoolManager;
        }

        #endregion

        protected virtual string NextSceneName => "1.MainScene";

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

        protected List<UniTask> tasks = new();

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.OpenViewAsync().Forget();
        }

        public override UniTask BindData()
        {
            this.objectPoolContainer = new(nameof(this.objectPoolContainer));
            Object.DontDestroyOnLoad(this.objectPoolContainer);

            this.loadingProgress = 0f;
            this.loadingSteps    = 5;

            this.CreateTask(this.Preload, this.PreLoadDefaultStuff, this.LoadUserData)
                .ContinueWith(() => this.CreateTask(this.OnUserDataLoaded))
                .ContinueWith(() => this.CreateTask(this.LoadBlueprint))
                .ContinueWith(() => this.CreateTask(this.OnBlueprintLoaded))
                .ContinueWith(() => this.CreateTask(this.OnBlueprintAndUserDataLoaded))
                .ContinueWith(() => this.CreateTask(this.OnLoadCompleted))
                .ContinueWith(this.LoadNextScene)
                .Forget();

            return UniTask.CompletedTask;
        }

        private async UniTask LoadNextScene()
        {
            SceneDirector.CurrentSceneName = this.NextSceneName;

            this.SignalBus.Fire<StartLoadingNewSceneSignal>();
            var nextScene = await this.TrackProgress(this.gameAssets.LoadSceneAsync(this.NextSceneName, LoadSceneMode.Single, false));
            await this.View.CompleteLoading();
            await nextScene.ActivateAsync();
            this.SignalBus.Fire<FinishLoadingNewSceneSignal>();

            Resources.UnloadUnusedAssets().ToUniTask().Forget();
            this.adService.ShowBannerAd();
        }

        private void LoadBlueprint()
        {
            this.TrackProgress<LoadBlueprintDataProgressSignal>();
            this.TrackProgress<ReadBlueprintProgressSignal>();
            this.tasks.Add(this.blueprintManager.LoadBlueprint());
        }

        private void LoadUserData()
        {
            this.tasks.Add(this.TrackProgress(this.userDataManager.LoadUserData()));
        }

        protected abstract void Preload();


        protected abstract void OnBlueprintLoaded();

        protected abstract void OnUserDataLoaded();

        protected abstract void OnBlueprintAndUserDataLoaded();

        protected abstract void OnLoadCompleted();

        private void PreLoadDefaultStuff()
        {
            async UniTask CreateAudioPool()
            {
                var audioSourcePrefab = (GameObject)await Resources.LoadAsync(AudioService.AudioSourceKey);
                this.objectPoolManager.CreatePool(audioSourcePrefab, 10, this.objectPoolContainer);
            }

            this.tasks.Add(CreateAudioPool());
        }

        private UniTask CreateTask(params Action[] factories)
        {
            this.tasks.Clear();
            factories.ForEach(factory => factory());
            return UniTask.WhenAll(this.tasks);
        }

        protected void PreloadAssets<T>(params object[] keys)
        {
            this.tasks.AddRange(this.gameAssets.PreloadAsync<T>(this.NextSceneName, keys).Select(aoh => this.TrackProgress(aoh).AsUniTask()));
        }

        protected void CreateObjectPool(string prefabName, int initialPoolSize = 1)
        {
            this.tasks.Add(this.TrackProgress(this.objectPoolManager.CreatePool(prefabName, initialPoolSize, this.objectPoolContainer)));
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