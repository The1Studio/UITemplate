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
    using Object = UnityEngine.Object;
    using Random = UnityEngine.Random;

    public class UITemplateFlyingAnimationCurrency
    {
        private readonly ScreenManager screenManager;
        private readonly IGameAssets   gameAssets;
        private const    string        PrefabName = "UITemplateFlyingAnimationItem";

        public UITemplateFlyingAnimationCurrency(ScreenManager screenManager, IGameAssets gameAssets)
        {
            this.screenManager = screenManager;
            this.gameAssets    = gameAssets;
        }

        public async UniTask PlayAnimation(RectTransform startPointRect, int minAmount = 6, int maxAmount = 10, float timeAnim = 1f, RectTransform target = null, string prefabName = "")
        {
            var currencyView = this.screenManager.RootUICanvas.RootUIShowTransform.GetComponentsInChildren<UITemplateCurrencyView>().First();
            var endUiPos     = Vector3.zero;

            if (currencyView != null)
            {
                endUiPos = currencyView.CurrencyIcon.transform.position;
            }

            if (target != null)
            {
                endUiPos = target.position;
            }

            if (currencyView == null && target == null)
            {
                return;
            }

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
                item.transform.localScale = Vector3.one;
                item.transform.position   = startPoint;

                item.transform.DOPunchPosition(item.transform.position * 0.3f, 0.5f, 2, 0.5f);
                listItem.Add(item);
            }

            Object.Destroy(box2D.gameObject);

            await UniTask.Delay(TimeSpan.FromSeconds(1f));
            var flyingTime = 0.08f;
            _ = this.DoFlyingCoins(listItem, flyingTime, endUiPos, timeAnim);

            await UniTask.Delay(TimeSpan.FromSeconds(flyingTime + timeAnim));
        }

        private async UniTask DoFlyingCoins(List<GameObject> listItem, float flyingTime, Vector3 endUiPos, float timeAnim)
        {
            foreach (var item in listItem)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(flyingTime));

                item.transform.DOMove(endUiPos, timeAnim).SetEase(Ease.InBack).OnComplete(() =>
                                                                                          {
                                                                                              item.Recycle();
                                                                                          });
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