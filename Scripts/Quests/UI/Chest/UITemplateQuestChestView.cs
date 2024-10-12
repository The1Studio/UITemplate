namespace TheOneStudio.UITemplate.Quests.UI
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using GameFoundation.DI;
    using GameFoundation.Scripts.Utilities.Extension;
    using GameFoundation.Scripts.Utilities.ObjectPool;
    using TheOneStudio.UITemplate.Quests.Data;
    using UnityEngine;
    using UnityEngine.UI;

    public class UITemplateQuestChestModel
    {
        public ReadOnlyCollection<UITemplateQuestController> Quests { get; }

        public UITemplateQuestChestModel(IEnumerable<UITemplateQuestController> quests)
        {
            this.Quests = quests.ToReadOnlyCollection();
        }
    }

    public class UITemplateQuestChestView : MonoBehaviour
    {
        [SerializeField] private Transform                    itemViewContainer;
        [SerializeField] private UITemplateQuestChestItemView itemViewPrefab;
        [SerializeField] private Slider                       sld;

        private ObjectPoolManager objectPoolManager;

        private void Awake()
        {
            var container = this.GetCurrentContainer();
            this.objectPoolManager = container.Resolve<ObjectPoolManager>();
        }

        public UITemplateQuestChestModel Model { get; set; }

        public void BindData()
        {
            if (this.Model.Quests.Count == 0)
            {
                this.gameObject.SetActive(false);
                return;
            }
            this.gameObject.SetActive(true);
            this.Model.Quests.ForEach(quest =>
            {
                var itemView = this.objectPoolManager.Spawn(this.itemViewPrefab, this.itemViewContainer);
                itemView.Model = new(quest);
                itemView.BindData();
            });
            if (this.Model.Quests.All(quest => quest.Progress.Status.HasFlag(QuestStatus.Completed)))
            {
                this.sld.value = 1;
            }
            else
            {
                var progressHandler = this.Model.Quests.Last().GetCompleteProgressHandlers().Last();
                this.sld.value = progressHandler.CurrentProgress / progressHandler.MaxProgress;
            }
        }

        public void Dispose()
        {
            this.objectPoolManager.RecycleAll(this.itemViewPrefab);
        }
    }
}