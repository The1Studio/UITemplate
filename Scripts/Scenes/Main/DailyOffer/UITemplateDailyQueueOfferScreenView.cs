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

        private readonly UITemplateDailyQueueOfferDataController offerDataController;

        #endregion

        private IEnumerable<UITemplateDailyQueueOfferItemModel> listFreeModel;
        private IEnumerable<UITemplateDailyQueueOfferItemModel> listRewardedAdsModel;

        public UITemplateDailyQueueOfferScreenPresenter
        (
            SignalBus                               signalBus,
            UITemplateDailyQueueOfferDataController offerDataController
        )
            : base(signalBus)
        {
            this.offerDataController = offerDataController;
        }

        public override UniTask BindData()
        {
            this.listFreeModel = this.offerDataController.GetCurrentDailyQueueOfferRecord()
                .OfferItems.Values.Where(item => !item.IsRewardedAds).Select(item => new UITemplateDailyQueueOfferItemModel(item.OfferId));
            this.listRewardedAdsModel = this.offerDataController.GetCurrentDailyQueueOfferRecord()
                .OfferItems.Values.Where(item => item.IsRewardedAds).Select(item => new UITemplateDailyQueueOfferItemModel(item.OfferId));

            return UniTask.CompletedTask;
        }
    }
}