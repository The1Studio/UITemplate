namespace TheOneStudio.UITemplate.UITemplate.Scenes.ChestRoom
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using TheOneStudio.UITemplate.UITemplate.Blueprints.Gacha;
    using TheOneStudio.UITemplate.UITemplate.Extension;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateChestRoomScreenView : BaseView
    {
        public UITemplateCurrencyView        CurrencyView;
        public List<UITemplateChestItemView> ChestItemViewList; // List of chest item view
        public Button                        NoThankButton;     // No thank button
        public UITemplateAdsButton           WatchAdButton;     // Watch ad button
        public UITemplateKeySetView          KeySetView;        // Key set view
        public GameObject                    KeyGroupObject;
        public Image                         bestPrizeImage;
    }

    [PopupInfo(nameof(UITemplateChestRoomScreenView), false)]
    // Change this into pop-up because I don't want to rebind screen after closing it
    // public class UITemplateChestRoomScreenPresenter : UITemplateBaseScreenPresenter<UITemplateChestRoomScreenView>
    public class UITemplateChestRoomScreenPresenter : UITemplateBasePopupPresenter<UITemplateChestRoomScreenView>
    {
        private const int MaxKeyAmount = 3;
        
        #region region

        private readonly UITemplateGachaChestRoomBlueprint uiTemplateGachaChestRoomBlueprint;
        private readonly IGameAssets                       gameAssets;
        private readonly UITemplateInventoryDataController uiTemplateInventoryDataController;
        private readonly UITemplateAdServiceWrapper        uiTemplateAdServiceWrapper;
        private readonly UITemplateAnimationHelper         uiTemplateAnimationHelper;

        #endregion

        private int                                  currentOpenedAmount;
        private List<UITemplateGachaChestRoomRecord> currentChestList;

        private int CurrentKeyAmount
        {
            get => this.uiTemplateInventoryDataController.GetCurrencyValue(UITemplateInventoryDataController.DefaultChestRoomKeyCurrencyID);
            set => this.uiTemplateInventoryDataController.UpdateCurrency(value, UITemplateInventoryDataController.DefaultChestRoomKeyCurrencyID);
        }

        public UITemplateChestRoomScreenPresenter(SignalBus                         signalBus, UITemplateGachaChestRoomBlueprint uiTemplateGachaChestRoomBlueprint, IGameAssets gameAssets,
                                                  UITemplateInventoryDataController uiTemplateInventoryDataController, UITemplateAdServiceWrapper uiTemplateAdServiceWrapper, UITemplateAnimationHelper uiTemplateAnimationHelper) : base(signalBus)
        {
            this.uiTemplateGachaChestRoomBlueprint = uiTemplateGachaChestRoomBlueprint;
            this.gameAssets                        = gameAssets;
            this.uiTemplateInventoryDataController = uiTemplateInventoryDataController;
            this.uiTemplateAdServiceWrapper        = uiTemplateAdServiceWrapper;
            this.uiTemplateAnimationHelper         = uiTemplateAnimationHelper;
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.View.NoThankButton.onClick.AddListener(this.OnClickNoThankButton);
            this.View.WatchAdButton.onClick.AddListener(this.OnClickWatchAdButton);
            foreach (var uiTemplateChestItemView in this.View.ChestItemViewList)
            {
                uiTemplateChestItemView.ChestButton.onClick.AddListener(() => this.OnClickChestButton(uiTemplateChestItemView));
            }
            this.View.WatchAdButton.OnViewReady(this.uiTemplateAdServiceWrapper);
            this.View.KeyGroupObject.SetActive(true);
            this.View.WatchAdButton.gameObject.SetActive(true);
            this.View.NoThankButton.gameObject.SetActive(true);
        }
        
        public override async UniTask BindData()
        {
            this.currentOpenedAmount = 0;
            this.SetKeyObjectActive(this.CurrentKeyAmount > 0, true);
            this.currentChestList = this.uiTemplateGachaChestRoomBlueprint.Values.Where(chestData => !this.uiTemplateInventoryDataController.IsAlreadyContainedItem(chestData.Reward))
                                        .Take(this.View.ChestItemViewList.Count).ToList();

            this.View.bestPrizeImage.sprite = await this.gameAssets.LoadAssetAsync<Sprite>(this.currentChestList.First(prize => prize.IsBestPrize).Icon);
            foreach (var uiTemplateChestItemView in this.View.ChestItemViewList)
            {
                uiTemplateChestItemView.Init();
            }
            
            //Bind view element
            this.View.KeySetView.BindData(this.SignalBus, this.CurrentKeyAmount);
            this.View.WatchAdButton.BindData("Chest_Room");
            this.View.CurrencyView.Subscribe(this.SignalBus, this.uiTemplateInventoryDataController.GetCurrencyValue());
        }

        private void SetKeyObjectActive(bool isActive, bool force = false) => this.uiTemplateAnimationHelper.SetActiveFreeObject(this.View.WatchAdButton.gameObject, this.View.NoThankButton.gameObject, this.View.KeyGroupObject, isActive, force);

        private async void OnClickChestButton(UITemplateChestItemView uiTemplateChestItemView)
        {
            if (this.CurrentKeyAmount == 0) return;
            this.currentOpenedAmount++;
            this.CurrentKeyAmount--;

            var weights     = this.currentChestList.Select(value => value.Weight).ToList();
            var randomChest = this.currentChestList.RandomGachaWithWeight(weights);
            this.currentChestList.Remove(randomChest);
            var rewardSpite = await this.gameAssets.LoadAssetAsync<Sprite>(randomChest.Icon);
            var value       = randomChest.Reward.Count == 1 ? randomChest.Reward.First().Value : 0;
            uiTemplateChestItemView.OpenChest(rewardSpite, value);

            this.uiTemplateInventoryDataController.AddGenericReward(randomChest.Reward, uiTemplateChestItemView.transform as RectTransform);

            if (this.currentOpenedAmount >= this.View.ChestItemViewList.Count)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1.5f));
                this.CloseView();
            }

            if (this.CurrentKeyAmount == 0)
            {
                this.SetKeyObjectActive(false);
            }
        }

        private void OnClickWatchAdButton()
        {
            this.uiTemplateAdServiceWrapper.ShowRewardedAd("Chest_Room", this.OnRewardedAdWatched);
        }
        private void OnRewardedAdWatched()
        {
            this.uiTemplateInventoryDataController.UpdateCurrency(MaxKeyAmount, UITemplateInventoryDataController.DefaultChestRoomKeyCurrencyID);
            this.SetKeyObjectActive(true);
            this.SetKeyObjectActive(true);
        }

        private void OnClickNoThankButton() { this.CloseView(); }

        public override void Dispose()
        {
            base.Dispose();
            //dispose view element
            this.View.KeySetView.Dispose();
            this.View.WatchAdButton.Dispose();
            this.View.CurrencyView.Unsubscribe(this.SignalBus);
        }
    }
}