namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main.Decoration
{
    using System.Collections.Generic;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main.Decoration.UI;
    using UnityEngine;
    using Zenject;

    public class UITemplateDecorationManager
    {
        private Dictionary<string, IDecorationItem> categoryToDecorationItem = new();

        #region Inject

        private readonly DiContainer                      diContainer;
        private readonly UITemplateDecorCategoryBlueprint uiTemplateDecorCategoryBlueprint;

        private UITemplateDecorationManager(DiContainer diContainer, UITemplateDecorCategoryBlueprint uiTemplateDecorCategoryBlueprint)
        {
            this.diContainer                      = diContainer;
            this.uiTemplateDecorCategoryBlueprint = uiTemplateDecorCategoryBlueprint;
            this.InitDecorItems();
        }

        #endregion

        private void InitDecorItems()
        {
            foreach (var key in this.uiTemplateDecorCategoryBlueprint.Keys)
            {
                this.CreateDecorationItem(key);
            }
        }

        private DecorationItem CreateDecorationItem(string category)
        {
            var decoration = new GameObject($"{category}_Decor").AddComponent<DecorationItem>();
            this.diContainer.InjectGameObject(decoration.gameObject);
            decoration.Init(this.uiTemplateDecorCategoryBlueprint[category]);
            this.categoryToDecorationItem.Add(category, decoration);
            return decoration;
        }

        public void OnChangeCategory(string category) { this.GetDecoration(category).ScaleItem(); }

        public void ChangeItem(string category, string addressItem) { this.GetDecoration(category).ChangeItem(addressItem); }

        public IDecorationItem GetDecoration(string category)
        {
            if (this.categoryToDecorationItem.ContainsKey(category)) return this.categoryToDecorationItem[category];
            var decoration = this.CreateDecorationItem(category);
            return decoration;
        }
    }
}