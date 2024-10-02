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
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Configs.GameEvents;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main.DailyReward.Pack;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using UnityEngine;
    using UnityEngine.Scripting;
    using UnityEngine.Serialization;
    using UnityEngine.UI;

    public class UITemplateDailyRewardPopupView : BaseView
    {
        [FormerlySerializedAs("dailyRewardItemAdapter")]
        [FormerlySerializedAs("dailyRewardAdapter")]
        public UITemplateDailyRewardPackAdapter dailyRewardPackAdapter;

        public Button btnClaim;
        public Button btnClose;
        public string claimSoundKey;
    }

    public class UITemplateDailyRewardPopupModel
    {
    }

    [PopupInfo(nameof(UITemplateDailyRewardPopupView), false, isOverlay: true)]
    public class UITemplateDailyRewardPopupPresenter : UITemplateBasePopupPresenter<UITemplateDailyRewardPopupView, UITemplateDailyRewardPopupModel>
    {
        #region inject

        private readonly UITemplateDailyRewardController uiTemplateDailyRewardController;
        private readonly UITemplateDailyRewardBlueprint  uiTemplateDailyRewardBlueprint;
        private readonly UITemplateLevelDataController   levelDataController;
        private readonly UITemplateAdServiceWrapper      uiTemplateAdServiceWrapper;
        private readonly DailyRewardAnimationHelper      dailyRewardAnimationHelper;
        private readonly GameFeaturesSetting             gameFeaturesSetting;

        #endregion

        private UITemplateDailyRewardPopupModel      popupModel;
        private List<UITemplateDailyRewardPackModel> listRewardModel;
        private CancellationTokenSource              closeViewCts;

        [Preserve]
        public UITemplateDailyRewardPopupPresenter(
            SignalBus                       signalBus,
            ILogService                     logger,
            UITemplateDailyRewardController uiTemplateDailyRewardController,
            UITemplateDailyRewardBlueprint  uiTemplateDailyRewardBlueprint,
            UITemplateLevelDataController   levelDataController,
            UITemplateAdServiceWrapper      uiTemplateAdServiceWrapper,
            DailyRewardAnimationHelper      dailyRewardAnimationHelper,
            GameFeaturesSetting             gameFeaturesSetting
        ) : base(signalBus, logger)
        {
            this.uiTemplateDailyRewardController = uiTemplateDailyRewardController;
            this.uiTemplateDailyRewardBlueprint  = uiTemplateDailyRewardBlueprint;
            this.levelDataController             = levelDataController;
            this.uiTemplateAdServiceWrapper      = uiTemplateAdServiceWrapper;
            this.dailyRewardAnimationHelper      = dailyRewardAnimationHelper;
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
                                       )
                                       .ToList();

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
            switch (model.RewardStatus)
            {
                case RewardStatus.Locked when model.IsGetWithAds:
                    this.ClaimAdsReward(model);
                    break;
                case RewardStatus.Unlocked:
                    this.ClaimReward();
                    break;
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
                    if (model.RewardStatus == RewardStatus.Locked &&
                        preReceiveConfig.TryGetValue(model.DailyRewardRecord.Day, out var canReceive))
                    {
                        model.IsGetWithAds = canReceive;
                    }
                }
            }
        }

        private void InitListDailyReward(List<UITemplateDailyRewardPackModel> dailyRewardModels) { this.View.dailyRewardPackAdapter.InitItemAdapter(dailyRewardModels).Forget(); }

        private void ClaimReward() { this.ClaimRewardAsync().Forget(); }

        private async UniTask ClaimRewardAsync()
        {
            this.View.btnClaim.gameObject.SetActive(false);
            this.View.btnClose.gameObject.SetActive(true);

            await this.dailyRewardAnimationHelper.PlayPreClaimRewardAnimation(this);

            var dayToView = new Dictionary<int, RectTransform>();
            for (var i = 0; i < this.listRewardModel.Count; i++)
            {
                if (this.listRewardModel[i].RewardStatus == RewardStatus.Unlocked)
                {
                    dayToView.Add(this.listRewardModel[i].DailyRewardRecord.Day, this.View.dailyRewardPackAdapter.GetPresenterAtIndex(i).View.transform as RectTransform);
                }
            }

            this.uiTemplateDailyRewardController.ClaimAllAvailableReward(dayToView, this.View.claimSoundKey);

            var claimedPresenter = new List<UITemplateDailyRewardPackPresenter>();

            for (var i = 0; i < this.listRewardModel.Count; i++)
            {
                if (this.listRewardModel[i].RewardStatus == RewardStatus.Unlocked)
                {
                    claimedPresenter.Add(this.View.dailyRewardPackAdapter.GetPresenterAtIndex(i));
                    this.listRewardModel[i].RewardStatus = RewardStatus.Claimed;
                }
            }

            await this.dailyRewardAnimationHelper.PlayPostClaimRewardAnimation(this);

            this.SetUpItemCanPreReceiveWithAds();
            this.RefreshAdapter();

            // call claim reward after refresh adapter for animation
            claimedPresenter.ForEach(presenter => presenter.ClaimReward());

            this.AutoClosePopup();
        }

        private void AutoClosePopup()
        {
            if (this.gameFeaturesSetting.DailyRewardConfig.preReceiveDailyRewardStrategy != PreReceiveDailyRewardStrategy.None) return;

            UniTask.Delay(TimeSpan.FromSeconds(1.5f),
                          cancellationToken: (this.closeViewCts = new()).Token, ignoreTimeScale:true)
                   .ContinueWith(this.CloseViewAsync)
                   .Forget();
        }

        private void RefreshAdapter() { this.View.dailyRewardPackAdapter.Refresh(); }

        public override void Dispose()
        {
            base.Dispose();
            this.closeViewCts?.Cancel();
            this.closeViewCts?.Dispose();
            this.closeViewCts = null;
        }
    }
}