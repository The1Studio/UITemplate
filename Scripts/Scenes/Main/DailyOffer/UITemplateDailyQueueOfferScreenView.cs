namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main.DailyOffer
{
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateDailyQueueOfferScreenView : BaseView
    {
        [SerializeField] private Button                           closeButton;
        [SerializeField] private UITemplateDailyQueueOfferAdapter freeOfferAdapter;
        [SerializeField] private UITemplateDailyQueueOfferAdapter rewardedAdsAdapter;

        public Button                           CloseButton        => this.closeButton;
        public UITemplateDailyQueueOfferAdapter FreeOfferAdapter   => this.freeOfferAdapter;
        public UITemplateDailyQueueOfferAdapter RewardedAdsAdapter => this.rewardedAdsAdapter;
    }

    [PopupInfo(nameof(UITemplateDailyQueueOfferScreenView), true, false, true)]
    public class UITemplateDailyQueueOfferPopupPresenter : UITemplateBasePopupPresenter<UITemplateDailyQueueOfferScreenView>
    {
        #region Inject

        private readonly UITemplateDailyQueueOfferDataController dailyOfferDataController;
        private readonly DiContainer                             diContainer;

        #endregion

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
            this.View.CloseButton.onClick.AddListener(this.CloseView);
        }

        public override UniTask BindData()
        {
            this.dailyOfferDataController.CheckOfferStatus();

            this.InitAdapter(this.View.FreeOfferAdapter,   false).Forget();
            this.InitAdapter(this.View.RewardedAdsAdapter, true).Forget();
            return UniTask.CompletedTask;
        }

        private UniTask InitAdapter(UITemplateDailyQueueOfferAdapter adapter, bool isRewardedAds)
        {
            var listModel = this.dailyOfferDataController.GetCurrentDailyQueueOfferRecord()
                .OfferItems.Values
                .Where(item => item.IsRewardedAds == isRewardedAds)
                .Select(item => new UITemplateDailyQueueOfferItemModel(item.OfferId))
                .ToList();

            return adapter.InitItemAdapter(listModel, this.diContainer);
        }
    }
}