namespace TheOneStudio.UITemplate.UITemplate.Scenes.Gacha
{
    using System;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.MVP;
    using UnityEngine;
    using UnityEngine.UI;

    public class UITemplateGachaItemView : TViewMono
    {
        public GameObject bgNormal;
        public GameObject bgChosen;
        public Image      imgItem;
        public Image      imgItemLock;
        public GameObject imgIconChosen;
        public Button     btnSelect;
    }

    public class UITemplateGachaItemModel
    {
        public int                              ItemId;
        public bool                             IsSelected;
        public bool                             IsLocked;
        public Action<UITemplateGachaItemModel> OnSelect;
    }

    public class UITemplateGachaItemPresenter : BaseUIItemPresenter<UITemplateGachaItemView, UITemplateGachaItemModel>
    {
        public UITemplateGachaItemPresenter(IGameAssets gameAssets) : base(gameAssets) { }

        public override void BindData(UITemplateGachaItemModel param)
        {
            this.View.btnSelect.onClick.AddListener(() => { this.OnSelect(param); });
            this.View.imgItemLock.gameObject.SetActive(param.IsLocked);
            this.SetSelected(param.IsSelected);
        }

        private void OnSelect(UITemplateGachaItemModel model)
        {
            if (model.IsLocked) return;

            Debug.Log($"Select item {model.ItemId}");
            model.OnSelect?.Invoke(model);
            this.SetSelected(true);
        }

        public void SetSelected(bool isSelected)
        {
            this.View.bgNormal.SetActive(!isSelected);
            this.View.bgChosen.SetActive(isSelected);
            this.View.imgIconChosen.SetActive(isSelected);
        }
    }
}