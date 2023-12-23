namespace TheOneStudio.UITemplate.Quests.UI
{
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.Utilities.Extension;
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

        public UITemplateQuestPopupPresenter(
            SignalBus              signalBus,
            UITemplateQuestManager questManager
        ) : base(signalBus)
        {
            this.questManager = questManager;
        }

        #endregion

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.View.ListView.Parent  = this;
            this.View.ChestView.Parent = this;
            this.View.TabButtons.ForEach(tabButton => tabButton.Parent = this);
            this.View.BtnClose.onClick.AddListener(() => this.CloseViewAsync().Forget());
        }

        public override UniTask BindData()
        {
            this.Tab = this.View.TabButtons[0].Tab;
            return UniTask.CompletedTask;
        }

        public void Refresh()
        {
            this.View.TabButtons.ForEach(tabButton => tabButton.SetActive(tabButton.Tab == this.Tab));

            var (chestQuests, normalQuests) = this.questManager.GetAllControllers()
                .Where(quest => quest.Record.Tags.Contains(this.Tab))
                .Split(quest => quest.Record.Tags.Contains("Chest"));

            this.View.ListView.Dispose();
            this.View.ListView.Model = new(normalQuests);
            this.View.ListView.BindData();

            this.View.ChestView.Dispose();
            this.View.ChestView.Model = new(chestQuests);
            this.View.ChestView.BindData();
        }

        public override void Dispose()
        {
            this.View.ListView.Dispose();
            this.View.ChestView.Dispose();
        }

        public string Tab
        {
            get => this.tab;
            set
            {
                this.tab = value;
                this.Refresh();
            }
        }

        private string tab;
    }
}