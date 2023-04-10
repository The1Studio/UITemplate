namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main.Decoration
{
    using Cysharp.Threading.Tasks;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using UnityEngine;

    public class Decoration2DThemeItem : DecorationItem
    {
        #region Cache

        private SpriteRenderer spriteRenderer;

        #endregion

        public override async void Init(UITemplateDecorCategoryRecord record)
        {
            base.Init(record);
            if (this.spriteRenderer == null) this.spriteRenderer = this.gameObject.AddComponent<SpriteRenderer>();
            this.spriteRenderer.sprite = await this.GameAssets.LoadAssetAsync<Sprite>(this.uiTemplateInventoryDataController.GetCurrentItemSelected(record.Id));
        }
        
        public override async UniTask ChangeItem(string addressItem)
        {
            await UniTask.WaitUntil(() => this.GameAssets != null);
            if (this.spriteRenderer == null) this.spriteRenderer = this.gameObject.AddComponent<SpriteRenderer>();
            this.spriteRenderer.sprite = await this.GameAssets.LoadAssetAsync<Sprite>(addressItem);
        }
    }
}