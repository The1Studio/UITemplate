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
        [SerializeField] private Button                            closeButton;
        [SerializeField] private UITemplateDailyQueueOfferItemView freeOfferItemView;
        [SerializeField] private UITemplateDailyQueueOfferAdapter  rewardedAdsAdapter;

        public Button                            CloseButton        => this.closeButton;
        public UITemplateDailyQueueOfferItemView FreeOfferItemView  => this.freeOfferItemView;
        public UITemplateDailyQueueOfferAdapter  RewardedAdsAdapter => this.rewardedAdsAdapter;
    }

    [PopupInfo(nameof(UITemplateDailyQueueOfferScreenView), true, false, true)]
    public class UITemplateDailyQueueOfferPopupPresenter : UITemplateBasePopupPresenter<UITemplateDailyQueueOfferScreenView>
    {
        #region Inject

        private readonly UITemplateDailyQueueOfferDataController dailyOfferDataController;
        private readonly DiContainer                             diContainer;

        #endregion

        private UITemplateDailyQueueOfferItemPresenter freeOfferItemPresenter;

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

            this.InitFreeOfferItem();
            this.InitRewardAdapter().Forget();
            return UniTask.CompletedTask;
        }

        public override void Dispose()
        {
            base.Dispose();
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

            this.freeOfferItemPresenter.BindData(new UITemplateDailyQueueOfferItemModel(record.ImageId));
        }

        private UniTask InitRewardAdapter()
        {
            var listModel = this.dailyOfferDataController.GetCurrentDailyQueueOfferRecord()
                .OfferItems.Values
                .Where(item => item.IsRewardedAds)
                .Select(item => new UITemplateDailyQueueOfferItemModel(item.OfferId))
                .ToList();

            return this.View.RewardedAdsAdapter.InitItemAdapter(listModel, this.diContainer);
        }
    }
}