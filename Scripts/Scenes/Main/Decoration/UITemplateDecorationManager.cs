namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main.Decoration
{
    using System.Collections.Generic;
    using System.Linq;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main.Decoration.UI;
    using UnityEngine;
    using Zenject;

    public class UITemplateDecorationManager
    {
        private readonly Dictionary<string, IDecorationItem> categoryToDecorationItem = new();

        #region Inject

        private readonly DiContainer                       diContainer;
        private readonly UITemplateDecorCategoryBlueprint  uiTemplateDecorCategoryBlueprint;
        private readonly UITemplateInventoryDataController uiTemplateInventoryDataController;
        private readonly UITemplateItemBlueprint           uiTemplateItemBlueprint;

        private UITemplateDecorationManager(DiContainer diContainer, UITemplateDecorCategoryBlueprint uiTemplateDecorCategoryBlueprint, UITemplateInventoryDataController uiTemplateInventoryDataController, UITemplateItemBlueprint uiTemplateItemBlueprint)
        {
            this.diContainer                       = diContainer;
            this.uiTemplateDecorCategoryBlueprint  = uiTemplateDecorCategoryBlueprint;
            this.uiTemplateInventoryDataController = uiTemplateInventoryDataController;
            this.uiTemplateItemBlueprint           = uiTemplateItemBlueprint;
        }

        #endregion

        public void InitDecorItems()
        {
            foreach (var key in this.uiTemplateDecorCategoryBlueprint.Keys)
            {
                if (this.uiTemplateInventoryDataController.GetCurrentItemSelected(key) == null)
                {
                    this.uiTemplateInventoryDataController.UpdateCurrentSelectedItem(key, this.GetDefaultItemId(key));
                }
                
                this.CreateDecorationItem(key);
            }
        }

        public void HideDecorItems()
        {
            foreach (var item in this.categoryToDecorationItem)
            {
                item.Value.HideItem();
            }
        }

        public void ShowDecorItems()
        {
            foreach (var item in this.categoryToDecorationItem)
            {
                item.Value.ShowItem();
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

        private string GetDefaultItemId(string category)
        {
            return this.uiTemplateItemBlueprint.First(x => x.Value.Category.Equals(category) && x.Value.IsDefaultItem).Value.Id;
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