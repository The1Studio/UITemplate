namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main.Decoration
{
    using System.Collections.Generic;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main.Decoration.UI;
    using UnityEngine;
    using Zenject;

    public class UITemplateDecorationManager
    {
        private readonly Dictionary<string, IDecorationItem> categoryToDecorationItem = new();

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

        private IDecorationItem CreateDecorationItem(string category)
        {
            var decoration = this.uiTemplateDecorCategoryBlueprint[category].Mode is DecorationMode.Theme2D
                ? this.CreateTheme2D(category)
                : this.CreateTheme3D(category);
            this.diContainer.InjectGameObject(decoration.gameObject);
            decoration.Init(this.uiTemplateDecorCategoryBlueprint[category]);
            this.categoryToDecorationItem.Add(category, decoration);
            return decoration;
        }

        private DecorationItem CreateTheme2D(string category) { return new GameObject($"{category}").AddComponent<Decoration2DThemeItem>(); }

        private DecorationItem CreateTheme3D(string category) { return new GameObject($"{category}").AddComponent<Decoration3DThemeItem>(); }

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