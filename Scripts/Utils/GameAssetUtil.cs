namespace TheOneStudio.UITemplate.UITemplate.Utils
{
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.AssetLibrary;
    using UnityEngine;
    using UnityEngine.Scripting;

    public class GameAssetUtil
    {
        #region inject

        private readonly IGameAssets gameAssets;

        #endregion

        [Preserve]
        public GameAssetUtil(IGameAssets gameAssets)
        {
            this.gameAssets = gameAssets;
        }

        public string GetItemIconName(string itemId)
        {
            return $"{itemId}Icon";
        }

        public async UniTask<Sprite> GetItemIconSprite(string itemId)
        {
            return await this.gameAssets.LoadAssetAsync<Sprite>(this.GetItemIconName(itemId));
        }
    }
}