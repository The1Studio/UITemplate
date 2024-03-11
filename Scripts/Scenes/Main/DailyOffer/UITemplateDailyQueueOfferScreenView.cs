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
    public class UITemplateDailyQueueOfferScreenPresenter : UITemplateBasePopupPresenter<UITemplateDailyQueueOfferScreenView>
    {
        #region Inject

        private readonly UITemplateDailyQueueOfferDataController dailyOfferDataController;
        private readonly DiContainer                             diContainer;

        #endregion

        private List<UITemplateDailyQueueOfferItemModel> listFreeModel;
        private List<UITemplateDailyQueueOfferItemModel> listRewardedAdsModel;

        public UITemplateDailyQueueOfferScreenPresenter
        (
            SignalBus                               signalBus,
            UITemplateDailyQueueOfferDataController dailyOfferDataController,
            DiContainer                             diContainer
        )
            : base(signalBus)
        {
            this.dailyOfferDataController = dailyOfferDataController;
            this.diContainer         = diContainer;
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.View.CloseButton.onClick.AddListener(this.CloseView);
        }

        public override UniTask BindData()
        {
            this.listFreeModel = this.dailyOfferDataController.GetCurrentDailyQueueOfferRecord()
                .OfferItems.Values
                .Where(item => !item.IsRewardedAds)
                .Select(item => new UITemplateDailyQueueOfferItemModel(item.OfferId))
                .ToList();
            this.listRewardedAdsModel = this.dailyOfferDataController.GetCurrentDailyQueueOfferRecord()
                .OfferItems.Values
                .Where(item => item.IsRewardedAds)
                .Select(item => new UITemplateDailyQueueOfferItemModel(item.OfferId))
                .ToList();

            this.dailyOfferDataController.CheckOfferStatus();
            this.View.FreeOfferAdapter.InitItemAdapter(this.listFreeModel, this.diContainer);
            this.View.RewardedAdsAdapter.InitItemAdapter(this.listRewardedAdsModel, this.diContainer);
            return UniTask.CompletedTask;
        }
    }
}