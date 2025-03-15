namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using GameFoundation.DI;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.Utilities;
    using GameFoundation.Scripts.Utilities.ObjectPool;
    using GameFoundation.Signals;
    using global::UITemplate.Scripts.Signals;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Extension;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using UnityEngine;
    using UnityEngine.Scripting;
    using Object = UnityEngine.Object;
    using Random = UnityEngine.Random;

    public class UITemplateFlyingAnimationController : IInitializable
    {
        private float FlyPunchTime              = 0.5f;
        private float DelayFlyTargetTimePerItem = 0.08f;
        private int   FlyPunchVibrator          = 2;
        private float FlyElasticPositionScale   = 0.5f;

        #region Inject

        private readonly IScreenManager              screenManager;
        private readonly IGameAssets                 gameAssets;
        private readonly SignalBus                   signalBus;
        private readonly UITemplateCurrencyBlueprint uiTemplateCurrencyBlueprint;
        private readonly IAudioService               audioService;
        private const    string                      PrefabName = "UITemplateFlyingAnimationItem";

        [Preserve]
        public UITemplateFlyingAnimationController(IScreenManager screenManager, IGameAssets gameAssets, SignalBus signalBus, UITemplateCurrencyBlueprint uiTemplateCurrencyBlueprint, IAudioService audioService)
        {
            this.screenManager               = screenManager;
            this.gameAssets                  = gameAssets;
            this.signalBus                   = signalBus;
            this.uiTemplateCurrencyBlueprint = uiTemplateCurrencyBlueprint;
            this.audioService                = audioService;
        }

        #endregion
        
        
        public void Initialize()
        {
            this.signalBus.Subscribe<PlayCurrencyAnimationSignal>(obj => this.OnPlayCurrencyAnimation(obj).Forget());
        }
        private async UniTaskVoid OnPlayCurrencyAnimation(PlayCurrencyAnimationSignal obj)
        {
            var id           = obj.currecyId;
            var flyingObject = this.uiTemplateCurrencyBlueprint.GetDataById(id).FlyingObject;
            var currencyView = this.screenManager.RootUICanvas.GetComponentsInChildren<UITemplateCurrencyView>().FirstOrDefault(viewTarget => viewTarget.CurrencyKey.Equals(id));
            if (currencyView != null)
            {
                if (!string.IsNullOrEmpty(obj.claimSoundKey)) this.audioService.PlaySound(obj.claimSoundKey);
                await this.PlayAnimation<UITemplateFlyingAnimationView>(
                    startPointRect: obj.startAnimationRect,
                    minAmount: obj.minAnimAmount,
                    maxAmount: obj.maxAnimAmount,
                    timeAnim: obj.timeAnimAnim,
                    target: currencyView.CurrencyIcon.transform as RectTransform,
                    prefabName: flyingObject,
                    flyPunchPositionFactor: obj.flyPunchPositionAnimFactor,
                    onCompleteEachItem: () =>
                    {
                        obj.onCompleteEachItem?.Invoke();
                        if (!string.IsNullOrEmpty(obj.flyCompleteSoundKey)) this.audioService.PlaySound(obj.flyCompleteSoundKey);
                    });

                this.signalBus.Fire(new OnFinishCurrencyAnimationSignal(id, obj.amount, obj.currencyWithCap));
            }
        }

        public async UniTask PlayAnimation<T>(RectTransform startPointRect, int minAmount = 6, int maxAmount = 10, float timeAnim = 1f, RectTransform target = null, string prefabName = "", float flyPunchPositionFactor = 0.3f, Action onCompleteEachItem = null)
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

            this.DoFlyingItems(listItem, this.DelayFlyTargetTimePerItem, endPosition.Value, timeAnim, onCompleteEachItem).Forget();

            await UniTask.Delay(TimeSpan.FromSeconds(this.DelayFlyTargetTimePerItem + timeAnim), DelayType.UnscaledDeltaTime);
        }

        private async UniTask DoFlyingItems(List<GameObject> listItem, float delayFlyingTime, Vector3 endUiPos, float timeAnim, Action onCompleteEachItem = null)
        {
            foreach (var item in listItem)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(delayFlyingTime), DelayType.UnscaledDeltaTime);

                item.transform.DOMove(endUiPos, timeAnim)
                    .SetEase(Ease.InBack)
                    .SetUpdate(true)
                    .OnComplete(() =>
                    {
                        item.Recycle();
                        onCompleteEachItem?.Invoke();
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
            tempObjGetBoxCollider.size                                    = new(startPointRect.rect.width, startPointRect.rect.height);

            return tempObjGetBoxCollider;
        }
    }
}