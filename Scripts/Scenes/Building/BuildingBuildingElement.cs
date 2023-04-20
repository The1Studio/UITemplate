namespace TheOneStudio.HyperCasual.DrawCarBase.Scripts.Runtime.Scenes.Building
{
    using DG.Tweening;
    using GameFoundation.Scripts.Utilities.LogService;
    using Sirenix.OdinInspector;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Extension;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;

    [RequireComponent(typeof(BoxCollider), typeof(Renderer))]
    public class BuildingBuildingElement : MonoBehaviour
    {
        public  string          BuildingId;
        public  Image           imgToFill;
        public  TextMeshProUGUI txtPrice;
        private BoxCollider     boxCollider => this.GetComponent<BoxCollider>();
        private Renderer        renderer    => this.GetComponent<Renderer>();

        #region Zenject

        private UITemplateBuildingController      uiTemplateBuildingController;
        private float                             timeIdeToUnlock = 0;
        private float                             TimeWhileUnlocking;
        private UITemplateBuildingBlueprint       uiTemplateBuildingBlueprint;
        private UITemplateInventoryDataController uiTemplateInventoryDataController;

        [Inject]
        public void OnInit(ILogService logger, SignalBus signalBus, UITemplateInventoryDataController uiTemplateInventoryDataController, UITemplateBuildingController uiTemplateBuildingController,
            UITemplateBuildingBlueprint uiTemplateBuildingBlueprint)
        {
            this.uiTemplateInventoryDataController = uiTemplateInventoryDataController;
            this.uiTemplateBuildingController      = uiTemplateBuildingController;
            this.uiTemplateBuildingBlueprint       = uiTemplateBuildingBlueprint;

            if (this.BuildingId.IsNullOrEmpty())
            {
                this.BuildingId = this.name;
            }

            if (this.imgToFill != null)
            {
                this.imgToFill.fillAmount = 0;
            }

            var canvas = this.GetComponentInChildren<Canvas>();

            if (canvas != null)
            {
                canvas.worldCamera = Camera.main;
            }

            this.InitBuildingStatus();
        }

        #endregion

        private void InitBuildingStatus()
        {
            var isOwner = this.uiTemplateBuildingController.CheckBuildingStatus(this.BuildingId);
            this.boxCollider.isTrigger = !isOwner;
            this.renderer.enabled      = isOwner;
            this.SetPrice();
        }

        private BuildingData GetBuildingData => this.uiTemplateBuildingController.GetBuildingData(this.BuildingId);

        public void CheckToUnlockBuilding()
        {
            if (this.GetBuildingData.IsUnlocked) return;

            this.timeIdeToUnlock += Time.deltaTime;
            this.CheckToFillCarOnStay(this.timeIdeToUnlock / this.uiTemplateBuildingBlueprint[this.BuildingId].IdleTimeUnlock);

            if (this.timeIdeToUnlock < this.uiTemplateBuildingBlueprint[this.BuildingId].IdleTimeUnlock) return;

            this.TryToUnlockBuilding();
        }

        private void TryToUnlockBuilding()
        {
            this.TimeWhileUnlocking += Time.deltaTime;
            var currentBuildingData = this.GetBuildingData;

            if (this.TimeWhileUnlocking >= 0.001f && !currentBuildingData.IsUnlocked && this.uiTemplateInventoryDataController.GetCurrencyValue() > 0)
            {
                this.TimeWhileUnlocking = 0;
                var reduceCurrencyValue = 10;

                if (currentBuildingData.UnlockPrice - reduceCurrencyValue < 0)
                {
                    reduceCurrencyValue = currentBuildingData.UnlockPrice;
                }

                if (this.uiTemplateInventoryDataController.GetCurrencyValue() < reduceCurrencyValue)
                {
                    reduceCurrencyValue = this.uiTemplateInventoryDataController.GetCurrencyValue();
                }

                currentBuildingData.UnlockPrice -= reduceCurrencyValue;
                this.SetPrice();
                this.uiTemplateInventoryDataController.UpdateCurrency(this.uiTemplateInventoryDataController.GetCurrencyValue() - reduceCurrencyValue);
            }

            if (currentBuildingData.UnlockPrice > 0) return;
            this.CheckToFillCarOnStay(0);
            this.uiTemplateBuildingController.UnlockBuilding(this.BuildingId);
            this.renderer.enabled     = true;
            this.boxCollider.enabled  = false;
            this.transform.localScale = Vector3.zero;
            this.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.InBack).OnComplete(() => { this.boxCollider.enabled = true; });
        }

        private void ResetColdDownTimeUnlock()
        {
            this.timeIdeToUnlock    = 0;
            this.TimeWhileUnlocking = 0;
            this.CheckToFillCarOnStay(0);
        }

        private void CheckToFillCarOnStay(float value)
        {
            if (this.imgToFill == null) return;
            this.imgToFill.fillAmount = value;
        }

        private void SetPrice()
        {
            if (this.txtPrice != null)
            {
                this.txtPrice.text = this.GetBuildingData.UnlockPrice.ToString();
                this.txtPrice.gameObject.SetActive(this.GetBuildingData.UnlockPrice > 0);
            }
        }

        public void OnCarEnter() { this.ResetColdDownTimeUnlock(); }

        public void OnCarExit()
        {
            this.ResetColdDownTimeUnlock();

            if (this.GetBuildingData.IsUnlocked)
            {
                this.InitBuildingStatus();
            }
        }
    }
}