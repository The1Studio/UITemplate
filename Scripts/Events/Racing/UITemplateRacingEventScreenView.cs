namespace TheOneStudio.UITemplate.UITemplate.Events.Racing
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.Utilities.UIStuff;
    using GameFoundation.Scripts.Utilities.LogService;
    using GameFoundation.Signals;
    using TheOneStudio.HyperCasual.GamePlay.Models;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using TMPro;
    using UIModule.Utilities;
    using UnityEngine;
    using UnityEngine.Scripting;
    using UnityEngine.UI;

    public class UITemplateRacingEventScreenView : BaseView
    {
        public Slider                        progressSlider;
        public Button                        closeButton;
        public List<UITemplateRacingRowView> playerSliders;
        public TMP_Text                      userCurrentAmountText;

        public TMP_Text countDownText;
    }

    [PopupInfo(nameof(UITemplateRacingEventScreenView))]
    public abstract class UITemplateRacingEventScreenPresenter : UITemplateBasePopupPresenter<UITemplateRacingEventScreenView>
    {
        #region inject

        protected readonly UITemplateEventRacingDataController uiTemplateEventRacingDataController;
        private readonly   AutoCooldownTimer                   autoCooldownTimer;

        #endregion

        private List<Tween> tweenList = new();

        [Preserve]
        protected UITemplateRacingEventScreenPresenter(
            SignalBus                           signalBus,
            ILogService                         logger,
            UITemplateEventRacingDataController uiTemplateEventRacingDataController,
            AutoCooldownTimer                   autoCooldownTimer
        ) : base(signalBus, logger)
        {
            this.uiTemplateEventRacingDataController = uiTemplateEventRacingDataController;
            this.autoCooldownTimer                   = autoCooldownTimer;
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.View.closeButton.onClick.AddListener(this.OnClickClose);
            this.autoCooldownTimer.CountDown(this.uiTemplateEventRacingDataController.RemainSecond,
                _ => { this.View.countDownText.text = TimeSpan.FromSeconds(this.uiTemplateEventRacingDataController.RemainSecond).ToShortTimeString(); });

            this.InitPlayerRowView();
        }

        protected virtual void OnClickClose() { this.CloseView(); }

        private void InitPlayerRowView()
        {
            for (var i = 0; i < this.View.playerSliders.Count; i++)
            {
                var rowView    = this.View.playerSliders[i];
                var playerData = this.uiTemplateEventRacingDataController.GetPlayerData(i);
                rowView.InitView(playerData, i, this.CheckRacingEventComplete);
            }
        }

        public override UniTask BindData()
        {
            var oldShowScore = this.uiTemplateEventRacingDataController.YourOldShowScore;
            this.uiTemplateEventRacingDataController.UpdateUserOldShowScore();
            var yourNewScore = this.uiTemplateEventRacingDataController.YourNewScore;

            var yourOldProgress      = 1f * oldShowScore / this.uiTemplateEventRacingDataController.RacingScoreMax;
            var yourNewProgress      = 1f * yourNewScore / this.uiTemplateEventRacingDataController.RacingScoreMax;
            var racingMaxProgression = this.uiTemplateEventRacingDataController.RacingMaxProgression;

            this.View.userCurrentAmountText.text = $"{yourNewScore}/{this.uiTemplateEventRacingDataController.RacingScoreMax}";

            this.View.progressSlider.value                                                                   = yourOldProgress;
            this.View.playerSliders[this.uiTemplateEventRacingDataController.YourIndex].progressSlider.value = Mathf.Clamp(yourOldProgress, 0, racingMaxProgression);

            if (yourNewProgress > yourOldProgress)
            {
                this.tweenList.Add(DOTween.To(() => yourOldProgress, x =>
                {
                    this.View.progressSlider.value                                                                   = x;
                    this.View.playerSliders[this.uiTemplateEventRacingDataController.YourIndex].progressSlider.value = x;
                }, yourNewProgress, 1f).SetUpdate(true));
                this.tweenList.Add(DOTween.To(() => oldShowScore, x => { this.View.playerSliders[this.uiTemplateEventRacingDataController.YourIndex].scoreText.text = x.ToString(); }, yourNewScore, 1f)
                    .SetUpdate(true));
            }

            var simulatePlayerScore = this.uiTemplateEventRacingDataController.SimulatePlayerScore();

            foreach (var (playerIndex, oldAndNewScore) in simulatePlayerScore)
            {
                var oldProgress = 1f * oldAndNewScore.Item1 / this.uiTemplateEventRacingDataController.RacingScoreMax;
                var newProgress = 1f * oldAndNewScore.Item2 / this.uiTemplateEventRacingDataController.RacingScoreMax;

                this.View.playerSliders[playerIndex].progressSlider.value = oldProgress;
                if (newProgress > oldProgress)
                {
                    this.tweenList.Add(DOTween.To(() => oldProgress, x => { this.View.playerSliders[playerIndex].progressSlider.value = x; }, Mathf.Clamp(newProgress, 0, racingMaxProgression), 1f)
                        .SetUpdate(true));
                    this.tweenList.Add(DOTween.To(() => oldAndNewScore.Item1, x => { this.View.playerSliders[playerIndex].scoreText.text = x.ToString(); }, oldAndNewScore.Item2, 1f)
                        .SetUpdate(true));
                }
            }

            this.View.playerSliders.ForEach(item => item.CheckStatus());

            this.CheckRacingEventComplete();
            return UniTask.CompletedTask;
        }

        protected virtual void CheckRacingEventComplete()
        {
            if (!this.uiTemplateEventRacingDataController.RacingEventComplete()) return;
            // Do something
        }

        public override void Dispose()
        {
            base.Dispose();

            //Clear tween
            foreach (var tween in this.tweenList) tween.Kill();
            this.tweenList.Clear();
        }
    }
}