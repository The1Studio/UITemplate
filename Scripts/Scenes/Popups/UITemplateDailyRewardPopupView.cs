namespace TheOneStudio.UITemplate.UITemplate.Scenes.Popups
{
    using System.Collections.Generic;
    using System.Linq;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using UnityEngine;
    using Zenject;

    public class UITemplateDailyRewardPopupView : BaseView
    {
        [SerializeField] private List<UITemplateDailyRewardItemView> dailyRewardItemViews;

        public List<UITemplateDailyRewardItemView> DailyRewardItemViews => this.dailyRewardItemViews;
    }

    [PopupInfo(nameof(UITemplateDailyRewardPopupView))]
    public class UITemplateDailyRewardPopupPresenter : BasePopupPresenter<UITemplateDailyRewardPopupView>
    {
        #region inject

        private readonly DiContainer                     diContainer;
        private readonly UITemplateDailyRewardBlueprint  uiTemplateDailyRewardBlueprint;
        private readonly UITemplateDailyRewardController uiTemplateDailyRewardController;
        private readonly UITemplateDailyRewardData       uiTemplateDailyRewardData;

        #endregion

        private int                                      userLoginDay;
        private List<UITemplateDailyRewardItemPresenter> dailyRewardItemPresenters = new();

        public UITemplateDailyRewardPopupPresenter(SignalBus                       signalBus, DiContainer diContainer, UITemplateDailyRewardBlueprint uiTemplateDailyRewardBlueprint,
                                                   UITemplateDailyRewardController uiTemplateDailyRewardController, UITemplateDailyRewardData uiTemplateDailyRewardData) : base(signalBus)
        {
            this.diContainer                     = diContainer;
            this.uiTemplateDailyRewardBlueprint  = uiTemplateDailyRewardBlueprint;
            this.uiTemplateDailyRewardController = uiTemplateDailyRewardController;
            this.uiTemplateDailyRewardData       = uiTemplateDailyRewardData;
        }

        public override async void BindData()
        {
            this.userLoginDay                                                  = await this.uiTemplateDailyRewardController.GetUserLoginDay();
            this.uiTemplateDailyRewardData.RewardStatus[this.userLoginDay - 1] = RewardStatus.Unlocked;
            for (var i = 0; i < this.View.DailyRewardItemViews.Count; i++)
            {
                this.dailyRewardItemPresenters.Add(this.diContainer.Instantiate<UITemplateDailyRewardItemPresenter>());
                this.dailyRewardItemPresenters[i].SetView(this.View.DailyRewardItemViews[i]);
                this.dailyRewardItemPresenters[i].BindData(new UITemplateDailyRewardItemModel(this.uiTemplateDailyRewardBlueprint.Values.First(record => record.Day == i + 1)));
            }
        }
    }
}