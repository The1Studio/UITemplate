namespace TheOneStudio.UITemplate.UITemplate.Scenes.Gacha.LuckyWheel
{
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.MVP;
    using TheOneStudio.UITemplate.UITemplate.Extension;
    using UnityEngine;
    using UnityEngine.UI;

    public class UITemplateLuckyWheelSpinItemModel
    {
        public int    ItemIndex { get; set; }
        public string Icon      { get; set; }
    }

    public class UITemplateLuckyWheelSpinItem : TViewMono
    {
        public Image imgIcon;
    }

    public class UITemplateLuckyWheelSpinItemPresenter : BaseUIItemPresenter<UITemplateLuckyWheelSpinItem, UITemplateLuckyWheelSpinItemModel>
    {
        public UITemplateLuckyWheelSpinItemPresenter(IGameAssets gameAssets) : base(gameAssets) { }

        public override async void BindData(UITemplateLuckyWheelSpinItemModel param)
        {
            if (param.Icon.IsNullOrEmpty())
            {
                this.View.imgIcon.gameObject.SetActive(false);
            }
            else
            {
                this.View.imgIcon.sprite = await this.GameAssets.LoadAssetAsync<Sprite>(param.Icon);
            }
        }
    }
}