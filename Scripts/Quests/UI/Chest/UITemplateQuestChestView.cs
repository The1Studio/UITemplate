namespace TheOneStudio.UITemplate.Quests.UI
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using GameFoundation.Scripts.Utilities.Extension;
    using UnityEngine;

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
        public UITemplateQuestPopupPresenter Parent { get; set; }

        public UITemplateQuestChestModel Model { get; set; }

        public void BindData()
        {
        }

        public void Dispose()
        {
        }
    }
}