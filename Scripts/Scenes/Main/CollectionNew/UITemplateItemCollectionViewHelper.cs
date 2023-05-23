namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main.CollectionNew
{
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.AssetLibrary;
    using UnityEngine;

    // Rebind this class to your own item view
    public class UITemplateItemCollectionViewHelper
    {
        protected readonly IGameAssets GameAssets;

        public UITemplateItemCollectionViewHelper(IGameAssets gameAssets) { this.GameAssets = gameAssets; }

        public virtual async void BindDataItem(ItemCollectionItemModel param, ItemCollectionItemView view)
        {
            view.imgIcon.sprite = await this.GameAssets.LoadAssetAsync<Sprite>(param.ItemBlueprintRecord.ImageAddress);
            view.txtPrice.text  = $"{param.ShopBlueprintRecord.Price}";
        }

        public virtual void DisposeItem(ItemCollectionItemView view) { }
    }
}