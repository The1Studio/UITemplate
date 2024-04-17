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
        [SerializeField] private UITemplateCurrencyView           currencyView;
        [SerializeField] private Button                           closeButton;
        [SerializeField] private UITemplateDailyQueueOfferAdapter rewardedAdsAdapter;

        public UITemplateCurrencyView           CurrencyView       => this.currencyView;
        public Button                           CloseButton        => this.closeButton;
        public UITemplateDailyQueueOfferAdapter RewardedAdsAdapter => this.rewardedAdsAdapter;
    }

    [PopupInfo(nameof(UITemplateDailyQueueOfferPopupView), true, false, true)]
    public class
        UITemplateDailyQueueOfferPopupPresenter : UITemplateBasePopupPresenter<UITemplateDailyQueueOfferPopupView>
    {
#region Inject

        private readonly UITemplateDailyQueueOfferDataController dailyOfferDataController;
        private readonly DiContainer                             diContainer;

#endregion

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

            this.InitOrRefreshRewardAdapter().Forget();

            return UniTask.CompletedTask;
        }

        public override void Dispose()
        {
            base.Dispose();
            this.View.RewardedAdsAdapter.GetPresenters().ForEach(presenter => presenter.Dispose());
        }

        private UniTask InitOrRefreshRewardAdapter()
        {
            this.listModel ??= this.dailyOfferDataController.GetCurrentDailyQueueOfferRecord()
               .OfferItems.Values
               .Select(item => new UITemplateDailyQueueOfferItemModel(item.OfferId))
               .ToList();

            return this.View.RewardedAdsAdapter.InitItemAdapter(this.listModel, this.diContainer);
        }
    }
}
#endif