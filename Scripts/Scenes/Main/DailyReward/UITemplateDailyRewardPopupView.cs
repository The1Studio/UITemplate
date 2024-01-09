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
    using TheOneStudio.UITemplate.UITemplate.Configs.GameEvents;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
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
        public bool   IsGetNextDayWithAds;
    }

    [PopupInfo(nameof(UITemplateDailyRewardPopupView), false, isOverlay: true)]
    public class UITemplateDailyRewardPopupPresenter : UITemplateBasePopupPresenter<UITemplateDailyRewardPopupView, UITemplateDailyRewardPopupModel>
    {
        #region inject

        private readonly DiContainer                     diContainer;
        private readonly UITemplateDailyRewardController uiTemplateDailyRewardController;
        private readonly UITemplateDailyRewardBlueprint  uiTemplateDailyRewardBlueprint;
        private readonly UITemplateLevelDataController   levelDataController;
        private readonly UITemplateAdServiceWrapper      uiTemplateAdServiceWrapper;
        private readonly GameFeaturesSetting             gameFeaturesSetting;

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
            UITemplateLevelDataController levelDataController,
            UITemplateAdServiceWrapper uiTemplateAdServiceWrapper,
            GameFeaturesSetting gameFeaturesSetting
        ) : base(signalBus, logger)
        {
            this.diContainer                     = diContainer;
            this.uiTemplateDailyRewardController = uiTemplateDailyRewardController;
            this.uiTemplateDailyRewardBlueprint  = uiTemplateDailyRewardBlueprint;
            this.levelDataController             = levelDataController;
            this.uiTemplateAdServiceWrapper      = uiTemplateAdServiceWrapper;
            this.gameFeaturesSetting             = gameFeaturesSetting;
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.levelDataController.UnlockFeature(UITemplateItemData.UnlockType.DailyReward);
            this.View.btnClaim.onClick.AddListener(this.ClaimReward);
            this.View.btnClose.onClick.AddListener(this.CloseView);
            this.View.btnClose.onClick.AddListener(() => this.View.btnClose.gameObject.SetActive(false));
        }

        public override UniTask BindData(UITemplateDailyRewardPopupModel param)
        {
            this.popupModel = param;

            this.listRewardModel = this.uiTemplateDailyRewardBlueprint.Values
                .Select(uiTemplateDailyRewardRecord =>
                    new UITemplateDailyRewardItemModel(
                        uiTemplateDailyRewardRecord,
                        this.uiTemplateDailyRewardController.GetDateRewardStatus(uiTemplateDailyRewardRecord.Day),
                        this.OnItemClick
                    )
                ).ToList();

            this.SetUpItemCanGetWithAds();
            this.InitListDailyReward(this.listRewardModel);

            var hasRewardCanClaim = this.uiTemplateDailyRewardController.CanClaimReward;
            this.View.btnClaim.gameObject.SetActive(hasRewardCanClaim);
            this.View.btnClose.gameObject.SetActive(!hasRewardCanClaim);
            return UniTask.CompletedTask;
        }

        private void OnItemClick(UITemplateDailyRewardItemPresenter presenter)
        {
            var model = presenter.Model;
            if (model.RewardStatus == RewardStatus.Locked && model.IsGetWithAds)
            {
                this.ClaimAdsReward(model);
            }
            else if (model.RewardStatus == RewardStatus.Unlocked)
            {
                this.ClaimReward();
            }
        }

        private void ClaimAdsReward(UITemplateDailyRewardItemModel model)
        {
            this.uiTemplateAdServiceWrapper.ShowRewardedAd(this.gameFeaturesSetting.DailyRewardConfig.dailyRewardAdPlacementId, () =>
            {
                this.uiTemplateDailyRewardController.UnlockDailyReward(model.DailyRewardRecord.Day);
                this.listRewardModel[model.DailyRewardRecord.Day - 1].RewardStatus = RewardStatus.Unlocked;

                this.ClaimReward();
            });
        }

        private void SetUpItemCanGetWithAds()
        {
            foreach (var model in this.listRewardModel)
            {
                if (model.RewardStatus == RewardStatus.Locked)
                {
                    model.IsGetWithAds = true;
                    break;
                }
            }
        }

        private void InitListDailyReward(List<UITemplateDailyRewardItemModel> dailyRewardModels) { this.View.dailyRewardAdapter.InitItemAdapter(dailyRewardModels, this.diContainer).Forget(); }

        private void ClaimReward()
        {
            this.View.btnClaim.gameObject.SetActive(false);
            this.View.btnClose.gameObject.SetActive(true);
            var dayToView = new Dictionary<int, RectTransform>();
            for (var i = 0; i < this.listRewardModel.Count; i++)
            {
                if (this.listRewardModel[i].RewardStatus == RewardStatus.Unlocked)
                {
                    dayToView.Add(this.listRewardModel[i].DailyRewardRecord.Day, this.View.dailyRewardAdapter.GetPresenterAtIndex(i).View.transform as RectTransform);
                }
            }

            this.uiTemplateDailyRewardController.ClaimAllAvailableReward(dayToView);
            this.logService.Log($"Do Animation Claim Reward");
            this.popupModel.OnClaimFinish?.Invoke();

            var claimedPresenter = new List<UITemplateDailyRewardItemPresenter>();

            for (var i = 0; i < this.listRewardModel.Count; i++)
            {
                if (this.listRewardModel[i].RewardStatus == RewardStatus.Unlocked)
                {
                    claimedPresenter.Add(this.View.dailyRewardAdapter.GetPresenterAtIndex(i));
                    this.listRewardModel[i].RewardStatus = RewardStatus.Claimed;
                }
            }

            this.SetUpItemCanGetWithAds();
            this.RefreshAdapter();

            // call claim reward after refresh adapter for animation
            claimedPresenter.ForEach(presenter => presenter.ClaimReward());

            this.AutoClosePopup();
        }

        private void AutoClosePopup()
        {
            if (this.Model.IsGetNextDayWithAds) return;

            UniTask.Delay(
                TimeSpan.FromSeconds(1.5f),
                cancellationToken: (this.closeViewCts = new()).Token
            ).ContinueWith(this.CloseViewAsync).Forget();
        }

        private void RefreshAdapter() { this.View.dailyRewardAdapter.Refresh(); }

        public override void Dispose()
        {
            base.Dispose();
            this.closeViewCts?.Cancel();
            this.closeViewCts?.Dispose();
        }
    }
}