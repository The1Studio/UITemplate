namespace TheOneStudio.HyperCasual.DrawCarBase.Scripts.Runtime.Scenes.Building
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DG.Tweening;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using UnityEngine;
    using Zenject;

    public class BuildingMapController : MonoBehaviour
    {
        #region Zenject

        public List<BuildingBuildingElement> buildingElements { get; private set; } = new();
        public GameObject                    BuildingArrow1;
        public GameObject                    BuildingArrow2;

        private Tween                      arrowTween1;
        private Tween                      arrowTween2;
        private BuildingBuildingElement    buildingId1;
        private BuildingBuildingElement    buildingId2;
        private TaskCompletionSource<bool> isInjected = new(false);

        private SignalBus                    signalBus;
        private UITemplateBuildingController uiTemplateBuildingController;

        [Inject]
        public void OnInit(SignalBus signalBus, UITemplateBuildingController uiTemplateBuildingController)
        {
            this.signalBus                    = signalBus;
            this.uiTemplateBuildingController = uiTemplateBuildingController;

            this.isInjected.TrySetResult(true);

            var buildingArrays = this.GetComponentsInChildren<BuildingBuildingElement>();

            foreach (var buildingElement in buildingArrays)
            {
                this.buildingElements.Add(buildingElement);
            }

            this.buildingId1 = this.buildingElements.Find(x => x.BuildingId.Equals("LivingHouseA"));
            this.buildingId2 = this.buildingElements.Find(x => x.BuildingId.Equals("CoffeeHouse"));

            this.AnimArrow1();
            this.AnimArrow2();
        }

        #endregion

        private async void OnEnable()
        {
            await this.isInjected.Task;
            this.signalBus.Subscribe<UITemplateUnlockBuildingSignal>(this.HandleAnimation);
        }

        private async void OnDisable()
        {
            await this.isInjected.Task;
            this.signalBus.Unsubscribe<UITemplateUnlockBuildingSignal>(this.HandleAnimation);
        }

        private void HandleAnimation()
        {
            if (this.uiTemplateBuildingController.CheckBuildingStatus(this.buildingId1.BuildingId))
            {
                this.arrowTween1.Kill();
                this.BuildingArrow1.SetActive(false);
                this.AnimArrow2();
            }

            if (this.uiTemplateBuildingController.CheckBuildingStatus(this.buildingId2.BuildingId))
            {
                this.arrowTween2.Kill();
                this.BuildingArrow2.SetActive(false);
            }
        }

        private void AnimArrow1()
        {
            if (this.uiTemplateBuildingController.CheckBuildingStatus(this.buildingId1.BuildingId) == false)
            {
                this.BuildingArrow1.SetActive(true);
                this.arrowTween1 = this.BuildingArrow1.transform.DOMoveY(10, 1f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
            }
            else
            {
                this.BuildingArrow1.SetActive(false);
            }
        }

        private void AnimArrow2()
        {
            if (this.uiTemplateBuildingController.CheckBuildingStatus(this.buildingId1.BuildingId) && this.uiTemplateBuildingController.CheckBuildingStatus(this.buildingId2.BuildingId) == false)
            {
                this.BuildingArrow2.SetActive(true);
                this.arrowTween2 = this.BuildingArrow2.transform.DOMoveY(10, 1f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
            }
            else
            {
                this.BuildingArrow2.SetActive(false);
            }
        }
    }
}