namespace TheOneStudio.UITemplate.UITemplate.Scenes.Gacha
{
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.MVP;
    using UnityEngine.UI;

    public class UITemplatePagingDotItemView: TViewMono
    {
        public Image imgPageDotActive;
    }
    
    public class UITemplatePagingDotItemModel
    {
        public bool IsSelected { get; set; }
    }
    
    public class UITemplatePagingDotItemPresenter: BaseUIItemPresenter<UITemplatePagingDotItemView, UITemplatePagingDotItemModel>
    {
        public UITemplatePagingDotItemPresenter(IGameAssets gameAssets) : base(gameAssets)
        {
        }

        public override void BindData(UITemplatePagingDotItemModel param)
        {
            this.SetDotActive(param.IsSelected);
        }
        
        public void SetDotActive(bool isActive)
        {
            this.View.imgPageDotActive.gameObject.SetActive(isActive);
        }
    }
}