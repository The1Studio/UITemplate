namespace TheOneStudio.UITemplate.UITemplate.FTUE
{
    using System.Linq;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Signals;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.FTUE.Signal;
    using UniRx;
    using UniRx.Triggers;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateFTUEController : MonoBehaviour
    {
        public Button        btnCompleteStep;
        public GameObject    hand;
        public RectTransform rotateHand, iconHand;

        private Transform originPath;

        private CompositeDisposable disposables = new();

        #region Zenject

        private ScreenManager           screenManager;
        private UITemplateFTUEBlueprint uiTemplateFtueBlueprint;
        private SignalBus               signalBus;
        private Transform               highLightTransform;
        private string                  currentActiveStepId;

        [Inject]
        public void Init(ScreenManager screenManager, SignalBus signalBus, UITemplateFTUEBlueprint uiTemplateFtueBlueprint)
        {
            this.screenManager           = screenManager;
            this.uiTemplateFtueBlueprint = uiTemplateFtueBlueprint;
            this.signalBus               = signalBus;
            this.signalBus.Subscribe<StartLoadingNewSceneSignal>(this.OnStartLoadingNewScene);
            this.signalBus.Subscribe<FinishLoadingNewSceneSignal>(this.OnFinishLoadingNewScene);
        }

        private void OnFinishLoadingNewScene(FinishLoadingNewSceneSignal obj) { this.MoveToCurrentRootUI(this.screenManager.CurrentOverlayRoot); }

        private void OnStartLoadingNewScene(StartLoadingNewSceneSignal obj) { this.MoveToOriginParent(); }

        #endregion

        private void Awake()
        {
            var projectContextTrans = FindObjectOfType<ProjectContext>();
            this.originPath = projectContextTrans.transform;
            this.gameObject.SetActive(false);
        }

        private void MoveToOriginParent()
        {
            this.transform.SetParent(this.originPath, false);
            this.transform.localPosition = Vector3.zero;
            this.gameObject.SetActive(false);
        }

        private void MoveToCurrentRootUI(Transform parent)
        {
            this.transform.SetParent(parent, false);
            this.transform.SetAsLastSibling();
        }

        public void DisableTutorial(string stepId)
        {
            if (!stepId.Equals(this.currentActiveStepId)) return;
            this.gameObject.SetActive(false);
            this.disposables.Dispose();
            this.hand.transform.SetParent(this.transform, false);

            if (this.highLightTransform != null &&this.highLightTransform.TryGetComponent<UITemplateFTUEControlElement>(out var uITemplateFtueControlElement))
            {
                Destroy(uITemplateFtueControlElement);
                this.highLightTransform = null;
            }
        }

        public void DoActiveFTUE(string stepId)
        {
            this.currentActiveStepId = stepId;
            this.gameObject.SetActive(true);
            var currentActiveScreen = this.screenManager.CurrentActiveScreen.Value;
            var childTransforms     = currentActiveScreen.CurrentTransform.GetComponentsInChildren<Transform>();
            var record              = this.uiTemplateFtueBlueprint[stepId];
            this.highLightTransform = childTransforms.First(childTransform => childTransform.name.Equals(record.HighLightPath));
            this.btnCompleteStep.onClick.RemoveAllListeners();

            this.disposables = new CompositeDisposable();
            this.highLightTransform.gameObject.AddComponent<UITemplateFTUEControlElement>();
            var btn = this.highLightTransform.GetComponent<Button>();

            this.btnCompleteStep.gameObject.SetActive(!record.ButtonCanClick);

            if (!record.ButtonCanClick)
            {
                this.btnCompleteStep.onClick.AddListener(() => { this.signalBus.Fire(new FTUEButtonClickSignal(btn.name, stepId)); });
            }

            if (btn != null && record.ButtonCanClick)
            {
                this.disposables.Add(btn.OnPointerClickAsObservable().Subscribe(data => { this.signalBus.Fire(new FTUEButtonClickSignal(btn.name, stepId)); }));
            }

            this.ConfigHandPosition(this.highLightTransform.gameObject, record);
        }

        private void ConfigHandPosition(GameObject targetHighlight, UITemplateFTUERecord record)
        {
            this.hand.transform.SetParent(targetHighlight.transform);
            this.hand.transform.localPosition                 = Vector3.zero;
            this.hand.transform.localScale                    = Vector3.one;
            this.hand.GetComponent<Canvas>().sortingLayerName = "UI";
            var rectTransform = this.hand.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta        = record.HandSizeDelta;
            this.iconHand.anchoredPosition = Vector2.one + new Vector2(0, record.Radius);

            var angle = record.HandAnchor switch
            {
                "Top" => -180,
                "TopRight" => -225,
                "TopLeft" => -135,
                "Left" => -90,
                "Right" => -270,
                "BottomLeft" => -45,
                "Bottom" => 0,
                "BottomRight" => -315,
                _ => 0f
            };

            this.rotateHand.localEulerAngles = new Vector3(0, 0, angle);
            this.iconHand.localEulerAngles   = record.HandRotation;
        }
    }
}