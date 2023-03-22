namespace TheOneStudio.UITemplate.UITemplate.Scenes.Gacha
{
    using System.Collections.Generic;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.MVP;
    using UnityEngine;
    using Zenject;

    public class UITemplateScrollViewPagingView: TViewMono
    {
        public UITemplatePagingDotItemView pagingDotItemView;
    }

    public class UITemplateScrollViewPagingModel
    {
        public List<UITemplatePagingDotItemModel> ListPagingDotItemModel = new();
    }
    
    public class UITemplateScrollViewPagingPresenter: BaseUIItemPresenter<UITemplateScrollViewPagingView, UITemplateScrollViewPagingModel>
    {
        private readonly DiContainer diContainer;

        private List<UITemplatePagingDotItemPresenter> listPagingDotItemPresenter = new();

        public UITemplateScrollViewPagingPresenter(IGameAssets gameAssets, DiContainer diContainer) : base(gameAssets) { this.diContainer = diContainer; }

        public override void BindData(UITemplateScrollViewPagingModel param)
        {
            this.ClearChildInGameObject(this.View.gameObject);
            foreach (var pagingDotItemModel in param.ListPagingDotItemModel)
            {
                var presenter = this.diContainer.Instantiate<UITemplatePagingDotItemPresenter>();
                var view    = GameObject.Instantiate(this.View.pagingDotItemView, this.View.transform);
                presenter.SetView(view);
                presenter.BindData(pagingDotItemModel);
                this.listPagingDotItemPresenter.Add(presenter);
            }
        }

        public void SetActiveDot(int index)
        {
            foreach (var pageItemPresenter in this.listPagingDotItemPresenter)
            {
                pageItemPresenter.SetDotActive(false);
            }
            this.listPagingDotItemPresenter[index].SetDotActive(true);
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