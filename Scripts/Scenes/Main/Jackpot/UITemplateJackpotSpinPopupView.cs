namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main.Jackpot
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Com.TheFallenGames.OSA.Core;
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.Utilities.LogService;
    using GameFoundation.Scripts.Utilities.Utils;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Extension;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateJackpotSpinPopupView : BaseView
    {
        public Button                       btnSpin;
        public Button                       btnWatchAds;
        public Button                       btnClaim;
        public Button                       btnSkipAds;
        public UITemplateJackpotItemAdapter jackpotItemAdapter;
        public GameObject                   jackpotSpinningObj;
        public List<GameObject>             listJackpotSpinning;
    }

    public class UITemplateJackpotSpinPopupModel
    {
        public Action OnClaim;

        public UITemplateJackpotSpinPopupModel(Action onClaim) { this.OnClaim = onClaim; }
    }

    [PopupInfo(nameof(UITemplateJackpotSpinPopupView))]
    public class UITemplateJackpotSpinPopupPresenter : UITemplateBasePopupPresenter<UITemplateJackpotSpinPopupView, UITemplateJackpotSpinPopupModel>
    {
        private const int MultipleTime = 20;

        private readonly ILogService                       logger;
        private readonly EventSystem                       eventSystem;
        private readonly DiContainer                       diContainer;
        private readonly UITemplateJackpotItemBlueprint    uiTemplateJackpotItemBlueprint;
        private readonly UITemplateAdServiceWrapper        uiTemplateAdServiceWrapper;
        private readonly UITemplateJackpotRewardBlueprint  uiTemplateJackpotRewardBlueprint;
        private readonly UITemplateJackpotController       uiTemplateJackpotController;
        private readonly UITemplateInventoryDataController uiTemplateInventoryDataController;
        private readonly UITemplateItemBlueprint           uiTemplateItemBlueprint;
        private readonly UITemplateShopBlueprint           uiTemplateShopBlueprint;

        private Snapper8                         snapper8;
        private List<UITemplateJackpotItemModel> listJackpotItemModels = new();
        private UITemplateJackpotItemModel       currentJackpotItem;
        private IDisposable                      randomTimerDispose;

        public UITemplateJackpotSpinPopupPresenter(SignalBus signalBus, ILogService logger, EventSystem eventSystem, DiContainer diContainer,
            UITemplateJackpotItemBlueprint uiTemplateJackpotItemBlueprint, UITemplateAdServiceWrapper uiTemplateAdServiceWrapper,
            UITemplateJackpotRewardBlueprint uiTemplateJackpotRewardBlueprint, UITemplateJackpotController uiTemplateJackpotController,
            UITemplateInventoryDataController uiTemplateInventoryDataController, UITemplateItemBlueprint uiTemplateItemBlueprint,
            UITemplateShopBlueprint uiTemplateShopBlueprint) : base(signalBus, logger)
        {
            this.logger                            = logger;
            this.eventSystem                       = eventSystem;
            this.diContainer                       = diContainer;
            this.uiTemplateJackpotItemBlueprint    = uiTemplateJackpotItemBlueprint;
            this.uiTemplateAdServiceWrapper        = uiTemplateAdServiceWrapper;
            this.uiTemplateJackpotRewardBlueprint  = uiTemplateJackpotRewardBlueprint;
            this.uiTemplateJackpotController       = uiTemplateJackpotController;
            this.uiTemplateInventoryDataController = uiTemplateInventoryDataController;
            this.uiTemplateItemBlueprint           = uiTemplateItemBlueprint;
            this.uiTemplateShopBlueprint           = uiTemplateShopBlueprint;
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.View.btnClaim.onClick.AddListener(this.OnClickClaim);
            this.View.btnSpin.onClick.AddListener(this.OnClickSpin);
            this.View.btnWatchAds.onClick.AddListener(this.WatchAdsToSpin);
            this.View.btnSkipAds.onClick.AddListener(this.CloseView);
        }

        public override void BindData(UITemplateJackpotSpinPopupModel popupModel)
        {
            this.Model = popupModel;
            this.FakeListJackpotItem();
            this.CheckButtonStatusByRemainingSpin();
            this.View.btnClaim.gameObject.SetActive(false);
        }

        private async void CheckButtonStatusByRemainingSpin()
        {
            var remainingSpin = await this.uiTemplateJackpotController.UserRemainingJackpotSpin() > 0;
            this.View.btnSpin.gameObject.SetActive(remainingSpin);
            this.View.btnWatchAds.gameObject.SetActive(!remainingSpin);
            this.View.btnSkipAds.gameObject.SetActive(!remainingSpin);
        }

        private void OnClickSpin()
        {
            this.DoSpin();
            this.DoSpinningAnimByListImages();
        }

        private void DoSpinningAnimByListImages()
        {
            this.View.listJackpotSpinning.GachaItemWithTimer(this.randomTimerDispose,
                (obj) => { this.View.listJackpotSpinning.ForEach(o => o.SetActive(false)); }, (obj) =>
                {
                    this.View.listJackpotSpinning.ForEach(o => o.SetActive(false));
                    obj.SetActive(true);
                }, 4f, 0.1f);
        }

        private void OnClickClaim()
        {
            if (this.currentJackpotItem == null) return;
            var jackpotItemRecord = this.uiTemplateJackpotItemBlueprint.GetDataById(this.currentJackpotItem.Id);
            this.Claim(jackpotItemRecord);
            this.FakeListJackpotItem();
            this.Model.OnClaim?.Invoke();
        }

        private void Claim(UITemplateJackpotItemRecord jackpotItemRecord)
        {
            this.View.btnClaim.gameObject.SetActive(false);
            this.CheckButtonStatusByRemainingSpin();
            foreach (var rewardDict in jackpotItemRecord.Reward)
            {
                foreach (var reward in rewardDict)
                {
                    if (reward.Key.Equals("Coin"))
                    {
                        this.uiTemplateInventoryDataController.AddCurrency(reward.Value, reward.Key);
                        // Do coin's reward animation
                    }
                    else
                    {
                        for (var i = 0; i < reward.Value; i++)
                            this.uiTemplateInventoryDataController.AddItemData(
                                new UITemplateItemData(reward.Key, this.uiTemplateShopBlueprint.GetDataById(reward.Key), UITemplateItemData.Status.Owned));
                        // Do item's reward animation
                    }
                }
            }
        }

        private void WatchAdsToSpin()
        {
            this.View.btnWatchAds.gameObject.SetActive(false);
            this.uiTemplateAdServiceWrapper.ShowRewardedAd("Jackpot", async () =>
            {
                await UniTask.Delay(1000);
                this.DoSpin();
            });
        }

        private void FakeListJackpotItem()
        {
            this.listJackpotItemModels = this.uiTemplateJackpotItemBlueprint.Values.Select(record => new UITemplateJackpotItemModel(record.Id)).Shuffle().ToList();
            for (var i = 0; i < MultipleTime; i++)
            {
                this.listJackpotItemModels.AddRange(this.uiTemplateJackpotItemBlueprint.Values.Select(record => new UITemplateJackpotItemModel(record.Id)).Shuffle());
            }

            this.InitListJackpotItem();
        }

        private async void InitListJackpotItem()
        {
            await this.View.jackpotItemAdapter.InitItemAdapter(this.listJackpotItemModels, this.diContainer);
            this.snapper8 = this.View.jackpotItemAdapter.GetComponent<Snapper8>();
        }

        private void DoSpin()
        {
            this.View.btnSpin.gameObject.SetActive(false);
            this.uiTemplateJackpotController.DoJackpotSpin();
            this.snapper8.enabled = false;
            var itemIdScrollTo    = this.uiTemplateJackpotRewardBlueprint.GetDataById(this.uiTemplateJackpotController.UserCurrentJackpotSpin().ToString());
            var itemIndexScrollTo = this.listJackpotItemModels.FindLastIndex(item => item.Id == itemIdScrollTo.JackpotItem);
            this.currentJackpotItem = this.listJackpotItemModels[itemIndexScrollTo];
            this.View.jackpotItemAdapter.SmoothScrollTo(itemIndexScrollTo, 4f, onDone: this.OnSpinFinish);
        }

        private void OnSpinFinish()
        {
            this.snapper8.enabled = true;
            this.eventSystem.gameObject.SetActive(true);
            this.View.jackpotItemAdapter.ForceUpdateFullVisibleItems();
            this.View.btnClaim.gameObject.SetActive(true);
        }
    }
}