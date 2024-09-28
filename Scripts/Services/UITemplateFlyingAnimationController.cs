namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.Utilities.ObjectPool;
    using TheOneStudio.UITemplate.UITemplate.Extension;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using UnityEngine;
    using UnityEngine.Scripting;
    using Object = UnityEngine.Object;
    using Random = UnityEngine.Random;

    public class UITemplateFlyingAnimationController
    {
        private float FlyPunchTime              = 0.5f;
        private float DelayFlyTargetTimePerItem = 0.08f;
        private int   FlyPunchVibrator          = 2;
        private float FlyElasticPositionScale   = 0.5f;

        #region Inject

        private readonly IScreenManager screenManager;
        private readonly IGameAssets    gameAssets;
        private const    string         PrefabName = "UITemplateFlyingAnimationItem";

        [Preserve]
        public UITemplateFlyingAnimationController(IScreenManager screenManager, IGameAssets gameAssets)
        {
            this.screenManager = screenManager;
            this.gameAssets    = gameAssets;
        }

        #endregion

        public async UniTask PlayAnimation<T>(RectTransform startPointRect, int minAmount = 6, int maxAmount = 10, float timeAnim = 1f, RectTransform target = null, string prefabName = "", float flyPunchPositionFactor = 0.3f)
            where T : UITemplateFlyingAnimationView
        {
            var endPosition = target != null
                ? target.position
                : this.screenManager.RootUICanvas
                    .GetComponentsInChildren<T>()
                    .FirstOrDefault()?
                    .TargetFlyingAnimation
                    .position;
            if (endPosition == null || startPointRect == null) return;

            var totalCount  = Random.Range(minAmount, maxAmount);
            var finalPrefab = string.IsNullOrEmpty(prefabName) ? PrefabName : prefabName;
            var prefab      = await this.gameAssets.LoadAssetAsync<GameObject>(finalPrefab);
            var listItem    = new List<GameObject>();
            //create temp object to get random point in rect
            var box2D = this.CreateTempBoxCollider(startPointRect);

            for (var i = 0; i < totalCount; i++)
            {
                var startPoint = box2D.bounds.RandomPointInBounds();
                var item       = prefab.Spawn(this.screenManager.RootUICanvas.RootUIOverlayTransform.transform);
                item.transform.localEulerAngles = Vector3.zero;
                item.transform.localScale       = Vector3.one;
                item.transform.position         = startPoint;

                item.transform.DOPunchPosition(item.transform.position * flyPunchPositionFactor, this.FlyPunchTime, this.FlyPunchVibrator, this.FlyElasticPositionScale).SetUpdate(true);
                listItem.Add(item);
            }

            Object.Destroy(box2D.gameObject);

            await UniTask.Delay(TimeSpan.FromSeconds(this.FlyPunchTime / 2), DelayType.UnscaledDeltaTime);
            this.DoFlyingItems(listItem, this.DelayFlyTargetTimePerItem, endPosition.Value, timeAnim).Forget();

            await UniTask.Delay(TimeSpan.FromSeconds(this.DelayFlyTargetTimePerItem + timeAnim), DelayType.UnscaledDeltaTime);
        }

        private async UniTask DoFlyingItems(List<GameObject> listItem, float delayFlyingTime, Vector3 endUiPos, float timeAnim)
        {
            foreach (var item in listItem)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(delayFlyingTime), DelayType.UnscaledDeltaTime);

                item.transform.DOMove(endUiPos, timeAnim)
                    .SetEase(Ease.InBack)
                    .SetUpdate(true)
                    .OnComplete(item.Recycle);
            }
        }

        private BoxCollider2D CreateTempBoxCollider(RectTransform startPointRect)
        {
            var tempObjGetBoxCollider = new GameObject().AddComponent<BoxCollider2D>();
            tempObjGetBoxCollider.transform.SetParent(this.screenManager.RootUICanvas.RootUIOverlayTransform.transform);
            tempObjGetBoxCollider.transform.localScale = Vector3.one;
            tempObjGetBoxCollider.gameObject.AddComponent<RectTransform>();

            tempObjGetBoxCollider.transform.position                      = startPointRect.position;
            tempObjGetBoxCollider.GetComponent<RectTransform>().sizeDelta = startPointRect.sizeDelta;
            tempObjGetBoxCollider.size                                    = new Vector2(startPointRect.rect.width, startPointRect.rect.height);

            return tempObjGetBoxCollider;
        }
    }
}