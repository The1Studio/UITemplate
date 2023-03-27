namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main.Jackpot
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Com.TheFallenGames.OSA.Core;
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.Utilities.LogService;
    using GameFoundation.Scripts.Utilities.Utils;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateJackpotSpinScreenView : BaseView
    {
        public Button                       btnClaim;
        public Button                       btnSpin;
        public UITemplateJackpotItemAdapter jackpotItemAdapter;
    }

    public class UITemplateJackpotSpinScreenModel
    {
        public Action<UITemplateJackpotItemModel> OnClaim;

        public UITemplateJackpotSpinScreenModel(Action<UITemplateJackpotItemModel> onClaim) { this.OnClaim = onClaim; }
    }

    [ScreenInfo(nameof(UITemplateJackpotSpinScreenView))]
    public class UITemplateJackpotSpinScreenPresenter : UITemplateBaseScreenPresenter<UITemplateJackpotSpinScreenView, UITemplateJackpotSpinScreenModel>
    {
        private readonly EventSystem                    eventSystem;
        private readonly DiContainer                    diContainer;
        private readonly UITemplateJackpotItemBlueprint uiTemplateJackpotItemBlueprint;

        private Snapper8                         snapper8;
        private List<UITemplateJackpotItemModel> listJackpotItemModels = new();
        private UITemplateJackpotItemModel       currentJackpotItem;

        public UITemplateJackpotSpinScreenPresenter(SignalBus signalBus, ILogService logger, EventSystem eventSystem, DiContainer diContainer,
            UITemplateJackpotItemBlueprint uiTemplateJackpotItemBlueprint) : base(signalBus, logger)
        {
            this.eventSystem                    = eventSystem;
            this.diContainer                    = diContainer;
            this.uiTemplateJackpotItemBlueprint = uiTemplateJackpotItemBlueprint;
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.View.btnClaim.onClick.AddListener(this.OnClickClaim);
            this.View.btnSpin.onClick.AddListener(this.OnClickSpin);
        }

        public override void BindData(UITemplateJackpotSpinScreenModel screenModel)
        {
            this.Model = screenModel;
            this.InitListJackpotItem();
        }

        private void OnClickSpin() { this.DoSpin(); }

        private void OnClickClaim()
        {
            if (this.currentJackpotItem == null) return;
            this.Model.OnClaim?.Invoke(this.currentJackpotItem);
        }

        private async void InitListJackpotItem()
        {
            this.listJackpotItemModels = this.uiTemplateJackpotItemBlueprint.Values.Select(record => new UITemplateJackpotItemModel(record.Id)).Shuffle().ToList();
            await this.View.jackpotItemAdapter.InitItemAdapter(this.listJackpotItemModels, this.diContainer);
            this.snapper8         = this.View.jackpotItemAdapter.GetComponent<Snapper8>();
            this.snapper8.enabled = false;
        }

        private void DoSpin()
        {
            this.View.jackpotItemAdapter.Velocity = Vector2.left * 10000;

            _ = DOTween.To(() => this.View.jackpotItemAdapter.Velocity.x, value => this.View.jackpotItemAdapter.Velocity = new Vector2(value, 0), -600, 4).SetEase(Ease.InOutQuint)
                .OnUpdate(() => this.eventSystem.gameObject.SetActive(false))
                .OnComplete((this.OnSpinFinish));
        }

        private async void OnSpinFinish()
        {
            this.snapper8.enabled = true;
            await UniTask.Delay((int)(this.snapper8.snapDuration * 1000));
            this.eventSystem.gameObject.SetActive(true);

            for (var i = 0; i < this.listJackpotItemModels.Count; i++)
            {
                if (this.View.jackpotItemAdapter.GetItemSignedVisibility(i) == 0)
                {
                    this.currentJackpotItem = this.listJackpotItemModels[i];
                }
            }

            this.View.jackpotItemAdapter.ForceUpdateFullVisibleItems();
        }
    }
}