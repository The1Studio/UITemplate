namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main.DailyReward
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.Utilities.LogService;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateDailyRewardPopupView : BaseView
    {
        public UITemplateDailyRewardAdapter dailyRewardAdapter;
        public Button                       btnClaim;
        public Button                       btnClose;
    }

    public class UITemplateDailyRewardPopupModel
    {
        public Action OnClaimFinish;
    }

    [PopupInfo(nameof(UITemplateDailyRewardPopupView))]
    public class UITemplateDailyRewardPopupPresenter : UITemplateBasePopupPresenter<UITemplateDailyRewardPopupView, UITemplateDailyRewardPopupModel>
    {
        #region inject

        private readonly DiContainer                       diContainer;
        private readonly UITemplateDailyRewardBlueprint    uiTemplateDailyRewardBlueprint;
        private readonly UITemplateDailyRewardController   uiTemplateDailyRewardController;
        private readonly UITemplateDailyRewardData         uiTemplateDailyRewardData;
        private readonly UITemplateInventoryDataController uiTemplateInventoryDataController;
        private readonly UITemplateShopBlueprint           uiTemplateShopBlueprint;

        #endregion

        private int                                  userLoginDay;
        private UITemplateDailyRewardPopupModel      popupModel;
        private List<UITemplateDailyRewardItemModel> listRewardCanClaim = new();

        public UITemplateDailyRewardPopupPresenter(SignalBus signalBus, ILogService logger, DiContainer diContainer, UITemplateDailyRewardBlueprint uiTemplateDailyRewardBlueprint,
            UITemplateDailyRewardController uiTemplateDailyRewardController, UITemplateDailyRewardData uiTemplateDailyRewardData,
            UITemplateInventoryDataController uiTemplateInventoryDataController, UITemplateShopBlueprint uiTemplateShopBlueprint) : base(signalBus, logger)
        {
            this.diContainer                       = diContainer;
            this.uiTemplateDailyRewardBlueprint    = uiTemplateDailyRewardBlueprint;
            this.uiTemplateDailyRewardController   = uiTemplateDailyRewardController;
            this.uiTemplateDailyRewardData         = uiTemplateDailyRewardData;
            this.uiTemplateInventoryDataController = uiTemplateInventoryDataController;
            this.uiTemplateShopBlueprint           = uiTemplateShopBlueprint;
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.View.btnClaim.onClick.AddListener(this.ClaimReward);
            this.View.btnClose.onClick.AddListener(this.CloseView);
        }

        public override async void BindData(UITemplateDailyRewardPopupModel param)
        {
            this.popupModel = param;
            var listRewardBlueprint = this.uiTemplateDailyRewardBlueprint.Values.ToList();
            if (this.uiTemplateDailyRewardData.RewardStatus.Count == 0 || this.CheckUserClaimAllReward())
                this.uiTemplateDailyRewardController.ResetRewardStatus(listRewardBlueprint.Count);

            var listRewardModel = listRewardBlueprint.Select(t =>
                new UITemplateDailyRewardItemModel(t, this.uiTemplateDailyRewardData.RewardStatus[t.Day - 1])).ToList();
            this.userLoginDay = await this.uiTemplateDailyRewardController.GetUserLoginDay();

            this.uiTemplateDailyRewardController.SetRewardStatus(this.userLoginDay, RewardStatus.Unlocked);

            this.InitListDailyReward(listRewardModel);

            var hasRewardCanClaim = this.uiTemplateDailyRewardData.RewardStatus.Any(t => t == RewardStatus.Unlocked);
            this.View.btnClaim.gameObject.SetActive(hasRewardCanClaim);
            this.View.btnClose.gameObject.SetActive(!hasRewardCanClaim);

            this.listRewardCanClaim = listRewardModel.Where(t => t.RewardStatus == RewardStatus.Unlocked).ToList();
        }

        private async void InitListDailyReward(List<UITemplateDailyRewardItemModel> dailyRewardModels) { await this.View.dailyRewardAdapter.InitItemAdapter(dailyRewardModels, this.diContainer); }

        private void ClaimReward()
        {
            var listRewards = this.listRewardCanClaim.Select(t => t.DailyRewardRecord.Reward).ToList();
            foreach (var rewardDict in listRewards)
            {
                foreach (var reward in rewardDict)
                {
                    if (reward.Key.Equals("Coin"))
                    {
                        this.uiTemplateInventoryDataController.AddCurrency(reward.Value, reward.Key);
                        // Do coin's reward animation
                    }
                    else
                    {
                        for (var i = 0; i < reward.Value; i++)
                            this.uiTemplateInventoryDataController.AddItemData(
                                new UITemplateItemData(reward.Key, this.uiTemplateShopBlueprint.GetDataById(reward.Key), UITemplateItemData.Status.Owned));
                        // Do item's reward animation
                    }
                }
            }

            this.uiTemplateDailyRewardController.ClaimAllAvailableReward();
            this.View.dailyRewardAdapter.Refresh();
            this.CloseView();
            this.logService.Log($"Do Animation Claim Reward");
            this.popupModel.OnClaimFinish?.Invoke();
        }

        private bool CheckUserClaimAllReward() { return !(this.uiTemplateDailyRewardData.RewardStatus.Any(t => t != RewardStatus.Claimed)); }
    }
}