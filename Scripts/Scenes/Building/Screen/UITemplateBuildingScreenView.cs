namespace TheOneStudio.HyperCasual.DrawCarBase.Scripts.Runtime.Scenes.Building
{
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateBuildingScreenView : BaseView
    {
        public UITemplateCurrencyView uiTemplateCurrencyView;
        public Button                 btnHOme;
        public List<Transform>        ListTranformTutorial;
        public GameObject             ObjTutorial;
        public GameObject             ObjFinger;
    }

    [ScreenInfo(nameof(UITemplateBuildingScreenView))]
    public class UITemplateBuildingScreenPresenter : UITemplateBaseScreenPresenter<UITemplateBuildingScreenView>
    {
        private Tween tutorialTween;

        private readonly SignalBus                         signalBus;
        private readonly SceneDirector                     sceneDirector;
        private readonly UITemplateInventoryDataController uiTemplateInventoryDataController;
        private readonly UITemplateBuildingController      buildingController;

        public UITemplateBuildingScreenPresenter(SignalBus signalBus, SceneDirector sceneDirector,
            UITemplateInventoryDataController uiTemplateInventoryDataController,
            UITemplateBuildingController buildingController) : base(signalBus)
        {
            this.signalBus                         = signalBus;
            this.sceneDirector                     = sceneDirector;
            this.uiTemplateInventoryDataController = uiTemplateInventoryDataController;
            this.buildingController                = buildingController;
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            _ = this.OpenViewAsync();
            this.View.btnHOme.onClick.AddListener(() => { this.sceneDirector.LoadSingleSceneAsync("1.MainScene"); });
        }

        public override UniTask BindData()
        {
            this.View.uiTemplateCurrencyView.Subscribe(this.SignalBus, this.uiTemplateInventoryDataController.GetCurrencyValue());
            if (this.buildingController.IsFirstTimeOpenBuilding)
            {
                this.BuildingTutorial();
            }
            this.signalBus.Subscribe<JoystickOnMouseDownSignal>(this.OnMouseDown);
            return UniTask.CompletedTask;
        }
        
        private void OnMouseDown()
        {
            this.View.ObjTutorial.SetActive(false);
        }

        private void BuildingTutorial()
        {
            this.tutorialTween = this.View.ObjFinger.transform.DOPath(this.View.ListTranformTutorial.Select(x => x.position).ToArray(),
                    2f,
                    PathType.CatmullRom,
                    PathMode.Ignore)
                .SetLoops(-1, LoopType.Restart)
                .SetEase(Ease.Linear);
        }

        public override void Dispose()
        {
            base.Dispose();
            this.signalBus.Unsubscribe<JoystickOnMouseDownSignal>(this.OnMouseDown);
            this.View.uiTemplateCurrencyView.Unsubscribe(this.SignalBus);
        }
    }
}