namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main.DailyReward
{
    using System.Collections.Generic;
    using System.Linq;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
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
    }

    [PopupInfo(nameof(UITemplateDailyRewardPopupView))]
    public class UITemplateDailyRewardPopupPresenter : UITemplateBasePopupPresenter<UITemplateDailyRewardPopupView>
    {
        #region inject

        private readonly DiContainer                     diContainer;
        private readonly UITemplateDailyRewardBlueprint  uiTemplateDailyRewardBlueprint;
        private readonly UITemplateDailyRewardController uiTemplateDailyRewardController;
        private readonly UITemplateDailyRewardData       uiTemplateDailyRewardData;

        #endregion

        private int userLoginDay;

        public UITemplateDailyRewardPopupPresenter(SignalBus signalBus, DiContainer diContainer, UITemplateDailyRewardBlueprint uiTemplateDailyRewardBlueprint,
            UITemplateDailyRewardController uiTemplateDailyRewardController, UITemplateDailyRewardData uiTemplateDailyRewardData) : base(signalBus)
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
        }

        public override async void BindData()
        {
            var listRewardBlueprint = this.uiTemplateDailyRewardBlueprint.Values.ToList();
            var listRewardModel     = listRewardBlueprint.Select(t => new UITemplateDailyRewardItemModel(t)).ToList();
            this.userLoginDay = await this.uiTemplateDailyRewardController.GetUserLoginDay();
            if (this.uiTemplateDailyRewardData.RewardStatus.Count == 0)
                this.uiTemplateDailyRewardController.ResetRewardStatus(listRewardBlueprint.Count);
            this.uiTemplateDailyRewardController.SetRewardStatus(this.userLoginDay, RewardStatus.Unlocked);

            this.InitListDailyReward(listRewardModel);
        }

        private async void InitListDailyReward(List<UITemplateDailyRewardItemModel> dailyRewardModels) { await this.View.dailyRewardAdapter.InitItemAdapter(dailyRewardModels, this.diContainer); }

        private async void ClaimReward() { this.uiTemplateDailyRewardController.SetRewardStatus(await this.uiTemplateDailyRewardController.GetUserLoginDay(), RewardStatus.Claimed); }
    }
}