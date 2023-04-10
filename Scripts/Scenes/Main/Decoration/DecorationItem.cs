namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main.Decoration
{
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using GameFoundation.Scripts.AssetLibrary;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main.Decoration.UI;
    using UnityEngine;
    using Zenject;

    public abstract class DecorationItem : MonoBehaviour, IDecorationItem
    {
        #region Cache

        private bool isDoPunchScale;

        #endregion

        #region Inject

        protected IGameAssets                       GameAssets;
        protected UITemplateInventoryDataController uiTemplateInventoryDataController;

        [Inject]
        protected void Init(IGameAssets gameAssets, UITemplateInventoryDataController uiTemplateInventoryDataController)
        {
            this.GameAssets                        = gameAssets;
            this.uiTemplateInventoryDataController = uiTemplateInventoryDataController;
        }

        #endregion

        #region Implement IDecorationItem

        public Vector3 PositionUI { get; private set; }

        public string Category { get; private set; }

        public virtual void Init(UITemplateDecorCategoryRecord record)
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

        public abstract UniTask ChangeItem(string addressItem);

        #endregion
    }
}