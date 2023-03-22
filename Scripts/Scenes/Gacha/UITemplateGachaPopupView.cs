namespace TheOneStudio.UITemplate.UITemplate.Scenes.Gacha
{
    using System.Collections.Generic;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.Utilities.LogService;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateGachaPopupView : BaseView
    {
        public Button                         btnClose;
        public UITemplateGachaItemAdapter     uiTemplateGachaItemAdapter;
        public UITemplateScrollViewPagingView uiTemplateScrollViewPagingView;
    }

    public class UITemplateGachaPopupModel
    {
        public List<UITemplateGachaPageModel> GachaPageModels = new();

        public UITemplateGachaPopupModel FakePageData()
        {
            var       result = new UITemplateGachaPopupModel();
            const int count  = 3;
            for (var i = 0; i < count; i++)
            {
                result.GachaPageModels.Add(new UITemplateGachaPageModel
                {
                    PageId        = i,
                    ListGachaItem = this.FakeListItemInPageData()
                });
            }

            return result;
        }

        private List<UITemplateGachaItemModel> FakeListItemInPageData()
        {
            var result = new List<UITemplateGachaItemModel>();
            var count  = Random.Range(1, 9);
            for (var i = 0; i < count; i++)
            {
                result.Add(new UITemplateGachaItemModel
                {
                    ItemId     = i,
                    IsSelected = Random.Range(0, 1) == 0,
                });
            }

            return result;
        }
    }

    [PopupInfo(nameof(UITemplateGachaPopupView))]
    public class UITemplateGachaPopupPresenter : BasePopupPresenter<UITemplateGachaPopupView, UITemplateGachaPopupModel>
    {
        private readonly DiContainer diContainer;

        private UITemplateScrollViewPagingPresenter uiTemplateScrollViewPagingPresenter;

        public UITemplateGachaPopupPresenter(SignalBus signalBus, ILogService logService, DiContainer diContainer) : base(signalBus, logService) { this.diContainer = diContainer; }

        public override void BindData(UITemplateGachaPopupModel param)
        {
            this.InitGachaAdapter(param);
            this.View.btnClose.onClick.AddListener(this.CloseView);
        }

        private async void InitGachaAdapter(UITemplateGachaPopupModel model)
        {
            await this.View.uiTemplateGachaItemAdapter.InitItemAdapter(model.GachaPageModels, this.diContainer);
            this.InitPaging(model);
        }

        private void InitPaging(UITemplateGachaPopupModel model)
        {
            if (this.uiTemplateScrollViewPagingPresenter != null)
                return;

            var pagingModel = new UITemplateScrollViewPagingModel();
            for (var i = 0; i < model.GachaPageModels.Count; i++)
            {
                pagingModel.ListPagingDotItemModel.Add(new UITemplatePagingDotItemModel
                {
                    IsSelected = i == 0,
                });
            }
            
            this.uiTemplateScrollViewPagingPresenter = this.diContainer.Instantiate<UITemplateScrollViewPagingPresenter>();
            this.uiTemplateScrollViewPagingPresenter.SetView(this.View.uiTemplateScrollViewPagingView);
            this.uiTemplateScrollViewPagingPresenter.BindData(pagingModel);
        }
    }
}