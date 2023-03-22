namespace TheOneStudio.UITemplate.UITemplate.Scenes.Gacha
{
    using System.Collections.Generic;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.MVP;
    using UnityEngine;
    using Zenject;

    public class UITemplateGachaPageView : TViewMono
    {
        public GameObject              gachaItemContainer;
        public UITemplateGachaItemView gachaItemView;
    }

    public class UITemplateGachaPageModel
    {
        public int                            PageId        { get; set; }
        public List<UITemplateGachaItemModel> ListGachaItem { get; set; } = new();
    }

    public class UITemplateGachaPagePresenter : BaseUIItemPresenter<UITemplateGachaPageView, UITemplateGachaPageModel>
    {
        private readonly DiContainer diContainer;

        private List<UITemplateGachaItemPresenter> listGachaItemPresenter = new();

        public UITemplateGachaPagePresenter(IGameAssets gameAssets, DiContainer diContainer) : base(gameAssets) { this.diContainer = diContainer; }

        public override void BindData(UITemplateGachaPageModel param) { this.InitListItems(param); }

        private void InitListItems(UITemplateGachaPageModel model)
        {
            this.ClearChildInGameObject(this.View.gameObject);

            foreach (var gachaItemModel in model.ListGachaItem)
            {
                var presenter = this.diContainer.Instantiate<UITemplateGachaItemPresenter>();
                var view      = GameObject.Instantiate(this.View.gachaItemView, this.View.transform);
                presenter.SetView(view);
                presenter.BindData(gachaItemModel);
                this.listGachaItemPresenter.Add(presenter);
            }
        }

        private void ClearChildInGameObject(GameObject parent)
        {
            var childCount = parent.transform.childCount;

            for (var i = childCount - 1; i >= 0; i--)
            {
                var child = parent.transform.GetChild(i);
                GameObject.Destroy(child.gameObject);
            }
        }
    }
}