#if THEONE_DAILY_QUEUE_REWARD
namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main.DailyOffer
{
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateDailyQueueOfferPopupView : BaseView
    {
        [SerializeField] private UITemplateCurrencyView            currencyView;
        [SerializeField] private Button                            closeButton;
        [SerializeField] private UITemplateDailyQueueOfferItemView freeOfferItemView;
        [SerializeField] private UITemplateDailyQueueOfferAdapter  rewardedAdsAdapter;

        public UITemplateCurrencyView            CurrencyView       => this.currencyView;
        public Button                            CloseButton        => this.closeButton;
        public UITemplateDailyQueueOfferItemView FreeOfferItemView  => this.freeOfferItemView;
        public UITemplateDailyQueueOfferAdapter  RewardedAdsAdapter => this.rewardedAdsAdapter;
    }

    [PopupInfo(nameof(UITemplateDailyQueueOfferPopupView), true, false, true)]
    public class UITemplateDailyQueueOfferPopupPresenter : UITemplateBasePopupPresenter<UITemplateDailyQueueOfferPopupView>
    {
        #region Inject

        private readonly UITemplateDailyQueueOfferDataController dailyOfferDataController;
        private readonly DiContainer                             diContainer;

        #endregion

        private UITemplateDailyQueueOfferItemPresenter   freeOfferItemPresenter;
        private List<UITemplateDailyQueueOfferItemModel> listModel;

        public UITemplateDailyQueueOfferPopupPresenter
        (
            SignalBus                               signalBus,
            UITemplateDailyQueueOfferDataController dailyOfferDataController,
            DiContainer                             diContainer
        )
            : base(signalBus)
        {
            this.dailyOfferDataController = dailyOfferDataController;
            this.diContainer              = diContainer;
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.diContainer.Inject(this.View.CurrencyView);
            this.View.CloseButton.onClick.AddListener(this.CloseView);
        }

        public override UniTask BindData()
        {
            this.dailyOfferDataController.CheckOfferStatus();

            this.InitFreeOfferItem();
            this.InitOrRefreshRewardAdapter().Forget();
            return UniTask.CompletedTask;
        }

        public override void Dispose()
        {
            base.Dispose();
            this.View.RewardedAdsAdapter.GetPresenters().ForEach(presenter => presenter.Dispose());
            if (this.freeOfferItemPresenter == null) return;
            
            this.freeOfferItemPresenter.Dispose();
            this.freeOfferItemPresenter = null;
        }

        private void InitFreeOfferItem()
        {
            var record = this.dailyOfferDataController.GetCurrentDailyQueueOfferRecord()
                .OfferItems.Values
                .FirstOrDefault(item => !item.IsRewardedAds);
            if (record == null)
            {
                this.View.FreeOfferItemView.gameObject.SetActive(false);
                return;
            }

            if (this.freeOfferItemPresenter == null)
            {
                this.freeOfferItemPresenter = this.diContainer.Instantiate<UITemplateDailyQueueOfferItemPresenter>();
                this.freeOfferItemPresenter.SetView(this.View.FreeOfferItemView);
                this.freeOfferItemPresenter.OnViewReady();
            }

            this.freeOfferItemPresenter.BindData(new UITemplateDailyQueueOfferItemModel(record.OfferId));
        }

        private UniTask InitOrRefreshRewardAdapter()
        {
            this.listModel ??= this.dailyOfferDataController.GetCurrentDailyQueueOfferRecord()
                .OfferItems.Values
                .Where(item => item.IsRewardedAds)
                .Select(item => new UITemplateDailyQueueOfferItemModel(item.OfferId))
                .ToList();
            return this.View.RewardedAdsAdapter.InitItemAdapter(this.listModel, this.diContainer);
        }
    }
}
#endif