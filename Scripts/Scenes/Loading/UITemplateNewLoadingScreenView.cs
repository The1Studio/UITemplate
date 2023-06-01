namespace TheOneStudio.UITemplate.UITemplate.Scenes.Loading
{
    using BlueprintFlow.BlueprintControlFlow;
    using BlueprintFlow.Signals;
    using Core.AdsServices;
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Signals;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using TheOneStudio.UITemplate.UITemplate.UserData;
    using UniRx;
    using UnityEngine;
    using UnityEngine.ResourceManagement.AsyncOperations;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateNewLoadingScreenView : BaseView
    {
        [SerializeField] private Slider sld;

        private Tween tween;
        private float trueProgress;

        private void Start()
        {
            this.sld.value = 0f;
        }

        public void SetProgress(float progress)
        {
            if (!this.sld) return;
            if (progress <= this.trueProgress) return;
            this.tween.Kill();
            this.tween = DOTween.To(
                getter: () => this.sld.value,
                setter: value => this.sld.value = value,
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

    [ScreenInfo(nameof(UITemplateNewLoadingScreenView))]
    public class UITemplateNewLoadingScreenPresenter : UITemplateBaseScreenPresenter<UITemplateNewLoadingScreenView>
    {
        #region Inject

        protected readonly UITemplateAdServiceWrapper adService;
        protected readonly IAOAAdService              aoaAdService;
        protected readonly BlueprintReaderManager     blueprintManager;
        protected readonly UserDataManager            userDataManager;
        protected readonly IGameAssets                gameAssets;

        protected UITemplateNewLoadingScreenPresenter(
            SignalBus signalBus,
            UITemplateAdServiceWrapper adService,
            IAOAAdService aoaAdService,
            BlueprintReaderManager blueprintManager,
            UserDataManager userDataManager,
            IGameAssets gameAssets
        ) : base(signalBus)
        {
            this.adService        = adService;
            this.aoaAdService     = aoaAdService;
            this.blueprintManager = blueprintManager;
            this.userDataManager  = userDataManager;
            this.gameAssets       = gameAssets;
        }

        #endregion

        protected virtual string NextSceneName  => "1.MainScene";
        protected virtual string NextScreenName => nameof(UITemplateHomeSimpleScreenView);

        private FloatReactiveProperty loadingProgress;
        private int                   loadingSteps;

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.OpenViewAsync().Forget();
        }

        public override UniTask BindData()
        {
            this.loadingProgress = new(0f);
            this.loadingSteps    = 1;

            this.loadingProgress.Subscribe(value => this.View.SetProgress(value / this.loadingSteps));

            UniTask.WhenAll(
                this.PreloadAssets(),
                this.WaitForAoa(),
                UniTask.WhenAll(
                    this.LoadBlueprint(),
                    this.LoadUserData()
                ).ContinueWith(this.OnBlueprintAndUserDataLoaded)
            ).ContinueWith(this.OnLoadingCompleted).ContinueWith(this.LoadNextScene);

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
            return this.TrackProgress(UniTask.WaitUntil(() => !this.aoaAdService.IsShowingAd));
        }

        protected virtual UniTask OnBlueprintAndUserDataLoaded()
        {
            return UniTask.CompletedTask;
        }

        protected virtual UniTask OnLoadingCompleted()
        {
            return UniTask.CompletedTask;
        }

        protected virtual UniTask PreloadAssets()
        {
            return this.PreloadAssets<GameObject>(this.NextScreenName);
        }

        protected UniTask PreloadAssets<T>(params object[] keys)
        {
            return UniTask.WhenAll(this.gameAssets.PreloadAsync<T>(this.NextSceneName, keys).Select(this.TrackProgress));
        }

        protected UniTask TrackProgress(UniTask task)
        {
            ++this.loadingSteps;
            return task.ContinueWith(() => ++this.loadingProgress.Value);
        }

        protected UniTask<T> TrackProgress<T>(AsyncOperationHandle<T> aoh)
        {
            ++this.loadingSteps;
            var localLoadingProgress = 0f;

            void UpdateProgress(float progress)
            {
                this.loadingProgress.Value += progress - localLoadingProgress;
                localLoadingProgress       =  progress;
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

            this.SignalBus.Subscribe<T>(UpdateReadProgress);

            void UpdateReadProgress(T progress)
            {
                this.loadingProgress.Value += progress.Percent - localLoadingProgress;
                localLoadingProgress       =  progress.Percent;
                if (progress.Percent >= 1f)
                {
                    this.SignalBus.Unsubscribe<T>(UpdateReadProgress);
                }
            }
        }
    }
}