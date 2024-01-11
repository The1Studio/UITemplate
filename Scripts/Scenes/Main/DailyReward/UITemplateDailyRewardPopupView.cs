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
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main.DailyReward.Pack;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using UnityEngine;
    using UnityEngine.Serialization;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateDailyRewardPopupView : BaseView
    {
        [FormerlySerializedAs("dailyRewardItemAdapter")] [FormerlySerializedAs("dailyRewardAdapter")]
        public UITemplateDailyRewardPackAdapter dailyRewardPackAdapter;

        public Button btnClaim;
        public Button btnClose;
    }

    public class UITemplateDailyRewardPopupModel
    {
        public Action OnClaimFinish;
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
        private List<UITemplateDailyRewardPackModel> listRewardModel;
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
                    new UITemplateDailyRewardPackModel(
                        uiTemplateDailyRewardRecord,
                        this.uiTemplateDailyRewardController.GetDateRewardStatus(uiTemplateDailyRewardRecord.Day),
                        this.OnItemClick
                    )
                ).ToList();

            this.SetUpItemCanPreReceiveWithAds();
            this.InitListDailyReward(this.listRewardModel);

            var hasRewardCanClaim = this.uiTemplateDailyRewardController.CanClaimReward;
            this.View.btnClaim.gameObject.SetActive(hasRewardCanClaim);
            this.View.btnClose.gameObject.SetActive(!hasRewardCanClaim);
            return UniTask.CompletedTask;
        }

        private void OnItemClick(UITemplateDailyRewardPackPresenter presenter)
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

        private void ClaimItemInPackReward(UITemplateDailyRewardPackPresenter presenter)
        {
            foreach (var itemPresenter in presenter.View.DailyRewardItemAdapter.GetPresenters())
            {
                itemPresenter.ClaimReward();
            }
        }

        private void ClaimAdsReward(UITemplateDailyRewardPackModel model)
        {
            this.uiTemplateAdServiceWrapper.ShowRewardedAd(this.gameFeaturesSetting.DailyRewardConfig.dailyRewardAdPlacementId, () =>
            {
                this.uiTemplateDailyRewardController.UnlockDailyReward(model.DailyRewardRecord.Day);
                this.listRewardModel[model.DailyRewardRecord.Day - 1].RewardStatus = RewardStatus.Unlocked;

                this.ClaimReward();
            });
        }

        private void SetUpItemCanPreReceiveWithAds()
        {
            switch (this.gameFeaturesSetting.DailyRewardConfig.preReceiveDailyRewardStrategy)
            {
                case PreReceiveDailyRewardStrategy.None:
                    break;
                case PreReceiveDailyRewardStrategy.NextDay:
                    NextDayPreReceiveReward();
                    break;
                case PreReceiveDailyRewardStrategy.Custom:
                    CustomPreReceiveReward();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return;

            void NextDayPreReceiveReward()
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

            void CustomPreReceiveReward()
            {
                var preReceiveConfig = this.gameFeaturesSetting.DailyRewardConfig.preReceiveConfig;
                foreach (var model in listRewardModel)
                {
                    if (model.RewardStatus == RewardStatus.Locked && preReceiveConfig.TryGetValue(model.DailyRewardRecord.Day, out var canReceive))
                    {
                        model.IsGetWithAds = canReceive;
                    }
                }
            }
        }

        private void InitListDailyReward(List<UITemplateDailyRewardPackModel> dailyRewardModels) { this.View.dailyRewardPackAdapter.InitItemAdapter(dailyRewardModels, this.diContainer).Forget(); }

        private void ClaimReward()
        {
            this.View.btnClaim.gameObject.SetActive(false);
            this.View.btnClose.gameObject.SetActive(true);
            var dayToView = new Dictionary<int, RectTransform>();
            for (var i = 0; i < this.listRewardModel.Count; i++)
            {
                if (this.listRewardModel[i].RewardStatus == RewardStatus.Unlocked)
                {
                    dayToView.Add(this.listRewardModel[i].DailyRewardRecord.Day, this.View.dailyRewardPackAdapter.GetPresenterAtIndex(i).View.transform as RectTransform);
                }
            }

            foreach (var packPresenter in this.View.dailyRewardPackAdapter.GetPresenters()
                         .Where(packPresenter => packPresenter.Model.RewardStatus == RewardStatus.Unlocked))
            {
                this.ClaimItemInPackReward(packPresenter);
            }

            this.uiTemplateDailyRewardController.ClaimAllAvailableReward(dayToView);
            this.logService.Log($"Do Animation Claim Reward");
            this.popupModel.OnClaimFinish?.Invoke();

            var claimedPresenter = new List<UITemplateDailyRewardPackPresenter>();

            for (var i = 0; i < this.listRewardModel.Count; i++)
            {
                if (this.listRewardModel[i].RewardStatus == RewardStatus.Unlocked)
                {
                    claimedPresenter.Add(this.View.dailyRewardPackAdapter.GetPresenterAtIndex(i));
                    this.listRewardModel[i].RewardStatus = RewardStatus.Claimed;
                }
            }

            this.SetUpItemCanPreReceiveWithAds();
            this.RefreshAdapter();

            // call claim reward after refresh adapter for animation
            claimedPresenter.ForEach(presenter => presenter.ClaimReward());

            this.AutoClosePopup();
        }

        private void AutoClosePopup()
        {
            if (this.gameFeaturesSetting.DailyRewardConfig.preReceiveDailyRewardStrategy != PreReceiveDailyRewardStrategy.None) return;

            UniTask.Delay(
                TimeSpan.FromSeconds(1.5f),
                cancellationToken: (this.closeViewCts = new()).Token
            ).ContinueWith(this.CloseViewAsync).Forget();
        }

        private void RefreshAdapter() { this.View.dailyRewardPackAdapter.Refresh(); }

        public override void Dispose()
        {
            base.Dispose();
            this.closeViewCts?.Cancel();
            this.closeViewCts?.Dispose();
        }
    }
}