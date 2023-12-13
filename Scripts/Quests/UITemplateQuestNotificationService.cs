namespace TheOneStudio.UITemplate.Quests
{
    using System;
    using System.Collections.Generic;
    using DG.Tweening;
    using GameFoundation.Scripts.Utilities.Extension;
    using TheOneStudio.UITemplate.Quests.Data;
    using TheOneStudio.UITemplate.Quests.Signals;
    using TMPro;
    using UnityEngine;
    using Zenject;

    public class UITemplateQuestNotificationService : MonoBehaviour, IInitializable
    {
        [SerializeField] private Transform popup;
        [SerializeField] private Transform destination;
        [SerializeField] private TMP_Text  txtTitle;
        [SerializeField] private TMP_Text  txtContent;

        [SerializeField] private string unlockedTitle  = "New Quest";
        [SerializeField] private string completedTitle = "Quest Completed";

        private SignalBus signalBus;

        [Inject]
        public void Construct(SignalBus signalBus)
        {
            this.signalBus = signalBus;
        }

        private readonly Queue<Action> actionQueue = new Queue<Action>();

        private Vector3 startPosition;
        private Vector3 stopPosition;
        private Tween   tween;

        private void Awake()
        {
            this.startPosition = this.popup.position;
            this.stopPosition  = this.destination.position;
        }

        void IInitializable.Initialize()
        {
            this.signalBus.Subscribe<QuestStatusChangedSignal>(this.OnQuestStatusChanged);
        }

        private void OnQuestStatusChanged(QuestStatusChangedSignal signal)
        {
            var status = signal.QuestController.Progress.Status;
            if (status is not QuestStatus.NotCompleted and not QuestStatus.NotCollected) return;

            if (this.tween.IsActive())
            {
                this.actionQueue.Enqueue(Action);
            }
            else
            {
                Action();
            }

            void Action()
            {
                this.txtTitle.text = status is QuestStatus.NotCompleted
                    ? this.unlockedTitle
                    : this.completedTitle;
                this.txtContent.text = signal.QuestController.Record.Description;
                this.tween = DOTween.Sequence()
                    .Append(this.popup.DOMove(this.stopPosition, 1f).SetEase(Ease.OutBack))
                    .AppendInterval(3f)
                    .Append(this.popup.DOMove(this.startPosition, 1f).SetEase(Ease.InBack))
                    .OnComplete(() => this.actionQueue.DequeueOrDefault()?.Invoke());
            }
        }
    }
}