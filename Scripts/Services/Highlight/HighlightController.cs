﻿namespace TheOneStudio.UITemplate.UITemplate.Scripts.Services.Highlight
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Com.ForbiddenByte.OSA.Core;
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Signals;
    using GameFoundation.Signals;
    using R3;
    using R3.Triggers;
    using TheOne.Extensions;
    using UnityEngine;
    using UnityEngine.UI;
    using Object = UnityEngine.Object;

    public class HighlightController : MonoBehaviour
    {
        // if the first element in path is "RootUICanvas", the highlight object will be searched in the RootUICanvas, otherwise it will be searched in the current screen
        // Example : "RootUICanvas|HighlightObject" or "ScreenName|HighlightObject" or HighlightObject
        private const string ROOT_UI_LOCATION = "RootUICanvas";
        private const string ALL              = "ALL";

        public Button        btnCompleteStep;
        public HighlightHand highlightHand;

        private CompositeDisposable disposables = new();

        #region Inject

        private IScreenManager screenManager;
        private SignalBus      signalBus;

        public void Construct(IScreenManager screenManager, SignalBus signalBus)
        {
            this.screenManager = screenManager;
            this.signalBus     = signalBus;
            this.signalBus.Subscribe<StartLoadingNewSceneSignal>(this.OnStartLoadingNewScene);
            this.signalBus.Subscribe<FinishLoadingNewSceneSignal>(this.OnFinishLoadingNewScene);
        }

        private void OnFinishLoadingNewScene(FinishLoadingNewSceneSignal obj)
        {
            this.MoveToCurrentRootUI(this.screenManager.CurrentOverlayRoot);
        }

        private void OnStartLoadingNewScene(StartLoadingNewSceneSignal obj)
        {
            this.MoveToOriginParent();
        }

        private void OnButtonClick()
        {
            this.TurnOffHighlight();
        }

        private void MoveToOriginParent()
        {
            this.transform.SetParent(null, false);
            this.gameObject.SetActive(false);
        }

        private void MoveToCurrentRootUI(Transform parent)
        {
            this.transform.SetParent(parent, false);
            this.transform.SetAsLastSibling();
        }

        private void Awake()
        {
            DontDestroyOnLoad(this);
            this.gameObject.SetActive(false);
        }

        #endregion

        #region Highlight

        private          List<Transform>     highlightObjects = new();
        private readonly IReadOnlyList<Type> screenPresenters = typeof(IScreenPresenter).GetDerivedTypes().ToList();

        public async UniTask SetHighlight(string highlightPath, bool canClickOutside = false, Action onButtonDown = null)
        {
            this.ClearHighlightObject();
            await this.GetHighlightObject(highlightPath);
            this.SetHighlight(canClickOutside, onButtonDown);
        }

        public UniTask SetHighlight(List<Transform> highlightObjects, bool canClickOutside = false, Action onButtonDown = null)
        {
            this.ClearHighlightObject();
            this.highlightObjects = highlightObjects;
            this.SetHighlight(canClickOutside, onButtonDown);
            return UniTask.CompletedTask;
        }

        private void SetHighlight(bool canClickOutside, Action onButtonDown = null)
        {
            if (this.highlightObjects.Count == 0)
            {
                this.TurnOffHighlight();
                return;
            }

            this.gameObject.SetActive(true);
            this.transform.SetAsLastSibling();
            this.btnCompleteStep.onClick.RemoveAllListeners();

            this.disposables = new();
            this.HandleButtonClick(canClickOutside, onButtonDown);

            var containHighlightObject = this.highlightObjects[0];
            foreach (var obj in this.highlightObjects)
                if (containHighlightObject.IsChildOf(obj))
                    containHighlightObject = obj;
            var rectMask = containHighlightObject.GetComponentInParent<Mask>();
            var osa      = containHighlightObject.GetComponentInParent<IOSA>();
            if (rectMask is null || osa is null) return;
            var rectTf = rectMask.GetComponent<RectTransform>();
            this.ConfigAdapter(containHighlightObject.gameObject, rectTf, osa);
        }

        public GameObject GetHighlightedObject()
        {
            return this.highlightObjects is { Count: > 0 } ? this.highlightObjects[0].gameObject : null;
        }

        public void TurnOffHighlight()
        {
            this.gameObject.SetActive(false);
            this.disposables.Dispose();

            this.ClearHighlightObject();

            if (this.activeHand.Count == 0) return;
            for (var i = this.activeHand.Count - 1; i >= 0; i--) this.DespawnHand(this.activeHand[i]);
            this.activeHand.Clear();
        }

        private async UniTask GetHighlightObject(string highlightPath)
        {
            List<HighlightElement> highlightElements;
            var                    objNames = highlightPath.Split('|').ToList();
            if (objNames.Count == 0) return;
            switch (objNames[0])
            {
                case ROOT_UI_LOCATION: highlightElements = this.screenManager.RootUICanvas.GetComponentsInChildren<HighlightElement>().ToList(); break;
                case ALL:              highlightElements = Object.FindObjectsByType<HighlightElement>(FindObjectsSortMode.None).ToList(); break;
                default:
                    var screenType = this.screenPresenters.FirstOrDefault(screen => screen.Name == objNames[0]);
                    if (screenType != null && await this.screenManager.GetScreen(screenType) is { } screenPresenter)
                    {
                        highlightElements = screenPresenter.CurrentTransform.GetComponentsInChildren<HighlightElement>().ToList();
                    }
                    else
                    {
                        objNames.Add(objNames[0]);
                        highlightElements = this.screenManager.CurrentActiveScreen.Value.CurrentTransform.GetComponentsInChildren<HighlightElement>().ToList();
                    }
                    break;
            }

            for (var i = 1; i < objNames.Count; i++)
            {
                var tf = highlightElements.Where(x => x.isActiveAndEnabled).FirstOrDefault(obj => obj.name == objNames[i]);
                if (tf) this.highlightObjects.Add(tf.transform);
            }
        }

        private void HandleButtonClick(bool canClickOutside, Action onButtonDown)
        {
            foreach (var highlightObject in this.highlightObjects)
            {
                if (!this.highlightObjects.Any(tf => highlightObject.IsChildOf(tf) && highlightObject != tf)) highlightObject.gameObject.GetComponent<HighlightElement>().Setup();

                var buttons = highlightObject.GetComponentsInChildren<Button>();
                foreach (var button in buttons)
                {
                    this.disposables.Add(button.OnPointerClickAsObservable().Subscribe(data =>
                    {
                        this.OnButtonClick();
                        onButtonDown?.Invoke();
                    }));
                }
            }
            this.btnCompleteStep.gameObject.SetActive(canClickOutside);
            if (canClickOutside)
                this.btnCompleteStep.onClick.AddListener(() =>
                {
                    this.OnButtonClick();
                    onButtonDown?.Invoke();
                });
        }

        private void ClearHighlightObject()
        {
            if (this.highlightObjects.Count > 0)
            {
                this.highlightObjects.ForEach(obj =>
                {
                    try
                    {
                        obj.GetComponent<HighlightElement>().Despawn();
                    }
                    catch
                    {
                        // ignore
                    }
                });
                this.highlightObjects.Clear();
            }
        }

        #endregion

        #region HandleHand

        private Queue<HighlightHand> handPools  = new();
        private List<HighlightHand>  activeHand = new();

        private HighlightHand SpawnHand(Vector2 handSize, float radius, string anchor, Vector3 rotation)
        {
            if (this.handPools.Count == 0)
            {
                var pooledHand = Instantiate(this.highlightHand, this.transform);
                pooledHand.gameObject.SetActive(false);
                this.handPools.Enqueue(pooledHand);
            }
            var hand = this.handPools.Dequeue();
            this.activeHand.Add(hand);
            hand.gameObject.SetActive(true);
            hand.transform.localPosition                 = Vector3.zero;
            hand.transform.localScale                    = Vector3.one;
            hand.GetComponent<Canvas>().sortingLayerName = "UI";
            var rectTransform = hand.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta        = handSize;
            hand.iconHand.anchoredPosition = new(0, radius);
            hand.iconHand.localEulerAngles = rotation;
            var angle = anchor switch
            {
                "Top"         => -180,
                "TopRight"    => -225,
                "TopLeft"     => -135,
                "Left"        => -90,
                "Right"       => -270,
                "BottomLeft"  => -45,
                "Bottom"      => 0,
                "BottomRight" => -315,
                _             => 0f,
            };
            hand.rotateHand.localEulerAngles = new(0, 0, angle);
            return hand;
        }

        private void DespawnHand(HighlightHand hand)
        {
            this.activeHand.Remove(hand);
            hand.gameObject.SetActive(false);
            this.handPools.Enqueue(hand);
        }

        public void ConfigHand(TypeConfigHand type, Vector2 handSize, float radius, string anchor, Vector3 rotation, params object[] param)
        {
            switch (type)
            {
                case TypeConfigHand.AllAppear:               this.HandleAllAppear(handSize, radius, anchor, rotation); break;
                case TypeConfigHand.AppearOneByOneWithDelay: this.HandleAppearOneByOneWithDelay(handSize, radius, anchor, rotation, (float)param[0]).Forget(); break;
                case TypeConfigHand.MoveOneByOneWithDelay:   this.HandleMoveOneByOneWithDelay(handSize, radius, anchor, rotation, (float)param[0], (float)param[1]).Forget(); break;
            }
        }

        private void HandleAllAppear(Vector2 handSize, float radius, string anchor, Vector3 rotation)
        {
            foreach (var highlightObject in this.highlightObjects)
            {
                var hand = this.SpawnHand(handSize, radius, anchor, rotation);
                hand.transform.position = highlightObject.position;
                highlightObject.GetComponent<HighlightElement>().OnPositionChange = () =>
                {
                    hand.transform.position = highlightObject.position;
                };
            }
        }

        private async UniTask HandleAppearOneByOneWithDelay(Vector2 handSize, float radius, string anchor, Vector3 rotation, float delay)
        {
            foreach (var highlightObject in this.highlightObjects)
            {
                var hand = this.SpawnHand(handSize, radius, anchor, rotation);
                hand.transform.position = highlightObject.position;
                await UniTask.Delay((int)(delay * 1000));
                this.DespawnHand(hand);
            }
        }

        private async UniTask HandleMoveOneByOneWithDelay(Vector2 handSize, float radius, string anchor, Vector3 rotation, float delay, float timeMove)
        {
            var hand = this.SpawnHand(handSize, radius, anchor, rotation);
            hand.transform.position = this.highlightObjects[0].position;
            for (var i = 1; i < this.highlightObjects.Count; i++)
            {
                await UniTask.Delay((int)(delay * 1000));
                var tween = hand.transform.DOMove(this.highlightObjects[i].position, timeMove).SetEase(Ease.Linear);
                await UniTask.WaitUntil(() => tween.IsComplete());
            }
        }

        #endregion

        #region ConfigAdapter

        private void ConfigAdapter(GameObject highlightGameObject, RectTransform rectTf, IOSA osa)
        {
            var target = highlightGameObject.GetComponent<RectTransform>();
            for (var i = 0; i < 1000; i++)
            {
                if (this.IsRectTransformFullyInside(rectTf, target)) break;
                osa.SetVirtualAbstractNormalizedScrollPosition(osa.GetNormalizedPosition() - 0.01, true, out _, true);
            }
        }

        private bool IsRectTransformFullyInside(RectTransform container, RectTransform target)
        {
            var containerWorldRect = this.GetWorldRect(container);
            var targetWorldRect    = this.GetWorldRect(target);

            return containerWorldRect.Contains(targetWorldRect.min) && containerWorldRect.Contains(targetWorldRect.max);
        }

        private Rect GetWorldRect(RectTransform rectTransform)
        {
            var corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);

            var bottomLeft = corners[0];
            var topRight   = corners[2];

            return new(bottomLeft, topRight - bottomLeft);
        }

        #endregion
    }

    public enum TypeConfigHand
    {
        AllAppear,
        AppearOneByOneWithDelay,
        MoveOneByOneWithDelay,
    }
}