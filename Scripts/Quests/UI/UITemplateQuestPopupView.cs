namespace TheOneStudio.UITemplate.Quests.UI
{
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.Utilities.Extension;
    using TheOneStudio.UITemplate.Quests.Data;
    #if THEONE_BADGE_NOTIFY
    using TheOneStudio.UITemplate.UITemplate.Scenes.BadgeNotify;
    #endif
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateQuestPopupView : BaseView
    {
        [field: SerializeField] public Button                          BtnClose   { get; private set; }
        [field: SerializeField] public UITemplateQuestListView         ListView   { get; private set; }
        [field: SerializeField] public UITemplateQuestChestView        ChestView  { get; private set; }
        [field: SerializeField] public UITemplateQuestPopupTabButton[] TabButtons { get; private set; }
    }

    [PopupInfo(nameof(UITemplateQuestPopupView))]
    public class UITemplateQuestPopupPresenter : UITemplateBasePopupPresenter<UITemplateQuestPopupView>
    {
        #region Inject

        private readonly UITemplateQuestManager questManager;

        #if THEONE_BADGE_NOTIFY
        private readonly UITemplateBadgeNotifySystem badgeNotifySystem;
        #endif

        public UITemplateQuestPopupPresenter(
            SignalBus              signalBus,
            UITemplateQuestManager questManager
            #if THEONE_BADGE_NOTIFY
            ,UITemplateBadgeNotifySystem badgeNotifySystem
            #endif
        ) : base(signalBus)
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
                this.badgeNotifySystem.RegisterButton(tabButton.GetComponent<UITemplateBadgeNotifyButtonView>(), this, null, () => this.CheckBadgeNotifyOnTabButton(tabButton));
                #endif
            });
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