namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main.Collection
{
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.Utilities.LogService;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main.Collection.Elements;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using UnityEngine.Scripting;
    using UnityEngine.UI;

    public class UITemplateCollectionScreenView : BaseView
    {
        public UITemplateCurrencyView CoinText;
        public Button                 HomeButton;
        public UITemplateOnOffButton  CharactersButton;
        public UITemplateOnOffButton  ItemsButton;
        public Button                 WatchAdsButton;
        public ItemCollectionAdapter  ItemCollectionAdapter;
    }

    [ScreenInfo(nameof(UITemplateCollectionScreenView))]
    public class UITemplateCollectionScreenPresenter : UITemplateBaseScreenPresenter<UITemplateCollectionScreenView>
    {
        private const string CatCharacter = "Character";
        private const string CatItem      = "Item";

        private readonly List<ItemCollectionItemModel> itemLists = new();

        [Preserve]
        public UITemplateCollectionScreenPresenter(
            SignalBus                         signalBus,
            ILogService                       logger,
            IScreenManager                    screenManager,
            UITemplateShopBlueprint           shopBlueprint,
            UITemplateItemBlueprint           itemBlueprint,
            UITemplateInventoryDataController uiTemplateInventoryDataController
        ) :
            base(signalBus, logger)
        {
            this.screenManager                     = screenManager;
            this.shopBlueprint                     = shopBlueprint;
            this.itemBlueprint                     = itemBlueprint;
            this.uiTemplateInventoryDataController = uiTemplateInventoryDataController;
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.View.HomeButton.onClick.AddListener(this.OnClickHome);
            this.View.WatchAdsButton.onClick.AddListener(this.OnClickWatchAds);
            this.View.CharactersButton.Button.onClick.AddListener(this.OnClickCharacters);
            this.View.ItemsButton.Button.onClick.AddListener(this.OnClickItem);
        }

        public override UniTask BindData()
        {
            this.GetItemDataList(this.itemLists);
            this.SelectTabCategory(CatCharacter);
            return UniTask.CompletedTask;
        }

        private void OnNotEnoughMoney()
        {
            // show popup not enough money here
        }

        private void GetItemDataList(List<ItemCollectionItemModel> source)
        {
            source.Clear();
            for (var i = 0; i < this.shopBlueprint.Values.Count; i++)
            {
                var shopRecord = this.shopBlueprint.Values.ElementAt(i);
                var itemRecord = this.itemBlueprint.GetDataById(shopRecord.Id);

                if (!itemRecord.Category.Equals(CatItem)) continue;
                var model = new ItemCollectionItemModel
                {
                    Index                       = i,
                    UITemplateItemInventoryData = this.uiTemplateInventoryDataController.GetItemData(itemRecord.Id),
                    Category                    = CatItem,
                    OnBuy                       = this.OnBuyItem,
                    OnSelected                  = this.OnSelectedItem,
                    OnNotEnoughMoney            = this.OnNotEnoughMoney
                };
                source.Add(model);
            }
        }

        private void OnBuyItem(ItemCollectionItemModel obj)
        {
            obj.UITemplateItemInventoryData.CurrentStatus = UITemplateItemData.Status.Owned;
            // this.userData.InventoryData.CurrentSelectItemId.Value = obj.UITemplateItemData.BlueprintRecord.Name;
            this.uiTemplateInventoryDataController.UpdateStatusItemData(obj.UITemplateItemInventoryData.ItemBlueprintRecord.Id, UITemplateItemData.Status.Owned);
            // update payment coin here

            this.View.ItemCollectionAdapter.Refresh();
        }

        private void OnSelectedItem(ItemCollectionItemModel obj)
        {
            // this.userData.UserPackageData.CurrentSelectItemId.Value = obj.UITemplateItemData.BlueprintRecord.Name;
            this.View.ItemCollectionAdapter.Refresh();
        }

        private async void SelectTabCategory(string categoryTab)
        {
            if (categoryTab.Equals(CatItem)) await this.View.ItemCollectionAdapter.InitItemAdapter(this.itemLists);

            // await this.View.CharacterCollectionAdapter.InitItemAdapter(this.characterLists, this.diContainer);
            this.View.ItemCollectionAdapter.gameObject.SetActive(categoryTab.Equals(CatItem));
            // this.View.CharacterCollectionAdapter.gameObject.SetActive(categoryTab.Equals(CatCharacter));
        }

        private void OnClickItem()
        {
            this.SelectTabCategory(CatItem);
            this.ConfigBtnStatus(false, true);
        }

        private void OnClickCharacters()
        {
            this.SelectTabCategory(CatCharacter);
            this.ConfigBtnStatus(true, false);
        }

        private void OnClickHome()
        {
            this.screenManager.OpenScreen<UITemplateHomeSimpleScreenPresenter>();
        }

        private void OnClickWatchAds()
        {
        }

        private void ConfigBtnStatus(bool isCharacterActive, bool isItemActive)
        {
            this.View.CharactersButton.SetOnOff(isCharacterActive);
            this.View.ItemsButton.SetOnOff(isItemActive);
        }

        #region Inject

        private readonly IScreenManager                    screenManager;
        private readonly UITemplateShopBlueprint           shopBlueprint;
        private readonly UITemplateItemBlueprint           itemBlueprint;
        private readonly UITemplateInventoryDataController uiTemplateInventoryDataController;

        #endregion
    }
}