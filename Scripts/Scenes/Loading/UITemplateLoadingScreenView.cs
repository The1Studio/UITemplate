namespace UITemplate.Scripts.Scenes.Loading
{
    using System;
    using BlueprintFlow.BlueprintControlFlow;
    using BlueprintFlow.Signals;
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
        private const string LoadingStepName    = "Loading static data...";
        private const string ReadingStepName    = "Reading static data...";
        
        private const float  MinimumLoadingTime = 0.5f; //seconds
        private const float  MinimumReadingTime = 0.5f; //seconds

        #region inject

        private readonly BlueprintReaderManager blueprintReaderManager;
        private readonly SceneDirector          sceneDirector;

        #endregion

        private DateTime startedLoadingTime;

        public UITemplateLoadingScreenPresenter(SignalBus signalBus, BlueprintReaderManager blueprintReaderManager, SceneDirector sceneDirector) : base(signalBus)
        {
            this.blueprintReaderManager = blueprintReaderManager;
            this.sceneDirector          = sceneDirector;
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.BindData();
        }

        public override async void BindData()
        {
            this.SignalBus.Subscribe<LoadBlueprintDataProgressSignal>(this.OnLoadBlueprintProgress);
            this.SignalBus.Subscribe<ReadBlueprintProgressSignal>(this.OnReadBlueprintProgress);
            this.SignalBus.Subscribe<LoadBlueprintDataSucceedSignal>(this.OnLoadBlueprintDataSucceed);

            await this.blueprintReaderManager.LoadBlueprint();
        }
        
        private void OnLoadBlueprintDataSucceed()
        {
            this.sceneDirector.LoadSingleSceneAsync("1.UITemplateMainScene");
        }

        private void OnLoadBlueprintProgress(LoadBlueprintDataProgressSignal obj)
        {
            this.SetLoadingProgress(obj.Percent, LoadingStepName, MinimumLoadingTime);
        }

        private void OnReadBlueprintProgress(ReadBlueprintProgressSignal obj)
        {
            this.SetLoadingProgress(obj.Percent, ReadingStepName, MinimumReadingTime);
        }

        private void SetLoadingProgress(float percent, string loadingContent, float minimumLoadingTime)
        {
            if (percent == 0)
            {
                this.View.SetLoadingText(loadingContent);
                this.startedLoadingTime = DateTime.Now;
            }
            
            var maximumLoadingPercent = (float)(DateTime.Now - this.startedLoadingTime).TotalSeconds / minimumLoadingTime;
            this.View.SetLoadingProgressValue(Mathf.Min(percent, maximumLoadingPercent));
        }
    }
}