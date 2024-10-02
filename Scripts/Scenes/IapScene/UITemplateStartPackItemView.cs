namespace TheOneStudio.UITemplate.UITemplate.Scenes.IapScene
{
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.MVP;
    using GameFoundation.Scripts.UIModule.Utilities.LoadImage;
    using TheOneStudio.UITemplate.UITemplate.Extension;
    using TMPro;
    using UnityEngine.Scripting;
    using UnityEngine.UI;

    public class UITemplateStartPackItemModel
    {
        public string IconAddress { get; set; }
        public string Value       { get; set; }
    }

    public class UITemplateStartPackItemView : TViewMono
    {
        public Image           imgIcon;
        public TextMeshProUGUI txtValue;
    }

    public class UITemplateStartPackItemPresenter : BaseUIItemPresenter<UITemplateStartPackItemView, UITemplateStartPackItemModel>
    {
        private readonly LoadImageHelper loadImageHelper;

        [Preserve]
        public UITemplateStartPackItemPresenter(IGameAssets gameAssets, LoadImageHelper loadImageHelper) : base(gameAssets) { this.loadImageHelper = loadImageHelper; }

        public override async void BindData(UITemplateStartPackItemModel param)
        {
            if (!param.IconAddress.IsNullOrEmpty())
            {
                this.View.imgIcon.sprite = await this.loadImageHelper.LoadLocalSprite(param.IconAddress);
            }

            this.View.txtValue.text = $"{param.Value}";
        }
    }
}