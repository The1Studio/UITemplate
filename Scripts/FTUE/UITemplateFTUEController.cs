namespace TheOneStudio.UITemplate.UITemplate.FTUE
{
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Signals;
    using GameFoundation.Scripts.Utilities.Extension;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.FTUE.Signal;
    using UniRx;
    using UniRx.Triggers;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateFTUEController : MonoBehaviour
    {
        private const string ROOT_UI_LOCATION = "RootUICanvas";

        public Button        btnCompleteStep;
        public GameObject    hand;
        public RectTransform rotateHand, iconHand;

        private Transform originPath;

        private CompositeDisposable disposables = new();

        #region Zenject

        private ScreenManager           screenManager;
        private UITemplateFTUEBlueprint uiTemplateFtueBlueprint;
        private SignalBus               signalBus;
        private Transform               highLightButtonTransform;
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

        public bool ThereIsFTUEActive() => !string.IsNullOrEmpty(this.currentActiveStepId);

        public void DoDeactiveFTUE(string stepId)
        {
            if (!stepId.Equals(this.currentActiveStepId)) return;
            this.currentActiveStepId = null;

            var record = this.uiTemplateFtueBlueprint.GetDataById(stepId);

            if (string.IsNullOrEmpty(record.HighLightPath)) return;

            this.gameObject.SetActive(false);
            this.disposables.Dispose();
            this.hand.transform.SetParent(this.transform, false);

            if (this.highLightButtonTransform != null && this.highLightButtonTransform.TryGetComponent<UITemplateFTUEControlElement>(out var uITemplateFtueControlElement))
            {
                Destroy(uITemplateFtueControlElement);
                this.highLightButtonTransform = null;
            }
        }

        public void DoActiveFTUE(string stepId, HashSet<GameObject> disableObjectSet)
        {
            this.currentActiveStepId = stepId;
            foreach (var disableObject in disableObjectSet)
            {
                disableObject.SetActive(true);
            }

            this.SetHighlightButton(stepId, this.uiTemplateFtueBlueprint.GetDataById(stepId)).Forget();
        }

        //Some time the button need time to create
        private async UniTaskVoid SetHighlightButton(string stepId, UITemplateFTUERecord record)
        {
            if (string.IsNullOrEmpty(record.HighLightPath)) return;

            this.gameObject.SetActive(true);
            if (this.highLightButtonTransform != null)
            {
                Destroy(this.highLightButtonTransform.GetComponent<UITemplateFTUEControlElement>());
                this.highLightButtonTransform = null;
            }

            while (this.highLightButtonTransform == null)
            {
                List<Button> buttons = new();
                if (record.HighLightPath.Split("|").Length != 1)
                {
                    var screenLocation = record.HighLightPath.Split("|")[0];
                    switch (screenLocation)
                    {
                        case ROOT_UI_LOCATION:
                            buttons = this.screenManager.RootUICanvas.transform.GetComponentsInChildren<Button>().ToList();

                            break;
                        default:
                            var presenterType                  = ReflectionUtils.GetAllDerivedTypes<IScreenPresenter>().FirstOrDefault(presenter => presenter.Name == screenLocation);
                            if (presenterType != null) buttons = (this.GetCurrentContainer().Resolve(presenterType) as IScreenPresenter)?.CurrentTransform.GetComponentsInChildren<Button>().ToList();

                            break;
                    }
                    this.highLightButtonTransform = buttons?.FirstOrDefault(button => button.gameObject.name.Equals(record.HighLightPath.Split("|")[1]))?.transform;
                }
                else
                {
                    buttons                       = this.screenManager.CurrentActiveScreen.Value.CurrentTransform.GetComponentsInChildren<Button>().ToList();
                    this.highLightButtonTransform = buttons.FirstOrDefault(button => button.gameObject.name.Equals(record.HighLightPath))?.transform;
                }
                if (this.highLightButtonTransform == null)
                {
                    await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
                }
            }

            this.btnCompleteStep.onClick.RemoveAllListeners();

            this.disposables = new();
            this.highLightButtonTransform.gameObject.AddComponent<UITemplateFTUEControlElement>();
            var highlightButton = this.highLightButtonTransform.GetComponent<Button>();

            this.btnCompleteStep.gameObject.SetActive(!record.ButtonCanClick);

            if (!record.ButtonCanClick)
            {
                this.btnCompleteStep.onClick.AddListener(() =>
                {
                    this.signalBus.Fire(new FTUEButtonClickSignal(highlightButton.name, stepId));
                });
            }

            if (highlightButton != null && record.ButtonCanClick)
            {
                this.disposables.Add(highlightButton.OnPointerClickAsObservable().Subscribe(data =>
                {
                    this.signalBus.Fire(new FTUEButtonClickSignal(highlightButton.name, stepId));
                }));
            }

            this.ConfigHandPosition(this.highLightButtonTransform.gameObject, record);
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
                "Top"         => -180,
                "TopRight"    => -225,
                "TopLeft"     => -135,
                "Left"        => -90,
                "Right"       => -270,
                "BottomLeft"  => -45,
                "Bottom"      => 0,
                "BottomRight" => -315,
                _             => 0f
            };

            this.rotateHand.localEulerAngles = new Vector3(0, 0, angle);
            this.iconHand.localEulerAngles   = record.HandRotation;
        }
    }
}