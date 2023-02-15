namespace UITemplate.Scripts.Scenes.Main.Collection.Elements
{
    using GameFoundation.Scripts.AssetLibrary;
    using UITemplate.Scripts.Scenes.Main.Collection.Base;

    public class CharacterCollectionItemModel : BaseItemCollectionModel
    {
    }

    public class CharacterCollectionItemView : BaseItemCollectionView
    {
    }

    public class CharacterCollectionItemPresenter : BaseItemCollectionPresenter<CharacterCollectionItemView, CharacterCollectionItemModel>
    {
        protected override string CategoryType => "Character";

        public CharacterCollectionItemPresenter(IGameAssets gameAssets) : base(gameAssets) { }

        public override void BindData(CharacterCollectionItemModel param)
        {
            
        }
    }
}