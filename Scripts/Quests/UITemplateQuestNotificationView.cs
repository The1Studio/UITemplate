namespace TheOneStudio.UITemplate.Quests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using GameFoundation.DI;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.Utilities;
    using GameFoundation.Scripts.Utilities.Extension;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.Quests.Data;
    using TheOneStudio.UITemplate.Quests.Signals;
    using TheOneStudio.UITemplate.Quests.UI;
    using TheOneStudio.UITemplate.UITemplate.Configs.GameEvents;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UITemplateQuestNotificationView : MonoBehaviour
    {
        [SerializeField] private Button    btn;
        [SerializeField] private Transform popup;
        [SerializeField] private Transform destination;

        [SerializeField] private TMP_Text txtName;
        [SerializeField] private Slider   slider;
        [SerializeField] private Image    imgReward;
        [SerializeField] private TMP_Text txtReward;

        [SerializeField] private GameObject[] normalObjects;
        [SerializeField] private GameObject[] completedObjects;

        private SignalBus                           signalBus;
        private IScreenManager                      screenManager;
        private IGameAssets                         gameAssets;
        private IAudioService                       audioService;
        private GameFeaturesSetting                 gameFeaturesSetting;
        private UITemplateGameSessionDataController gameSessionDataController;

        private Vector3 startPosition;
        private Vector3 stopPosition;

        private void Awake()
        {
            var container = this.GetCurrentContainer();
            this.signalBus                 = container.Resolve<SignalBus>();
            this.screenManager             = container.Resolve<IScreenManager>();
            this.gameAssets                = container.Resolve<IGameAssets>();
            this.audioService              = container.Resolve<IAudioService>();
            this.gameFeaturesSetting       = container.Resolve<GameFeaturesSetting>();
            this.gameSessionDataController = container.Resolve<UITemplateGameSessionDataController>();

            this.startPosition = this.popup.position;
            this.stopPosition  = this.destination.position;
        }

        private void Start()
        {
            this.btn.onClick.AddListener(() => this.screenManager.OpenScreen<UITemplateQuestPopupPresenter>().Forget());
            this.signalBus.Subscribe<QuestStatusChangedSignal>(this.OnQuestStatusChanged);
        }

        private readonly Queue<Action> actionQueue = new Queue<Action>();

        private Tween tween;

        private void OnQuestStatusChanged(QuestStatusChangedSignal signal)
        {
            if (this.gameSessionDataController.OpenTime < this.gameFeaturesSetting.QuestSystemConfig.showNotificationAfterSession) return;
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
                var reward = signal.QuestController.Record.Rewards.Single();

                this.txtName.text = signal.QuestController.Record.Name;
                this.slider.value = 0;

                this.imgReward.sprite = this.gameAssets.LoadAssetAsync<Sprite>(reward.Image).WaitForCompletion();
                this.txtReward.text   = reward.Value.ToString();

                this.normalObjects.ForEach(obj => obj.SetActive(true));
                this.completedObjects.ForEach(obj => obj.SetActive(false));

                if (status is QuestStatus.NotCollected)
                {
                    if (!string.IsNullOrEmpty(this.gameFeaturesSetting.QuestSystemConfig.questNotificationSoundKey)) this.audioService.PlaySound(this.gameFeaturesSetting.QuestSystemConfig.questNotificationSoundKey);
                    DOTween.Sequence()
                        .AppendInterval(1)
                        .Append(this.slider.DOValue(1, .5f))
                        .AppendCallback(() =>
                        {
                            this.normalObjects.ForEach(obj => obj.SetActive(false));
                            this.completedObjects.ForEach(obj => obj.SetActive(true));
                        })
                        .Append(this.popup.DOPunchScale(Vector3.one / 3, .75f, 0, 0))
                        .SetUpdate(true);
                }

                this.tween = DOTween.Sequence()
                    .Append(this.popup.DOMove(this.stopPosition, 1).SetEase(Ease.OutBack))
                    .AppendInterval(3)
                    .Append(this.popup.DOMove(this.startPosition, 1).SetEase(Ease.InBack))
                    .OnComplete(() => this.actionQueue.DequeueOrDefault()?.Invoke())
                    .SetUpdate(true);
            }
        }
    }
}