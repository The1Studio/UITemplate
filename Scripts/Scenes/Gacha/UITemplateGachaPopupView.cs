namespace TheOneStudio.UITemplate.UITemplate.Scenes.Gacha
{
    using System;
    using System.Collections.Generic;
    using Com.TheFallenGames.OSA.Util;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.Utilities.LogService;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateGachaPopupView : BaseView
    {
        public Button                         btnClose;
        public UITemplateGachaAdapter         uiTemplateGachaAdapter;
        public UITemplateScrollViewPagingView uiTemplateScrollViewPagingView;
        public Button                         btnUnlockRandom;
        public Button                         btnWatchAds;
    }

    public class UITemplateGachaPopupModel
    {
        public List<UITemplateGachaPageModel> GachaPageModels = new();
    }

    [PopupInfo(nameof(UITemplateGachaPopupView))]
    public class UITemplateGachaPopupPresenter : BasePopupPresenter<UITemplateGachaPopupView, UITemplateGachaPopupModel>
    {
        private readonly DiContainer                diContainer;
        private readonly UITemplateAdServiceWrapper uiTemplateAdServiceWrapper;
        private readonly EventSystem                eventSystem;

        private UITemplateScrollViewPagingPresenter uiTemplateScrollViewPagingPresenter;
        private UITemplateGachaPopupModel           model;
        private int                                 currentIndex = 0;
        private IDisposable                         randomTimerDispose;

        public UITemplateGachaPopupPresenter(SignalBus signalBus, ILogService logService, DiContainer diContainer,
            UITemplateAdServiceWrapper uiTemplateAdServiceWrapper) : base(signalBus, logService)
        {
            this.diContainer                = diContainer;
            this.uiTemplateAdServiceWrapper = uiTemplateAdServiceWrapper;
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.View.btnClose.onClick.AddListener(this.CloseView);
            this.View.btnUnlockRandom.onClick.AddListener(this.UnlockRandom);
            this.View.btnWatchAds.onClick.AddListener(this.WatchAds);
        }

        public override void BindData(UITemplateGachaPopupModel param)
        {
            this.model = param;
            this.InitGachaAdapter();
        }

        private async void InitGachaAdapter()
        {
            await this.View.uiTemplateGachaAdapter.InitItemAdapter(this.model.GachaPageModels, this.diContainer);
            this.View.uiTemplateGachaAdapter.GetComponent<OSASnapperFocusedItemInfo>().FocusedItemIndexChanged.AddListener(this.OnFocusedItemIndexChanged);
            this.InitPaging();
        }

        private void OnFocusedItemIndexChanged(int index)
        {
            if (this.uiTemplateScrollViewPagingPresenter == null)
                return;

            //Change the paging index
            Debug.Log($"the index changed: {index}");
            this.currentIndex = index < 0 ? this.currentIndex : index;
            this.uiTemplateScrollViewPagingPresenter.SetActiveDot(index);
        }

        private void InitPaging()
        {
            if (this.uiTemplateScrollViewPagingPresenter != null)
                return;

            var pagingModel = new UITemplateScrollViewPagingModel();
            for (var i = 0; i < this.model.GachaPageModels.Count; i++)
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

        private void UnlockRandom()
        {
            this.model.GachaPageModels[this.currentIndex].OnRandomGachaItem?.Invoke();
        }

        protected virtual void WatchAds() { this.uiTemplateAdServiceWrapper.ShowRewardedAd("Shop", this.RewardedAdsWhenComplete); }

        protected virtual void RewardedAdsWhenComplete() { Debug.Log($"Rewarded in Shop"); }

        public override void Dispose()
        {
            base.Dispose();
            this.uiTemplateScrollViewPagingPresenter = null;
            this.randomTimerDispose?.Dispose();
        }
    }
}