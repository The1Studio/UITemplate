namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main.Jackpot
{
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.MVP;
    using GameFoundation.Scripts.UIModule.Utilities.LoadImage;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using UnityEngine.UI;

    public class UITemplateJackpotItemView : TViewMono
    {
        public Image imgIcon;
    }

    public class UITemplateJackpotItemModel
    {
        public string Id;

        public UITemplateJackpotItemModel(string id) { this.Id = id; }
    }

    public class UITemplateJackpotItemPresenter : BaseUIItemPresenter<UITemplateJackpotItemView, UITemplateJackpotItemModel>
    {
        private readonly LoadImageHelper                loadImageHelper;
        private readonly UITemplateJackpotItemBlueprint jackpotItemBlueprint;

        public UITemplateJackpotItemPresenter(IGameAssets gameAssets, LoadImageHelper loadImageHelper, UITemplateJackpotItemBlueprint jackpotItemBlueprint) : base(gameAssets)
        {
            this.loadImageHelper      = loadImageHelper;
            this.jackpotItemBlueprint = jackpotItemBlueprint;
        }

        public override async void BindData(UITemplateJackpotItemModel param)
        {
            var itemRecord = this.jackpotItemBlueprint.GetDataById(param.Id);
            this.View.imgIcon.sprite = await this.loadImageHelper.LoadLocalSprite(itemRecord.Icon);
        }
    }
}