namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main.DailyReward
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.Utilities.LogService;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;
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
        public Action OnClaimFinish;
    }

    [PopupInfo(nameof(UITemplateDailyRewardPopupView), false)]
    public class UITemplateDailyRewardPopupPresenter : UITemplateBasePopupPresenter<UITemplateDailyRewardPopupView, UITemplateDailyRewardPopupModel>
    {
        #region inject

        private readonly DiContainer                     diContainer;
        private readonly UITemplateDailyRewardController uiTemplateDailyRewardController;
        private readonly UITemplateDailyRewardBlueprint  uiTemplateDailyRewardBlueprint;
        private readonly UITemplateLevelDataController   levelDataController;

        #endregion

        private int                                  userLoginDay;
        private UITemplateDailyRewardPopupModel      popupModel;
        private List<UITemplateDailyRewardItemModel> listRewardModel;
        private CancellationTokenSource              closeViewCts;

        public UITemplateDailyRewardPopupPresenter(
            SignalBus signalBus,
            ILogService logger,
            DiContainer diContainer,
            UITemplateDailyRewardController uiTemplateDailyRewardController,
            UITemplateDailyRewardBlueprint uiTemplateDailyRewardBlueprint,
            UITemplateLevelDataController levelDataController
        ) : base(signalBus, logger)
        {
            this.diContainer                     = diContainer;
            this.uiTemplateDailyRewardController = uiTemplateDailyRewardController;
            this.uiTemplateDailyRewardBlueprint  = uiTemplateDailyRewardBlueprint;
            this.levelDataController             = levelDataController;
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.levelDataController.UnlockFeature(Feature.DailyReward);
            this.View.btnClaim.onClick.AddListener(this.ClaimReward);
            this.View.btnClose.onClick.AddListener(this.CloseView);
            this.View.btnClose.onClick.AddListener(() => this.View.btnClose.gameObject.SetActive(false));
        }

        public override UniTask BindData(UITemplateDailyRewardPopupModel param)
        {
            this.popupModel = param;

            this.listRewardModel = this.uiTemplateDailyRewardBlueprint.Values.Select(uiTemplateDailyRewardRecord =>
                                                                                         new UITemplateDailyRewardItemModel(uiTemplateDailyRewardRecord,
                                                                                                                            this.uiTemplateDailyRewardController
                                                                                                                                .GetDateRewardStatus(uiTemplateDailyRewardRecord.Day))).ToList();

            this.InitListDailyReward(this.listRewardModel);

            var hasRewardCanClaim = this.uiTemplateDailyRewardController.CanClaimReward;
            this.View.btnClaim.gameObject.SetActive(hasRewardCanClaim);
            this.View.btnClose.gameObject.SetActive(!hasRewardCanClaim);
            return UniTask.CompletedTask;
        }

        private async void InitListDailyReward(List<UITemplateDailyRewardItemModel> dailyRewardModels) { await this.View.dailyRewardAdapter.InitItemAdapter(dailyRewardModels, this.diContainer); }

        private void ClaimReward()
        {
            this.View.btnClaim.gameObject.SetActive(false);
            this.View.btnClose.gameObject.SetActive(true);
            var claimAbleItemRectTransforms = new Dictionary<int, RectTransform>();
            for (var i = 0; i < this.listRewardModel.Count; i++)
            {
                if (this.listRewardModel[i].RewardStatus == RewardStatus.Unlocked)
                {
                    claimAbleItemRectTransforms.Add(i, this.View.dailyRewardAdapter.GetPresenterAtIndex(i).View.transform as RectTransform);
                }
            }

            this.uiTemplateDailyRewardController.ClaimAllAvailableReward(claimAbleItemRectTransforms);
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

            this.closeViewCts = new();
            UniTask.Delay(TimeSpan.FromSeconds(1.5), cancellationToken: this.closeViewCts.Token).ContinueWith(this.CloseView);
        }

        public override void Dispose()
        {
            base.Dispose();
            this.closeViewCts?.Dispose();
            this.closeViewCts = null;
        }
    }
}