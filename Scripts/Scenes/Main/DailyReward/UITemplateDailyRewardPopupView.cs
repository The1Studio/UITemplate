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
    using UnityEngine;
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
        public Action OnClaim;
    }

    [PopupInfo(nameof(UITemplateDailyRewardPopupView))]
    public class UITemplateDailyRewardPopupPresenter : UITemplateBasePopupPresenter<UITemplateDailyRewardPopupView, UITemplateDailyRewardPopupModel>
    {
        #region inject

        private readonly DiContainer                     diContainer;
        private readonly UITemplateDailyRewardBlueprint  uiTemplateDailyRewardBlueprint;
        private readonly UITemplateDailyRewardController uiTemplateDailyRewardController;
        private readonly UITemplateDailyRewardData       uiTemplateDailyRewardData;

        #endregion

        private int                             userLoginDay;
        private UITemplateDailyRewardPopupModel popupModel;

        public UITemplateDailyRewardPopupPresenter(SignalBus signalBus, ILogService logger, DiContainer diContainer, UITemplateDailyRewardBlueprint uiTemplateDailyRewardBlueprint,
            UITemplateDailyRewardController uiTemplateDailyRewardController, UITemplateDailyRewardData uiTemplateDailyRewardData) : base(signalBus, logger)
        {
            this.diContainer                     = diContainer;
            this.uiTemplateDailyRewardBlueprint  = uiTemplateDailyRewardBlueprint;
            this.uiTemplateDailyRewardController = uiTemplateDailyRewardController;
            this.uiTemplateDailyRewardData       = uiTemplateDailyRewardData;
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
            var listRewardModel     = listRewardBlueprint.Select(t => new UITemplateDailyRewardItemModel(t)).ToList();
            this.userLoginDay = await this.uiTemplateDailyRewardController.GetUserLoginDay();
            if (this.uiTemplateDailyRewardData.RewardStatus.Count == 0 || this.CheckUserClaimAllReward())
                this.uiTemplateDailyRewardController.ResetRewardStatus(listRewardBlueprint.Count);
            this.uiTemplateDailyRewardController.SetRewardStatus(this.userLoginDay, RewardStatus.Unlocked);

            this.InitListDailyReward(listRewardModel);

            var hasRewardCanClaim = this.uiTemplateDailyRewardData.RewardStatus.Any(t => t == RewardStatus.Unlocked);
            this.View.btnClaim.gameObject.SetActive(hasRewardCanClaim);
            this.View.btnClose.gameObject.SetActive(!hasRewardCanClaim);
        }

        private async void InitListDailyReward(List<UITemplateDailyRewardItemModel> dailyRewardModels) { await this.View.dailyRewardAdapter.InitItemAdapter(dailyRewardModels, this.diContainer); }

        private void ClaimReward()
        {
            this.uiTemplateDailyRewardController.GetAllAvailableReward();
            this.View.dailyRewardAdapter.Refresh();
            this.CloseView();
            this.logService.Log($"Do Animation Claim Reward");
            this.popupModel.OnClaim?.Invoke();
        }

        private bool CheckUserClaimAllReward() { return !(this.uiTemplateDailyRewardData.RewardStatus.Any(t => t != RewardStatus.Claimed)); }
    }
}