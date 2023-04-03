namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main.Decoration
{
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    public class Decoration2DThemeItem : DecorationItem
    {
        #region Cache

        private SpriteRenderer spriteRenderer;

        #endregion

        public override async UniTask ChangeItem(string addressItem)
        {
            await UniTask.WaitUntil(() => this.GameAssets != null);
            if (this.spriteRenderer == null) this.spriteRenderer = this.gameObject.AddComponent<SpriteRenderer>();
            this.spriteRenderer.sprite = await this.GameAssets.LoadAssetAsync<Sprite>(addressItem);
        }
    }
}