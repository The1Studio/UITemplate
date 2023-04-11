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

        private   bool                          isDoPunchScale;
        protected UITemplateDecorCategoryRecord record;

        #endregion

        #region Inject

        protected IGameAssets                       GameAssets;
        protected UITemplateInventoryDataController uiTemplateInventoryDataController;
        protected SignalBus                         signalBus;

        [Inject]
        protected void Init(IGameAssets gameAssets, UITemplateInventoryDataController uiTemplateInventoryDataController, SignalBus signalBus)
        {
            this.GameAssets                        = gameAssets;
            this.uiTemplateInventoryDataController = uiTemplateInventoryDataController;
            this.signalBus                         = signalBus;
        }

        #endregion

        #region Implement IDecorationItem

        public Vector3 PositionUI { get; private set; }

        public string Category { get; private set; }

        public virtual void Init(UITemplateDecorCategoryRecord record)
        {
            this.record                = record;
            this.transform.position    = record.PositionOnScene;
            this.transform.eulerAngles = record.RotationOnScene;
            this.PositionUI            = record.ButtonPosition;
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

        public virtual void ShowItem()
        {
        }

        public virtual void HideItem()
        {
        }

        #endregion
    }
}