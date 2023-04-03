namespace TheOneStudio.UITemplate.UITemplate.FTUE
{
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
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

        [Inject]
        public void Init(ScreenManager screenManager, SignalBus signalBus, UITemplateFTUEBlueprint uiTemplateFtueBlueprint)
        {
            this.screenManager           = screenManager;
            this.uiTemplateFtueBlueprint = uiTemplateFtueBlueprint;
            this.signalBus               = signalBus;
        }

        #endregion

        private void Awake()
        {
            this.originPath = this.GetComponentInParent<Transform>();
            this.gameObject.SetActive(false);
        }

        public void MoveToOriginParent()
        {
            this.transform.SetParent(this.originPath, false);
            this.transform.localPosition = Vector3.zero;
            this.gameObject.SetActive(false);
        }

        public void MoveToCurrentRootUI(Transform parent)
        {
            this.transform.SetParent(parent, false);
            this.transform.SetAsLastSibling();
        }

        public void SetTutorialStatus(bool status, string triggerId)
        {
            this.gameObject.SetActive(status);
            this.PrepareTutorial(status, triggerId);
        }

        private void PrepareTutorial(bool status, string triggerId)
        {
            var currentActiveScreen = this.screenManager.CurrentActiveScreen.Value;
            var childTransform      = currentActiveScreen.CurrentTransform.GetComponentsInChildren<Transform>();
            var record              = this.uiTemplateFtueBlueprint[triggerId];
            var highLightPath       = record.HighLightPath;
            var buttonCanClick      = record.ButtonCanClick;
            this.btnCompleteStep.onClick.RemoveAllListeners();

            if (status)
            {
                foreach (var current in childTransform)
                {
                    if (!current.name.Equals(highLightPath)) continue;
                    current.gameObject.AddComponent<UITemplateFTUEControlElement>();
                    var btn = current.GetComponent<Button>();

                    this.btnCompleteStep.gameObject.SetActive(!buttonCanClick);

                    if (!buttonCanClick)
                    {
                        this.btnCompleteStep.onClick.AddListener(() => { this.signalBus.Fire(new FTUEButtonClickSignal(btn.name, triggerId)); });
                    }

                    if (btn != null && buttonCanClick)
                    {
                        this.disposables.Add(btn.OnPointerClickAsObservable().Subscribe(data => { this.signalBus.Fire(new FTUEButtonClickSignal(btn.name, triggerId)); }));
                    }

                    this.ConfigHandPosition(current.gameObject, record);
                }
            }
            else
            {
                this.disposables.Dispose();

                foreach (var current in childTransform)
                {
                    if (current.GetComponent<UITemplateFTUEControlElement>() != null)
                    {
                        Destroy(current.GetComponent<UITemplateFTUEControlElement>());
                    }
                }
            }
        }

        private void ConfigHandPosition(GameObject targetHighlight, UITemplateFTUERecord record)
        {
            this.hand.transform.SetParent(targetHighlight.transform);
            this.hand.transform.localPosition = Vector3.zero;
            this.hand.transform.localScale    = Vector3.one;
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