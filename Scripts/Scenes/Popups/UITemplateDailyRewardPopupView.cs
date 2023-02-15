namespace UITemplate.Scripts.Scenes.Popups
{
    using System.Collections.Generic;
    using System.Linq;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using LocalData;
    using UITemplate.Scripts.Blueprints;
    using UITemplate.Scripts.Models;
    using UITemplate.Scripts.Scenes.Utils;
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
        private readonly DiContainer                    diContainer;
        private readonly UserLocalData                  localData;
        private readonly UITemplateDailyRewardBlueprint uiTemplateDailyRewardBlueprint;

        private int                                      userLoginDay;
        private List<UITemplateDailyRewardItemPresenter> dailyRewardItemPresenters = new();

        public UITemplateDailyRewardPopupPresenter(SignalBus signalBus, DiContainer diContainer, UserLocalData localData, UITemplateDailyRewardBlueprint uiTemplateDailyRewardBlueprint) :
            base(signalBus)
        {
            this.diContainer                    = diContainer;
            this.localData                      = localData;
            this.uiTemplateDailyRewardBlueprint = uiTemplateDailyRewardBlueprint;
        }

        public override async void BindData()
        {
            this.userLoginDay                                             = await this.localData.RewardData.GetUserLoginDay();
            this.localData.RewardData.RewardStatus[this.userLoginDay - 1] = RewardStatus.Unlocked;
            for (var i = 0; i < this.View.DailyRewardItemViews.Count; i++)
            {
                this.dailyRewardItemPresenters.Add(this.diContainer.Instantiate<UITemplateDailyRewardItemPresenter>());
                this.dailyRewardItemPresenters[i].SetView(this.View.DailyRewardItemViews[i]);
                this.dailyRewardItemPresenters[i].BindData(new UITemplateDailyRewardItemModel(this.uiTemplateDailyRewardBlueprint.Values.First(record => record.Day == i + 1)));
            }
        }
    }
}