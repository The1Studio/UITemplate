namespace TheOneStudio.HyperCasual.DrawCarBase.Scripts.Runtime.Scenes.Building
{
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using UnityEngine.UI;
    using Zenject;

    public class BuildingScreenView : BaseView
    {
        public UITemplateCurrencyView uiTemplateCurrencyView;
        public Button                 btnHOme;
    }

    [ScreenInfo(nameof(BuildingScreenView))]
    public class BuildingScreenPresenter : BaseScreenPresenter<BuildingScreenView>
    {
        private readonly SceneDirector                     sceneDirector;
        private readonly UITemplateInventoryDataController uiTemplateInventoryDataController;

        public BuildingScreenPresenter(SignalBus signalBus, SceneDirector sceneDirector, UITemplateInventoryDataController uiTemplateInventoryDataController) : base(signalBus)
        {
            this.sceneDirector                     = sceneDirector;
            this.uiTemplateInventoryDataController = uiTemplateInventoryDataController;
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

            return UniTask.CompletedTask;
        }

        public override void Dispose()
        {
            base.Dispose();
            this.View.uiTemplateCurrencyView.Unsubscribe(this.SignalBus);
        }
    }
}