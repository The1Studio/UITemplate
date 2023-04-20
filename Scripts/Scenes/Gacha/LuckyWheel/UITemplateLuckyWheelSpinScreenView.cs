namespace TheOneStudio.UITemplate.UITemplate.Scenes.Gacha.LuckyWheel
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.Utilities.LogService;
    using TheOneStudio.UITemplate.UITemplate.Blueprints.Gacha;
    using TheOneStudio.UITemplate.UITemplate.Extension;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateLuckyWheelSpinModel
    {
        public bool                            IsFreeSpin     { get; set; }
        public float                           RotateDuration { get; set; } = 4f;
        public int                             CircleTimes    { get; set; } = 3;
        public List<UITemplateLuckySpinRecord> SpinRecords    { get; set; } = new();
        public int                             ForceSpinIndex { get; set; } = -1;
        public Action<int, RectTransform>      OnSpinComplete { get; set; }
    }

    public class UITemplateLuckyWheelSpinScreenView : BaseView
    {
        public GameObject                         rotateObject;
        public Button                             btnSpin, btnAdsSpin;
        public List<UITemplateLuckyWheelSpinItem> spinItemList;
        public AnimationCurve                     animationCurve;
        public GameObject                         objCircleNoteParent;
        public GameObject                         arrowOn, arrowOff;
        public List<CircleNote>                   circleNotes;
        public RectTransform                      animationCoin;
        public GameObject                         blockInput;
        public Button                             noThankButton;
    }

    [PopupInfo(nameof(UITemplateLuckyWheelSpinScreenView), false)]
    public class UITemplateLuckyWheelSpinScreenPresenter : UITemplateBasePopupPresenter<UITemplateLuckyWheelSpinScreenView, UITemplateLuckyWheelSpinModel>
    {
        private readonly EventSystem                   eventSystem;
        private readonly DiContainer                   diContainer;
        private readonly UITemplateAdServiceWrapper    uiTemplateAdServiceWrapper;
        private readonly UITemplateAnimationHelper     uiTemplateAnimationHelper;
        private readonly UITemplateLevelDataController levelDataController;

        private Tween spinTween;

        private const int DeltaDegree = 45;

        private const int                                         CircleDegree    = 360;
        private const string                                      RewardPlacement = "lucky_wheel_spin";
        private       int                                         lastRewardIndex;
        private       List<UITemplateLuckyWheelSpinItemPresenter> spinItemPresenters = new();

        public UITemplateLuckyWheelSpinScreenPresenter(
            SignalBus signalBus,
            EventSystem eventSystem,
            DiContainer diContainer,
            UITemplateAdServiceWrapper uiTemplateAdServiceWrapper,
            ILogService logger,
            UITemplateAnimationHelper uiTemplateAnimationHelper,
            UITemplateLevelDataController levelDataController
        ) : base(signalBus, logger)
        {
            this.eventSystem                = eventSystem;
            this.diContainer                = diContainer;
            this.uiTemplateAdServiceWrapper = uiTemplateAdServiceWrapper;
            this.uiTemplateAnimationHelper  = uiTemplateAnimationHelper;
            this.levelDataController        = levelDataController;
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.levelDataController.UnlockFeature(Feature.LuckySpin);
            this.View.btnSpin.onClick.AddListener(this.OnFreeSpin);
            this.View.btnAdsSpin.onClick.AddListener(this.OnAdsSpin);
            this.View.noThankButton.onClick.AddListener(this.CloseView);
        }

        private void OnAdsSpin() { this.uiTemplateAdServiceWrapper.ShowRewardedAd(RewardPlacement, this.DoSpin); }

        private void OnFreeSpin() { this.DoSpin(); }

        private async void DoSpin()
        {
            this.View.blockInput.SetActive(true);
            this.eventSystem.gameObject.SetActive(false);
            this.SetActiveButtons(false);
            var currentTime = 0f;
            var weights     = new List<float>();
            var prizes      = new List<int>();

            for (var i = 0; i < this.Model.SpinRecords.Count; i++)
            {
                weights.Add(this.Model.SpinRecords[i].Weight);
                prizes.Add(i);
            }

            var rewardTargetIndex = prizes.RandomGachaWithWeight(weights);

            if (this.Model.ForceSpinIndex != -1)
            {
                rewardTargetIndex = this.Model.ForceSpinIndex;
            }

            var startAngle = this.View.rotateObject.transform.localEulerAngles.z;
            var angelWant  = startAngle - CircleDegree * this.Model.CircleTimes - (rewardTargetIndex - this.lastRewardIndex) * DeltaDegree;

            this.lastRewardIndex = rewardTargetIndex;

            while (currentTime < this.Model.RotateDuration)
            {
                await UniTask.WaitForEndOfFrame();
                this.FlashCircleNote(0.1f * (currentTime + 1) / 2);
                currentTime += Time.deltaTime;
                var currentAngle = angelWant * this.View.animationCurve.Evaluate(currentTime / this.Model.RotateDuration);
                this.View.rotateObject.transform.localEulerAngles = new Vector3(0, 0, currentAngle);
            }
            
            this.SetActiveButtons(true);
            this.SetActiveWatchAdsButton(true);

            this.FlashCircleNote(0.5f);

            this.View.arrowOff.SetActive(false);
            this.View.arrowOn.SetActive(true);

            this.eventSystem.gameObject.SetActive(true);
            this.Model.OnSpinComplete?.Invoke(this.lastRewardIndex, this.View.animationCoin);
            this.View.blockInput.SetActive(false);
        }

        private void SetActiveButtons(bool isActive)
        {
            this.View.btnSpin.gameObject.SetActive(isActive);
            this.View.btnAdsSpin.gameObject.SetActive(isActive);
            this.View.noThankButton.gameObject.SetActive(isActive);
        }

        private void SetActiveWatchAdsButton(bool isActive, bool force = false) => this.uiTemplateAnimationHelper.SetActiveFreeObject(this.View.btnAdsSpin.gameObject, this.View.noThankButton.gameObject, this.View.btnSpin.gameObject, !isActive, force);

        private void FlashCircleNote(float startTime)
        {
            foreach (var c in this.View.circleNotes)
            {
                c.ChangeColor(startTime);
            }
        }

        public override UniTask BindData(UITemplateLuckyWheelSpinModel model)
        {
            this.View.blockInput.SetActive(false);
            this.SetActiveWatchAdsButton(!model.IsFreeSpin, true);
            this.lastRewardIndex                              = 0;
            this.View.rotateObject.transform.localEulerAngles = Vector3.zero;
            this.View.objCircleNoteParent.SetActive(true);
            this.View.arrowOff.SetActive(true);
            this.View.arrowOn.SetActive(false);
            this.FlashCircleNote(0.5f);

            if (model.SpinRecords.Count == this.View.spinItemList.Count)
            {
                this.InitItemLuckySpin();
            }
            else
            {
                this.logService.Error($"Error: SpinRecords.Count != spinItemList.Count");
            }

            return UniTask.CompletedTask;
        }

        private void InitItemLuckySpin()
        {
            if (this.spinItemPresenters.Count == 0)
            {
                foreach (var view in this.View.spinItemList)
                {
                    var p = this.diContainer.Instantiate<UITemplateLuckyWheelSpinItemPresenter>();
                    p.SetView(view);

                    this.spinItemPresenters.Add(p);
                }
            }

            for (var index = 0; index < this.spinItemPresenters.Count; index++)
            {
                var p = this.spinItemPresenters[index];

                p.BindData(new UITemplateLuckyWheelSpinItemModel()
                {
                    ItemIndex = index,
                    Icon      = this.Model.SpinRecords[index].Icon,
                    Value     = this.Model.SpinRecords[index].Rewards[UITemplateInventoryDataController.DefaultSoftCurrencyID]
                });
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            this.View.objCircleNoteParent.SetActive(false);
        }
    }
}