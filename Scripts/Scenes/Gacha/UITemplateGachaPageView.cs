namespace TheOneStudio.UITemplate.UITemplate.Scenes.Gacha
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.MVP;
    using TheOneStudio.UITemplate.UITemplate.Extension;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using Zenject;

    public class UITemplateGachaPageView : TViewMono
    {
        public UITemplateGachaPageContentAdapter gachaPageContentAdapter;
    }

    public class UITemplateGachaPageModel
    {
        public int                            PageId        { get; set; }
        public List<UITemplateGachaItemModel> ListGachaItem { get; set; } = new();
        public Action                         OnRandomGachaItem;
    }

    public class UITemplateGachaPagePresenter : BaseUIItemPresenter<UITemplateGachaPageView, UITemplateGachaPageModel>
    {
        private readonly DiContainer              diContainer;
        private readonly EventSystem              eventSystem;
        private          UITemplateGachaPageModel model;
        private          IDisposable              randomTimerDispose;

        public UITemplateGachaPagePresenter(IGameAssets gameAssets, DiContainer diContainer, EventSystem eventSystem) : base(gameAssets)
        {
            this.diContainer = diContainer;
            this.eventSystem = eventSystem;
        }

        public override void BindData(UITemplateGachaPageModel param)
        {
            this.model                   = param;
            this.model.OnRandomGachaItem = this.OnRandomGachaItem;
            this.InitListItems();
        }

        private async void InitListItems()
        {
            foreach (var gachaItemModel in this.model.ListGachaItem)
            {
                gachaItemModel.OnSelect = this.OnSelectGachaItem;
            }

            await this.View.gachaPageContentAdapter.InitItemAdapter(this.model.ListGachaItem, this.diContainer);
        }

        private void OnSelectGachaItem(UITemplateGachaItemModel selectedGachaItemModel)
        {
            this.model.ListGachaItem.ForEach(o => o.IsSelected = false);
            this.model.ListGachaItem.FirstOrDefault(o => o.ItemId == selectedGachaItemModel.ItemId)!.IsSelected = true;
            this.View.gachaPageContentAdapter.Refresh();
        }

        private void OnRandomGachaItem()
        {
            this.model.ListGachaItem.ForEach(o => o.IsSelected = false);
            var lockedList = this.model.ListGachaItem.Where(o => o.IsLocked).ToList();
            var maxTime    = this.model.ListGachaItem.Select(o => o.IsLocked).ToList().Count == 1 ? 0.3f : 3;
            this.eventSystem.enabled = false;
            lockedList.GachaItemWithTimer(this.randomTimerDispose,
                this.OnUnlockRandomComplete, this.OnSelectGachaItem, maxTime, 0.1f);
        }

        protected virtual void OnUnlockRandomComplete(UITemplateGachaItemModel gachaItemModel)
        {
            gachaItemModel.IsLocked  = false;
            this.eventSystem.enabled = true;
            this.OnSelectGachaItem(gachaItemModel);
        }

        public override void Dispose()
        {
            base.Dispose();
            this.randomTimerDispose?.Dispose();
            this.View.gachaPageContentAdapter.ResetItems(0);
            this.model               = new UITemplateGachaPageModel
            {
                ListGachaItem = new List<UITemplateGachaItemModel>()
            };
        }
    }
}