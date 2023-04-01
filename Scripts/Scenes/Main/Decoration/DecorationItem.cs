namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main.Decoration
{
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using GameFoundation.Scripts.AssetLibrary;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main.Decoration.UI;
    using UnityEngine;
    using Zenject;

    public class DecorationItem : MonoBehaviour, IDecorationItem
    {
        #region Cache

        private SpriteRenderer spriteRenderer;
        private bool           isDoPunchScale;

        #endregion

        #region Inject

        private IGameAssets gameAssets;

        [Inject]
        private void Init(IGameAssets gameAssets) { this.gameAssets = gameAssets; }

        #endregion

        #region Implement IDecorationItem

        public Vector3 PositionUI { get; protected set; }

        public string Category { get; protected set; }

        public void Init(UITemplateDecorCategoryRecord record)
        {
            this.transform.position    = record.PositionOnScene;
            this.transform.eulerAngles = record.RotationOnScene;
            this.PositionUI            = this.transform.position + record.OffsetPositionOnUI;
            this.Category              = record.Id;
        }

        public void ScaleItem()
        {
            if (this.isDoPunchScale) return;
            const float addPunchScaleMultiple = 0.1f;
            const float scaleDuration         = 1f;
            this.transform.DOPunchScale(this.transform.localScale * addPunchScaleMultiple, scaleDuration, 2)
                .SetEase(Ease.Linear)
                .OnUpdate(() => this.isDoPunchScale   = true)
                .OnComplete(() => this.isDoPunchScale = false);
        }

        public async UniTask ChangeItem(string addressItem)
        {
            await UniTask.WaitUntil(() => this.gameAssets != null);
            if (this.spriteRenderer == null) this.spriteRenderer = this.gameObject.AddComponent<SpriteRenderer>();
            this.spriteRenderer.sprite = await this.gameAssets.LoadAssetAsync<Sprite>(addressItem);
        }

        #endregion
    }
}