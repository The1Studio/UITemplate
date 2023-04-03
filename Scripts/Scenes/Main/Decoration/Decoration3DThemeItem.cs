namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main.Decoration
{
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.Utilities.ObjectPool;
    using UnityEngine;

    public class Decoration3DThemeItem : DecorationItem
    {
        #region Cache

        private GameObject currentChild;

        #endregion

        public override async UniTask ChangeItem(string addressItem)
        {
            await UniTask.WaitUntil(() => this.GameAssets != null);
            if (this.currentChild != null) this.currentChild.Recycle();
            this.currentChild = null;
            this.currentChild = await ObjectPoolManager.Instance.Spawn(addressItem, this.transform);
        }
    }
}