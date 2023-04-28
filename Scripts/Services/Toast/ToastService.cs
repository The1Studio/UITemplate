namespace TheOneStudio.UITemplate.UITemplate.Services.Toast
{
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.Utilities.ObjectPool;
    using UnityEngine;

    public enum ToastPosition
    {
        Bottom,
        Top,
        Center
    }

    public class ToastService
    {
        private readonly IGameAssets gameAssets;

        private const string          ToastPrefabAddressableName = "ToastCanvas";
        private       ToastController toastController;

        public ToastService(IGameAssets gameAssets) { this.gameAssets = gameAssets; }

        public async void ShowToast(string message, float offsetX = 0, float offsetY = 0, ToastPosition position = ToastPosition.Bottom)
        {
            if (this.toastController == null)
            {
                var toastPrefab = await this.gameAssets.LoadAssetAsync<GameObject>(ToastPrefabAddressableName);
                var toastObj    = toastPrefab.Spawn();
                this.toastController = toastObj.GetComponent<ToastController>();
            }

            this.toastController.SetContent(message, new Vector2(offsetX, offsetY), position);
        }
    }
}