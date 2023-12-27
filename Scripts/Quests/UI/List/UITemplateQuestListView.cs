namespace TheOneStudio.UITemplate.Quests.UI
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.Utilities.Extension;
    using TheOneStudio.UITemplate.Quests.Data;
    using UnityEngine;
    using Zenject;

    public class UITemplateQuestListModel
    {
        public ReadOnlyCollection<UITemplateQuestController> Quests { get; }

        public UITemplateQuestListModel(IEnumerable<UITemplateQuestController> quests)
        {
            this.Quests = quests.ToReadOnlyCollection();
        }
    }

    public class UITemplateQuestListView : MonoBehaviour
    {
        [SerializeField] private UITemplateQuestListItemAdapter listItemAdapter;

        private void Awake()
        {
            ZenjectUtils.GetCurrentContainer().Inject(this);
        }

        private DiContainer diContainer;

        [Inject]
        public void Construct(DiContainer diContainer)
        {
            this.diContainer = diContainer;
        }

        public UITemplateQuestPopupPresenter Parent { get; set; }

        public UITemplateQuestListModel Model { get; set; }

        public void BindData()
        {
            var models = this.Model.Quests
                .Where(quest => quest.Progress.Status is QuestStatus.NotCompleted or QuestStatus.NotCollected)
                .OrderBy(quest => quest.Progress.Status is QuestStatus.NotCompleted)
                .Select(quest => new UITemplateQuestListItemModel(this.Parent, quest))
                .ToList();
            this.listItemAdapter.InitItemAdapter(models, this.diContainer).Forget();
        }
    }
}