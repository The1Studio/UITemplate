namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.Utilities.ObjectPool;
    using GameFoundation.Signals;
    using UnityEngine;
    using UnityEngine.Scripting;

    public class UITemplateVFXSpawnService
    {
        public List<string> ListEncourageVFX = new() { "amazing", "perfect", "toohot", "youregood" };
        public List<string> ListComboVFX     = new() { "x2", "x3", "x4" };

        private readonly IGameAssets gameAssets;

        [Preserve]
        public UITemplateVFXSpawnService(SignalBus signalBus, IGameAssets gameAssets) { this.gameAssets = gameAssets; }

        public async void SpawnVFX(Transform target, List<string> listVFXKey, bool randomRotate = true, bool isFloat = true)
        {
            var randomIndex = Random.Range(0, listVFXKey.Count - 1);
            var vfxKey      = listVFXKey[randomIndex];
            var vfxPrefab   = await this.gameAssets.LoadAssetAsync<GameObject>(vfxKey);
            // spawn vfx follow target's position
            var position = target.position;
            //spawn random position base on target's position
            position.x += Random.Range(-1f, 1f);
            position.y += Random.Range(0f, 2f);
            position.z =  -1;
            // random vfx rotation
            var rotation = randomRotate ? Quaternion.Euler(0, 0, Random.Range(-50, 50)) : Quaternion.identity;
            // spawn vfx
            var vfxObj = vfxPrefab.Spawn(position, rotation);
            if (isFloat) vfxObj.transform.DOMoveY(position.y + 1f, 1f);
            await UniTask.Delay(2000);
            vfxObj.Recycle();
        }

        public void SpawnEncourageVFX(Transform target) { this.SpawnVFX(target, this.ListEncourageVFX); }

        public void SpawnComboVFX(Transform target) { this.SpawnVFX(target, this.ListComboVFX); }
    }
}