namespace TheOneStudio.UITemplate.UITemplate.Scenes.Loading
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BlueprintFlow.BlueprintControlFlow;
    using BlueprintFlow.Signals;
    using Core.AdsServices;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using TheOneStudio.UITemplate.UITemplate.Scripts.Signals;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateLoadingScreenView : BaseView
    {
        [SerializeField]
        private Slider LoadingSlider;

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

        private const float MinimumLoadingBlueprintTime = 1f; //seconds

        private Dictionary<Type, float> loadingTypeToProgressPercent;

        private DateTime startedLoadingTime;
        private DateTime startedShowingAOATime;


        public UITemplateLoadingScreenPresenter(SignalBus signalBus, BlueprintReaderManager blueprintReaderManager, SceneDirector sceneDirector, IAOAAdService aoaAdService, UITemplateAdServiceWrapper uiTemplateAdServiceWrapper) : base(signalBus)
        {
            this.blueprintReaderManager     = blueprintReaderManager;
            this.sceneDirector              = sceneDirector;
            this.aoaAdService               = aoaAdService;
            this.uiTemplateAdServiceWrapper = uiTemplateAdServiceWrapper;
        }

        protected virtual string NextSceneName => "1.UITemplateMainScene";

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.OpenViewAsync();
            this.SignalBus.Fire<AppOpenSignal>();
        }

        public override UniTask BindData()
        {
            this.startedLoadingTime = DateTime.Now;
            this.SignalBus.Subscribe<LoadBlueprintDataProgressSignal>(this.OnLoadProgress);
            this.SignalBus.Subscribe<ReadBlueprintProgressSignal>(this.OnLoadProgress);

            this.loadingTypeToProgressPercent = new Dictionary<Type, float> { { typeof(LoadBlueprintDataProgressSignal), 0f }, { typeof(ReadBlueprintProgressSignal), 0f } };

            this.ShowLoadingProgress(LoadingBlueprintStepName);
            this.blueprintReaderManager.LoadBlueprint();
            return UniTask.CompletedTask;
        }

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

                var currentProgress       = this.loadingTypeToProgressPercent.Values.Average();
                var maximumLoadingPercent = (float)(DateTime.Now - this.startedLoadingTime).TotalSeconds / MinimumLoadingBlueprintTime;
                progressInView = Mathf.Min(currentProgress, maximumLoadingPercent);
                this.View.SetLoadingProgressValue(progressInView);
                await UniTask.WaitForEndOfFrame();
            }

            await this.sceneDirector.LoadSingleSceneAsync(this.NextSceneName);
            this.uiTemplateAdServiceWrapper.ShowBannerAd();
        }

        private void OnLoadProgress(IProgressPercent obj)
        {
            this.loadingTypeToProgressPercent[obj.GetType()] = obj.Percent;
        }

        #region inject

        private readonly BlueprintReaderManager     blueprintReaderManager;
        private readonly SceneDirector              sceneDirector;
        private readonly IAOAAdService              aoaAdService;
        private readonly UITemplateAdServiceWrapper uiTemplateAdServiceWrapper;

        #endregion
    }
}