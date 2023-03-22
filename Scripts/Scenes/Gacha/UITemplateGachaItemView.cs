namespace TheOneStudio.UITemplate.UITemplate.Scenes.Gacha
{
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.MVP;
    using UnityEngine;
    using UnityEngine.UI;

    public class UITemplateGachaItemView : TViewMono
    {
        public GameObject bgNormal;
        public GameObject bgChosen;
        public Image      imgItem;
        public GameObject imgIconChosen;
        public Button     btnSelect;
    }

    public class UITemplateGachaItemModel
    {
        public int  ItemId;
        public bool IsSelected;
    }

    public class UITemplateGachaItemPresenter : BaseUIItemPresenter<UITemplateGachaItemView, UITemplateGachaItemModel>
    {
        public UITemplateGachaItemPresenter(IGameAssets gameAssets) : base(gameAssets) { }

        public override void BindData(UITemplateGachaItemModel param)
        {
            this.View.btnSelect.onClick.AddListener(() => { this.OnSelect(param); });
            this.View.bgNormal.SetActive(!param.IsSelected);
            this.View.bgChosen.SetActive(param.IsSelected);
            this.View.imgIconChosen.SetActive(param.IsSelected);
        }

        private void OnSelect(UITemplateGachaItemModel model) { Debug.Log($"Select item {model.ItemId}"); }
    }
}