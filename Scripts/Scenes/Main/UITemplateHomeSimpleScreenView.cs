namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main
{
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Gacha;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Gacha.Jackpot;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main.Decoration.UI;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateHomeSimpleScreenView : BaseView
    {
        public Button                      PlayButton;
        public Button                      LevelButton;
        public Button                      ShopButton;
        public UITemplateCurrencyView      CoinText;
        public UITemplateSettingButtonView SettingButtonView;
    }

    [ScreenInfo(nameof(UITemplateHomeSimpleScreenView))]
    public class UITemplateHomeSimpleScreenPresenter : UITemplateBaseScreenPresenter<UITemplateHomeSimpleScreenView>
    {
        public UITemplateHomeSimpleScreenPresenter(SignalBus signalBus, DiContainer diContainer, IScreenManager screenManager, UITemplateInventoryDataController uiTemplateInventoryDataController) :
            base(signalBus)
        {
            this.diContainer                       = diContainer;
            this.ScreenManager                     = screenManager;
            this.uiTemplateInventoryDataController = uiTemplateInventoryDataController;
        }

        protected override async void OnViewReady()
        {
            base.OnViewReady();
            await this.OpenViewAsync();
            this.diContainer.Inject(this.View.SettingButtonView);
            this.View.PlayButton.onClick.AddListener(this.OnClickPlay);
            this.View.LevelButton.onClick.AddListener(this.OnClickLevel);
            this.View.ShopButton.onClick.AddListener(this.OnClickShop);
        }

        protected virtual void OnClickLevel()
        {
            this.ScreenManager.OpenScreen<UITemplateJackpotSpinPopupPresenter, UITemplateJackpotSpinPopupModel>(new UITemplateJackpotSpinPopupModel(() =>
            {
                Debug.Log($"Get Reward from Jackpot Complete");
            }));
        }

        protected virtual void OnClickShop() { }

        private UITemplateGachaPopupModel FakeGachaPage()
        {
            var       result = new UITemplateGachaPopupModel();
            const int count  = 3;

            for (var i = 0; i < count; i++)
            {
                result.GachaPageModels.Add(new UITemplateGachaPageModel { PageId = i, ListGachaItem = this.FakeListItemInPageData() });
            }

            return result;
        }

        private List<UITemplateGachaItemModel> FakeListItemInPageData()
        {
            var result = new List<UITemplateGachaItemModel>();
            var count  = Random.Range(4, 10);

            for (var i = 0; i < count; i++)
            {
                result.Add(new UITemplateGachaItemModel { ItemId = i, IsSelected = i == 0, IsLocked = i > 0 && Random.Range(0, 2) == 0 });
            }

            return result.OrderBy(o => o.IsLocked).ToList();
        }

        protected virtual void OnClickPlay()
        {
            // this.uiTemplateInventoryDataController.UpdateCurrency(1000);
            // this.ScreenManager.OpenScreen<UITemplateDecorScreenPresenter>();
        }

        public override UniTask BindData()
        {
            this.View.CoinText.Subscribe(this.SignalBus, this.uiTemplateInventoryDataController.GetCurrencyValue());
            return UniTask.CompletedTask;
        }

        public override void Dispose()
        {
            base.Dispose();
            this.View.CoinText.Unsubscribe(this.SignalBus);
        }

        #region inject

        protected readonly DiContainer                       diContainer;
        protected readonly IScreenManager                    ScreenManager;
        protected readonly UITemplateInventoryDataController uiTemplateInventoryDataController;

        #endregion
    }
}