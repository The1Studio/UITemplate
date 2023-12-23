namespace TheOneStudio.UITemplate.Quests.UI
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using GameFoundation.Scripts.Utilities.Extension;
    using UnityEngine;

    public class UITemplateQuestChestItemModel
    {
        public ReadOnlyCollection<UITemplateQuestController> Quests { get; }

        public UITemplateQuestChestItemModel(IEnumerable<UITemplateQuestController> quests)
        {
            this.Quests = quests.ToReadOnlyCollection();
        }
    }

    public class UITemplateQuestChestItemView : MonoBehaviour
    {
        public UITemplateQuestPopupPresenter Parent { get; set; }

        public UITemplateQuestChestItemModel Model { get; set; }

        public void BindData()
        {
        }

        public void Dispose()
        {
        }
    }
}