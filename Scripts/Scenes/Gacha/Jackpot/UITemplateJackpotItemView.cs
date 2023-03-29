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
        private readonly UITemplateGachaJackpotBlueprint gachaJackpotBlueprint;

        public UITemplateJackpotItemPresenter(IGameAssets gameAssets, LoadImageHelper loadImageHelper, UITemplateGachaJackpotBlueprint gachaJackpotBlueprint) : base(gameAssets)
        {
            this.loadImageHelper      = loadImageHelper;
            this.gachaJackpotBlueprint = gachaJackpotBlueprint;
        }

        public override async void BindData(UITemplateJackpotItemModel param)
        {
            var itemRecord = this.gachaJackpotBlueprint.GetDataById(param.Id);
            this.View.imgIcon.sprite = await this.loadImageHelper.LoadLocalSprite(itemRecord.Icon);
        }
    }
}