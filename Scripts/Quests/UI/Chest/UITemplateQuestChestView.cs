namespace TheOneStudio.UITemplate.Quests.UI
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using GameFoundation.Scripts.Utilities.Extension;
    using GameFoundation.Scripts.Utilities.ObjectPool;
    using TheOneStudio.UITemplate.Quests.Data;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;

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
            this.sld.value = (float)this.Model.Quests.Count(quest => quest.Progress.Status.HasFlag(QuestStatus.Completed))
                / this.Model.Quests.Count;
        }

        public void Dispose()
        {
            this.objectPoolManager.RecycleAll(this.itemViewPrefab);
        }
    }
}