namespace UITemplate.Scripts.Scenes.Loading
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using BlueprintFlow.BlueprintControlFlow;
    using BlueprintFlow.Signals;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateLoadingScreenView : BaseView
    {
        [SerializeField] private TMP_Text LoadingText;
        [SerializeField] private Slider   LoadingSlider;

        public void SetLoadingText(string content)
        {
            if (this.LoadingText == null) return;

            this.LoadingText.text = content;
        }

        public void SetLoadingProgressValue(float value)
        {
            if (this.LoadingSlider == null) return;

            this.LoadingSlider.value = value;
        }
    }

    [ScreenInfo(nameof(UITemplateLoadingScreenView))]
    public class UITemplateLoadingScreenPresenter : BaseScreenPresenter<UITemplateLoadingScreenView>
    {
        protected virtual string NextSceneName { get; set; }          
        private const     string LoadingBlueprintStepName = "Loading static data...";

        private const float MinimumLoadingBlueprintTime = 2f; //seconds

        #region inject

        private readonly BlueprintReaderManager blueprintReaderManager;
        private readonly SceneDirector          sceneDirector;

        #endregion

        private DateTime                startedLoadingTime;
        private Dictionary<Type, float> loadingTypeToProgressPercent;


        public UITemplateLoadingScreenPresenter(SignalBus signalBus, BlueprintReaderManager blueprintReaderManager, SceneDirector sceneDirector) : base(signalBus)
        {
            this.blueprintReaderManager = blueprintReaderManager;
            this.sceneDirector          = sceneDirector;
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.OpenViewAsync();
        }

        public override void BindData()
        {
            this.startedLoadingTime = DateTime.Now;
            this.SignalBus.Subscribe<LoadBlueprintDataProgressSignal>(this.OnLoadProgress);
            this.SignalBus.Subscribe<ReadBlueprintProgressSignal>(this.OnLoadProgress);
            this.loadingTypeToProgressPercent = new Dictionary<Type, float> { { typeof(LoadBlueprintDataProgressSignal), 0f }, { typeof(ReadBlueprintProgressSignal), 0f }, };

            this.ShowLoadingProgress(LoadingBlueprintStepName);
            this.blueprintReaderManager.LoadBlueprint();
        }
        
        private async void ShowLoadingProgress(string loadingContent)
        {
            this.View.SetLoadingText(loadingContent);
            var progressInView       = 0f;
            
            while (progressInView < 1f)
            {
                var currentProgress = this.loadingTypeToProgressPercent.Values.Average();
                var maximumLoadingPercent = (float)(DateTime.Now - this.startedLoadingTime).TotalSeconds / MinimumLoadingBlueprintTime;
                progressInView = Mathf.Min(currentProgress, maximumLoadingPercent);
                this.View.SetLoadingProgressValue(progressInView);
                await UniTask.WaitForEndOfFrame();
            }
            this.sceneDirector.LoadSingleSceneAsync(NextSceneName);
        }

        private void OnLoadProgress(IProgressPercent obj) { this.loadingTypeToProgressPercent[obj.GetType()] = obj.Percent; }
    }
}