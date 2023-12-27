namespace TheOneStudio.UITemplate.Quests
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.Utilities.Extension;
    using TheOneStudio.UITemplate.Quests.Data;
    using TheOneStudio.UITemplate.Quests.Signals;
    using TheOneStudio.UITemplate.Quests.UI;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateQuestNotificationView : MonoBehaviour, IInitializable
    {
        [SerializeField] private Button    btn;
        [SerializeField] private Transform popup;
        [SerializeField] private Transform destination;
        [SerializeField] private TMP_Text  txtTitle;
        [SerializeField] private TMP_Text  txtContent;

        [SerializeField] private string unlockedTitle  = "New Quest";
        [SerializeField] private string completedTitle = "Quest Completed";

        private Vector3       startPosition;
        private Vector3       stopPosition;
        private SignalBus     signalBus;
        private ScreenManager screenManager;

        private void Awake()
        {
            this.startPosition = this.popup.position;
            this.stopPosition  = this.destination.position;
        }

        [Inject]
        public void Construct(SignalBus signalBus, ScreenManager screenManager)
        {
            this.signalBus     = signalBus;
            this.screenManager = screenManager;
        }

        void IInitializable.Initialize()
        {
            this.signalBus.Subscribe<QuestStatusChangedSignal>(this.OnQuestStatusChanged);
            this.btn.onClick.AddListener(() => this.screenManager.OpenScreen<UITemplateQuestPopupPresenter>().Forget());
        }

        private readonly Queue<Action> actionQueue = new Queue<Action>();

        private Tween tween;

        private void OnQuestStatusChanged(QuestStatusChangedSignal signal)
        {
            if (signal.QuestController.Record.HasTag("Chest")) return;
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

            return;

            void Action()
            {
                this.txtTitle.text = status is QuestStatus.NotCompleted
                    ? this.unlockedTitle
                    : this.completedTitle;
                this.txtContent.text = signal.QuestController.Record.Name;
                this.tween = DOTween.Sequence()
                    .Append(this.popup.DOMove(this.stopPosition, 1f).SetEase(Ease.OutBack))
                    .AppendInterval(3f)
                    .Append(this.popup.DOMove(this.startPosition, 1f).SetEase(Ease.InBack))
                    .SetUpdate(true)
                    .OnComplete(() => this.actionQueue.DequeueOrDefault()?.Invoke());
            }
        }
    }
}