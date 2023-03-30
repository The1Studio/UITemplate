namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main.DailyReward
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
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

    [PopupInfo(nameof(UITemplateDailyRewardPopupView), false)]
    public class UITemplateDailyRewardPopupPresenter : UITemplateBasePopupPresenter<UITemplateDailyRewardPopupView, UITemplateDailyRewardPopupModel>
    {
        #region inject

        private readonly DiContainer                     diContainer;
        private readonly UITemplateDailyRewardController uiTemplateDailyRewardController;
        private readonly UITemplateDailyRewardData       uiTemplateDailyRewardData;
        private readonly UITemplateDailyRewardBlueprint  uiTemplateDailyRewardBlueprint;

        #endregion

        private int                                  userLoginDay;
        private UITemplateDailyRewardPopupModel      popupModel;
        private List<UITemplateDailyRewardItemModel> listRewardModel;

        public UITemplateDailyRewardPopupPresenter(SignalBus                 signalBus, ILogService logger, DiContainer diContainer, UITemplateDailyRewardController uiTemplateDailyRewardController,
                                                   UITemplateDailyRewardData uiTemplateDailyRewardData, UITemplateDailyRewardBlueprint uiTemplateDailyRewardBlueprint) : base(signalBus, logger)
        {
            this.diContainer                     = diContainer;
            this.uiTemplateDailyRewardController = uiTemplateDailyRewardController;
            this.uiTemplateDailyRewardData       = uiTemplateDailyRewardData;
            this.uiTemplateDailyRewardBlueprint  = uiTemplateDailyRewardBlueprint;
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

            this.listRewardModel = this.uiTemplateDailyRewardBlueprint.Values.Select(uiTemplateDailyRewardRecord =>
                                                                                         new UITemplateDailyRewardItemModel(uiTemplateDailyRewardRecord,
                                                                                                                            this.uiTemplateDailyRewardData.RewardStatus
                                                                                                                                [uiTemplateDailyRewardRecord.Day - 1])).ToList();
            this.userLoginDay = await this.uiTemplateDailyRewardController.GetUserLoginDay();

            this.uiTemplateDailyRewardController.SetRewardStatus(this.userLoginDay, RewardStatus.Unlocked);

            this.InitListDailyReward(this.listRewardModel);

            var hasRewardCanClaim = this.uiTemplateDailyRewardData.RewardStatus.Any(t => t == RewardStatus.Unlocked);
            this.View.btnClaim.gameObject.SetActive(hasRewardCanClaim);
            this.View.btnClose.gameObject.SetActive(!hasRewardCanClaim);
        }

        private async void InitListDailyReward(List<UITemplateDailyRewardItemModel> dailyRewardModels) { await this.View.dailyRewardAdapter.InitItemAdapter(dailyRewardModels, this.diContainer); }

        private async void ClaimReward()
        {
            this.uiTemplateDailyRewardController.ClaimAllAvailableReward();
            this.View.dailyRewardAdapter.Refresh();
            this.logService.Log($"Do Animation Claim Reward");
            this.popupModel.OnClaimFinish?.Invoke();
            foreach (var uiTemplateDailyRewardItemModel in this.listRewardModel)
            {
                if (uiTemplateDailyRewardItemModel.RewardStatus == RewardStatus.Unlocked)
                {
                    uiTemplateDailyRewardItemModel.RewardStatus = RewardStatus.Claimed;
                }
            }

            this.View.dailyRewardAdapter.Refresh();
            await UniTask.Delay(TimeSpan.FromSeconds(1.5f));
            this.CloseView();
        }
    }
}