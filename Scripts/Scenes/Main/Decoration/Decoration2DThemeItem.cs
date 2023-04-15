namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main.Decoration
{
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using UnityEngine;

    public class Decoration2DThemeItem : DecorationItem
    {
        #region Cache

        private SpriteRenderer                spriteRenderer;

        #endregion

        public override async UniTask Init(UITemplateDecorCategoryRecord record)
        {
            base.Init(record);
            
            this.signalBus.Subscribe<ScaleDecoration2DItem>(this.OnChangeScale);
            
            if (this.spriteRenderer == null) this.spriteRenderer = this.gameObject.AddComponent<SpriteRenderer>();
            this.spriteRenderer.sprite       = await this.GameAssets.LoadAssetAsync<Sprite>(this.uiTemplateInventoryDataController.GetCurrentItemSelected(record.Id));
            this.spriteRenderer.sortingOrder = record.Layer;
            
            if (this.record.IsScaleRoot)
            {
                this.AdjustScale();   
            }
        }   

        public override async UniTask ChangeItem(string addressItem)
        {
            await UniTask.WaitUntil(() => this.GameAssets != null);
            if (this.spriteRenderer == null) this.spriteRenderer = this.gameObject.AddComponent<SpriteRenderer>();
            this.spriteRenderer.sprite = await this.GameAssets.LoadAssetAsync<Sprite>(addressItem);
        }

        private void AdjustScale()
        {
            var camera = Camera.main;

            var worldScreenHeight = camera.orthographicSize      * 2;
            var worldScreenWidth  = worldScreenHeight / Screen.height * Screen.width;

            var boundsSize = this.spriteRenderer.sprite.bounds.size;
            this.transform.localScale = new Vector3(
                                                    worldScreenWidth  / boundsSize.x,
                                                    worldScreenHeight / boundsSize.y, 1);
            
            this.signalBus.Fire(new ScaleDecoration2DItem(worldScreenWidth  / boundsSize.x, worldScreenHeight / boundsSize.y));
        }

        private void OnChangeScale(ScaleDecoration2DItem signal)
        {
            this.transform.localScale = new Vector3(signal.WidthScale, signal.HeightScale, 1);
        }

        public override UniTask HideItem()
        {
            this.gameObject.SetActive(false);
            return UniTask.CompletedTask;
        }

        public override UniTask ShowItem()
        {
            this.gameObject.SetActive(true);
            return UniTask.CompletedTask;
        }
        
    }
}