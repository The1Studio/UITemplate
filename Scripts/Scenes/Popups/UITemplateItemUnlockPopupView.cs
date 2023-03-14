namespace TheOneStudio.UITemplate.UITemplate.Scenes.Popups
{
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.Utilities.LogService;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateItemUnlockPopupModel
    {
        public UITemplateItemUnlockPopupModel(string itemId)
        {
            this.ItemId = itemId;
        }

        public string ItemId { get; set; }
    }

    public class UITemplateItemUnlockPopupView : BaseView
    {
        [SerializeField]
        private Image imgItem;

        [SerializeField]
        private Button btnGet;

        public Button BtnGet => this.btnGet;

        public void SetView(Sprite itemSprite)
        {
            this.imgItem.sprite = itemSprite;
        }
    }

    [PopupInfo(nameof(UITemplateItemUnlockPopupView))]
    public class UITemplateItemUnlockPopupPresenter : UITemplateBasePopupPresenter<UITemplateItemUnlockPopupView, UITemplateItemUnlockPopupModel>
    {
        private readonly IGameAssets             gameAssets;
        private readonly UITemplateItemBlueprint uiTemplateItemBlueprint;

        public UITemplateItemUnlockPopupPresenter(IGameAssets gameAssets, SignalBus signalBus, ILogService logService, UITemplateItemBlueprint uiTemplateItemBlueprint) : base(signalBus, logService)
        {
            this.gameAssets              = gameAssets;
            this.uiTemplateItemBlueprint = uiTemplateItemBlueprint;
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.OpenViewAsync();
            this.InitButtonListener();
        }

        public override async void BindData(UITemplateItemUnlockPopupModel popupModel)
        {
            var itemImageAddress = this.uiTemplateItemBlueprint.Values.First(record => record.Id.Equals(popupModel.ItemId)).ImageAddress;
            var itemSprite       = await this.gameAssets.LoadAssetAsync<Sprite>(itemImageAddress);
            this.View.SetView(itemSprite);
        }

        private void OnGetItem()
        {
            this.logService.LogWithColor("Open ads here!", Color.yellow);
            this.logService.LogWithColor("Add to user data here", Color.yellow);
        }

        private void InitButtonListener()
        {
            this.View.BtnGet.onClick.AddListener(this.OnGetItem);
        }
    }
}