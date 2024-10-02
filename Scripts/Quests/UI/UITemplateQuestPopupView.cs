namespace TheOneStudio.UITemplate.Quests.UI
{
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.Utilities.Extension;
    using GameFoundation.Scripts.Utilities.LogService;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.Quests.Data;
    using TheOneStudio.UITemplate.UITemplate.Quests.Signals;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using UnityEngine;
    using UnityEngine.Scripting;
    using UnityEngine.UI;
    #if THEONE_BADGE_NOTIFY
    using TheOneStudio.UITemplate.UITemplate.Scenes.BadgeNotify;
    #endif

    public class UITemplateQuestPopupView : BaseView
    {
        [field: SerializeField] public Button                          BtnClose    { get; private set; }
        [field: SerializeField] public Button                          BtnClaimAll { get; private set; }
        [field: SerializeField] public UITemplateQuestListView         ListView    { get; private set; }
        [field: SerializeField] public UITemplateQuestChestView        ChestView   { get; private set; }
        [field: SerializeField] public UITemplateQuestPopupTabButton[] TabButtons  { get; private set; }
    }

    [PopupInfo(nameof(UITemplateQuestPopupView))]
    public class UITemplateQuestPopupPresenter : UITemplateBasePopupPresenter<UITemplateQuestPopupView>
    {
        #region Inject

        private readonly UITemplateQuestManager questManager;

        #if THEONE_BADGE_NOTIFY
        private readonly UITemplateBadgeNotifySystem badgeNotifySystem;
        #endif

        [Preserve]
        public UITemplateQuestPopupPresenter(
            SignalBus              signalBus,
            ILogService            logger,
            UITemplateQuestManager questManager
            #if THEONE_BADGE_NOTIFY
            , UITemplateBadgeNotifySystem badgeNotifySystem
            #endif
        ) : base(signalBus, logger)
        {
            this.questManager = questManager;
            #if THEONE_BADGE_NOTIFY
            this.badgeNotifySystem = badgeNotifySystem;
            #endif
        }

        #endregion

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.View.ListView.Parent = this;
            this.View.TabButtons.ForEach(tabButton =>
            {
                tabButton.AddOnLick(() => this.SetTag(tabButton.Tab));
                #if THEONE_BADGE_NOTIFY
                this.badgeNotifySystem.RegisterBadge(tabButton.GetComponent<UITemplateBadgeNotifyView>(), this, () => this.CheckBadgeNotifyOnTabButton(tabButton));
                #endif
            });

            if (this.View.BtnClaimAll != null) this.View.BtnClaimAll.onClick.AddListener(this.ClaimAll);
            this.View.BtnClose.onClick.AddListener(() => this.CloseViewAsync().Forget());
        }

        public override UniTask BindData()
        {
            this.SetTag(this.View.TabButtons[0].Tab);

            return UniTask.CompletedTask;
        }

        private string currentTag;

        public void SetTag(string tag)
        {
            this.currentTag = tag;
            this.Refresh();
        }

        private void ClaimAll()
        {
            this.SignalBus.Fire<ClaimAllQuestSignal>();
        }

        #if THEONE_BADGE_NOTIFY
        private bool CheckBadgeNotifyOnTabButton(UITemplateQuestPopupTabButton tabButton)
        {
            var quests = this.questManager.GetAllControllers()
                .Where(quest => quest.Record.HasTag(tabButton.Tab));

            return quests.Any(quest => quest.Progress.Status == QuestStatus.NotCollected);
        }
        #endif

        public void Refresh()
        {
            this.View.TabButtons.ForEach(tabButton => tabButton.SetActive(tabButton.Tab == this.currentTag));

            var (chestQuests, normalQuests) = this.questManager.GetAllControllers()
                .Where(quest => quest.Record.HasTag(this.currentTag))
                .Split(quest => quest.Record.HasTag("Chest"));

            this.View.ListView.Model = new(normalQuests);
            this.View.ListView.BindData();

            this.View.ChestView.Dispose();
            this.View.ChestView.Model = new(chestQuests);
            this.View.ChestView.BindData();

            #if THEONE_BADGE_NOTIFY
            this.badgeNotifySystem.CheckAllBadgeNotifyStatus();
            #endif
        }

        public override void Dispose()
        {
            this.View.ChestView.Dispose();
        }
    }
}