namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main.DailyOffer
{
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.MVP;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UITemplateDailyQueueOfferItemView : TViewMono
    {
        [SerializeField] private TMP_Text            buttonText;
        [SerializeField] private TMP_Text            itemText;
        [SerializeField] private Image               itemImage;
        [SerializeField] private Button              claimButton;
        [SerializeField] private UITemplateAdsButton adsClaimButton;

        public TMP_Text            ButtonText     => this.buttonText;
        public TMP_Text            ItemText       => this.itemText;
        public Image               ItemImage      => this.itemImage;
        public Button              ClaimButton    => this.claimButton;
        public UITemplateAdsButton AdsClaimButton => this.adsClaimButton;
    }

    public class UITemplateDailyQueueOfferItemModel
    {
        public string OfferId { get; private set; }

        public UITemplateDailyQueueOfferItemModel(string offerId) { this.OfferId = offerId; }
    }

    public class UITemplateDailyQueueOfferItemPresenter : BaseUIItemPresenter<UITemplateDailyQueueOfferItemView, UITemplateDailyQueueOfferItemModel>
    {
        public UITemplateDailyQueueOfferItemPresenter
        (
            IGameAssets                        gameAssets,
            UITemplateDailyQueueOfferBlueprint dailyQueueOfferBlueprint
        )
            : base(gameAssets)
        {
        }

        public override void BindData(UITemplateDailyQueueOfferItemModel param) { }
    }
}