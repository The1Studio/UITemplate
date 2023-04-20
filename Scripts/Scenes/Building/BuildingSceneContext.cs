namespace TheOneStudio.HyperCasual.DrawCarBase.Scripts.Runtime.Scenes.Building
{
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.UIModule.Utilities;
    using UnityEngine;

    public class BuildingSceneContext : BaseSceneInstaller
    {
        [SerializeField] private BuildingMainCamera    buildingMainCamera;
        [SerializeField] private BuildingMapController buildingMapController;
        [SerializeField] private BuildingCarController buildingCarController;
        [SerializeField] private BuildingJoyStick      buildingJoyStick;

        public override void InstallBindings()
        {
            base.InstallBindings();
            this.Container.Bind<BuildingMainCamera>().FromInstance(this.buildingMainCamera).AsCached().NonLazy();
            this.Container.Bind<BuildingMapController>().FromInstance(this.buildingMapController).AsCached().NonLazy();
            this.Container.Bind<BuildingCarController>().FromInstance(this.buildingCarController).AsCached().NonLazy();
            this.Container.Bind<BuildingJoyStick>().FromComponentInNewPrefab(this.buildingJoyStick).AsCached().NonLazy();
            this.Container.InitScreenManually<BuildingScreenPresenter>();
        }
    }
}