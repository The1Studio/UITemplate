namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main.Jackpot
{
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.MVP;

    public class UITemplateJackpotItemView : TViewMono
    {
    }

    public class UITemplateJackpotItemModel
    {
        public string Id;

        public UITemplateJackpotItemModel(string id) { this.Id = id; }
    }

    public class UITemplateJackpotItemPresenter : BaseUIItemPresenter<UITemplateJackpotItemView, UITemplateJackpotItemModel>
    {
        public UITemplateJackpotItemPresenter(IGameAssets gameAssets) : base(gameAssets) { }

        public override void BindData(UITemplateJackpotItemModel param) { }
    }
}