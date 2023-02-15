namespace UITemplate.Scripts.Scenes.Main.Collection.Elements
{
    using GameFoundation.Scripts.AssetLibrary;
    using UITemplate.Scripts.Scenes.Main.Collection.Base;

    public class ItemCollectionItemModel : BaseItemCollectionModel
    {
    }

    public class ItemCollectionItemView : BaseItemCollectionView
    {
    }

    public class ItemCollectionItemPresenter : BaseItemCollectionPresenter<ItemCollectionItemView, ItemCollectionItemModel>
    {
        protected override string CategoryType => "Item";

        public ItemCollectionItemPresenter(IGameAssets gameAssets) : base(gameAssets) { }

        public override void BindData(ItemCollectionItemModel param)
        {
            
        }
    }
}