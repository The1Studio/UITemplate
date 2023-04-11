namespace TheOneStudio.UITemplate.UITemplate.Scenes.ChestRoom
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using TheOneStudio.UITemplate.UITemplate.Blueprints.Gacha;
    using TheOneStudio.UITemplate.UITemplate.Extension;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateChestRoomScreenView : BaseView
    {
        public List<UITemplateChestItemView> ChestItemViewList; // List of chest item view
        public Button                        NoThankButton;     // No thank button
        public Button                        WatchAdButton;     // Watch ad button
        public List<GameObject>              KeyObjectList;     // Open button
        public GameObject                    KeyGroupObject;
    }

    public class UITemplateChestRoomScreenPresenter : UITemplateBaseScreenPresenter<UITemplateChestRoomScreenView>
    {
        #region region

        private readonly UITemplateGachaChestRoomBlueprint uiTemplateGachaChestRoomBlueprint;
        private readonly IGameAssets                       gameAssets;

        #endregion

        private int currentOpenedAmount;
        private List<UITemplateGachaChestRoomRecord> currentChestList;
        
        public UITemplateChestRoomScreenPresenter(SignalBus signalBus, UITemplateGachaChestRoomBlueprint uiTemplateGachaChestRoomBlueprint, IGameAssets gameAssets) : base(signalBus)
        {
            this.uiTemplateGachaChestRoomBlueprint = uiTemplateGachaChestRoomBlueprint;
            this.gameAssets                        = gameAssets;
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
        }
        
        private async void OnClickChestButton(UITemplateChestItemView uiTemplateChestItemView)
        {
            this.currentOpenedAmount++;

            var weights     = this.currentChestList.Select(value => value.Weight).ToList();
            var randomChest = this.currentChestList.RandomGachaWithWeight(weights);
            this.currentChestList.Remove(randomChest);
            var rewardSpite = await this.gameAssets.LoadAssetAsync<Sprite>(randomChest.Icon);
            var value = randomChest.Reward.Count == 1 ? randomChest.Reward.First().Value : 0;
            uiTemplateChestItemView.OpenChest(rewardSpite, value);

            if (this.currentOpenedAmount >= this.View.ChestItemViewList.Count)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1f));
                this.CloseView();
            }
        }

        private void OnClickWatchAdButton() { this.ResetKeys(); }

        private void OnClickNoThankButton() { }

        public override UniTask BindData()
        {
            this.currentOpenedAmount = 0;
            this.currentChestList    = this.uiTemplateGachaChestRoomBlueprint.Values.ToList();
            foreach (var uiTemplateChestItemView in this.View.ChestItemViewList)
            {
                uiTemplateChestItemView.Init();
            }

            this.ResetKeys();

            return UniTask.CompletedTask;
        }

        private void ResetKeys()
        {
            this.View.NoThankButton.gameObject.SetActive(false);
            this.View.WatchAdButton.gameObject.SetActive(false);
            this.View.KeyGroupObject.SetActive(true);

            foreach (var keyObject in this.View.KeyObjectList)
            {
                keyObject.SetActive(true);
            }
        }
    }
}