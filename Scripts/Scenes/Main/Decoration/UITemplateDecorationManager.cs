namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main.Decoration
{
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
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
            
            this.InitDecorItems();
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

                this.CreateDecorationItemThenHide(key);
            }
        }

        private async UniTask CreateDecorationItemThenHide(string key) => (await this.CreateDecorationItem(key)).HideItem();

        public void HideDecorItems()
        {
            foreach (var item in this.categoryToDecorationItem)
            {
                item.Value.HideItem();
            }
        }

        public async UniTask ShowDecorItems()
        {
            var showItemTasks = new List<UniTask>();
            foreach (var item in this.categoryToDecorationItem)
            {
                showItemTasks.Add(item.Value.ShowItem());
            }

            await UniTask.WhenAll(showItemTasks);
        }

        private async UniTask<IDecorationItem> CreateDecorationItem(string category)
        {
            var decoration = this.uiTemplateDecorCategoryBlueprint[category].Mode is DecorationMode.Theme2D
                ? await this.CreateTheme2D(category)
                : await this.CreateTheme3D(category);
            this.diContainer.InjectGameObject(decoration.gameObject);
            decoration.Init(this.uiTemplateDecorCategoryBlueprint[category]);
            this.categoryToDecorationItem.Add(category, decoration);
            return decoration;
        }

        private string GetDefaultItemId(string category)
        {
            return this.uiTemplateItemBlueprint.First(x => x.Value.Category.Equals(category) && x.Value.IsDefaultItem).Value.Id;
        }

        private async UniTask<DecorationItem> CreateTheme2D(string category)
        {
            return new GameObject($"{category}").AddComponent<Decoration2DThemeItem>();
        }

        private async UniTask<DecorationItem> CreateTheme3D(string category)
        {
            return new GameObject($"{category}").AddComponent<Decoration3DThemeItem>();
        }

        public async void OnChangeCategory(string category) { (await this.GetDecoration(category)).ScaleItem(); }

        public async void ChangeItem(string category, string addressItem) { (await this.GetDecoration(category)).ChangeItem(addressItem); }

        public async UniTask<IDecorationItem> GetDecoration(string category)
        {
            if (this.categoryToDecorationItem.ContainsKey(category)) return this.categoryToDecorationItem[category];
            return await this.CreateDecorationItem(category);
        }
    }
}