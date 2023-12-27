namespace TheOneStudio.UITemplate.Quests.UI
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using GameFoundation.Scripts.Utilities.Extension;
    using GameFoundation.Scripts.Utilities.ObjectPool;
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
        [SerializeField] private Transform                   itemViewContainer;
        [SerializeField] private UITemplateQuestListItemView itemViewPrefab;

        private void Awake()
        {
            ZenjectUtils.GetCurrentContainer().Inject(this);
        }

        private ObjectPoolManager objectPoolManager;

        [Inject]
        public void Construct(ObjectPoolManager objectPoolManager)
        {
            this.objectPoolManager = objectPoolManager;
        }

        public UITemplateQuestPopupPresenter Parent { get; set; }

        public UITemplateQuestListModel Model { get; set; }

        public void BindData()
        {
            this.Model.Quests
                .OrderByDescending(quest => quest.Progress.Status is QuestStatus.NotCollected)
                .Where(quest => quest.Progress.Status is QuestStatus.NotCompleted or QuestStatus.NotCollected)
                .ForEach(quest =>
                {
                    var itemView = this.objectPoolManager.Spawn(this.itemViewPrefab, this.itemViewContainer);
                    itemView.Parent = this.Parent;
                    itemView.Model  = new(quest);
                    itemView.BindData();
                });
        }

        public void Dispose()
        {
            this.objectPoolManager.RecycleAll(this.itemViewPrefab);
        }
    }
}