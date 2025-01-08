namespace TheOneStudio.UITemplate.UITemplate.Creative.Cheat
{
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using DG.DemiEditor;
    using GameFoundation.DI;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Signals;
    using Sirenix.OdinInspector;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Configs.GameEvents;
    using TheOneStudio.UITemplate.UITemplate.Creative.ChangeBGColor;
    using TheOneStudio.UITemplate.UITemplate.Creative.CheatLevel;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class TheOneCheatView : MonoBehaviour
    {
        [BoxGroup("Common")]   public GameObject            goContainer;
        [BoxGroup("Common")]   public Button                btnClose;
        [BoxGroup("UI")]       public UITemplateOnOffButton onOffUI;
        [BoxGroup("Currency")] public TMP_Dropdown          ddCurrency;
        [BoxGroup("Currency")] public TMP_InputField        inputCurrencyValue;
        [BoxGroup("Currency")] public Button                btnAddCurrency;
        [BoxGroup("Level")]    public TMP_InputField        inputLevel;
        [BoxGroup("Level")]    public Button                btnChangeLevel;

        [BoxGroup("GameplayBGColor")] public TMP_InputField txtHexCode;
        [BoxGroup("GameplayBGColor")] public Button         btnChangeColor;
        [BoxGroup("GameplayBGColor")] public List<Slider>   listSlider;
        [BoxGroup("GameplayBGColor")] public Image          backgroundTest;

        protected ICheatDetector cheatDetector;

        #region Inject

        protected UITemplateInventoryDataController inventoryDataController;
        protected GameFeaturesSetting               gameFeaturesSetting;
        protected UITemplateLevelDataController     levelDataController;
        protected SignalBus                         signalBus;
        protected UITemplateCurrencyBlueprint       currencyBlueprint;

        #endregion

        protected virtual void Awake()
        {
            DontDestroyOnLoad(this.gameObject);

            var container = this.GetCurrentContainer();
            this.inventoryDataController = container.Resolve<UITemplateInventoryDataController>();
            this.gameFeaturesSetting     = container.Resolve<GameFeaturesSetting>();
            this.levelDataController     = container.Resolve<UITemplateLevelDataController>();
            this.signalBus               = container.Resolve<SignalBus>();
            this.currencyBlueprint       = container.Resolve<UITemplateCurrencyBlueprint>();

            this.onOffUI.Button.onClick.AddListener(this.OnOnOffUIClick);
            this.btnAddCurrency.onClick.AddListener(this.OnAddCurrencyClick);
            this.btnClose.onClick.AddListener(this.OnCloseView);
            this.btnChangeLevel.onClick.AddListener(this.OnChangeLevelClick);
            
            this.btnChangeColor.onClick.AddListener(this.ChangeGameplayBGColor);
            this.listSlider.ForEach(slider => slider.onValueChanged.AddListener(this.OnSliderValueChanged));
            this.txtHexCode.onValueChanged.AddListener(this.ChangeHexcimal);

            this.cheatDetector = this.gameFeaturesSetting.cheatActiveBy switch
            {
                TheOneCheatActiveType.TripleTap        => new TripleTapCheatDetector(),
                TheOneCheatActiveType.DrawTripleCircle => new CircleDrawCheatDetector(),
                _                                      => null,
            };

            this.goContainer.SetActive(false);
        }

        protected void OnChangeLevelClick()
        {
            if (!int.TryParse(this.inputLevel.text, out var level)) return;

            this.levelDataController.SelectLevel(level);
            this.signalBus.Fire(new ChangeLevelCreativeSignal(level));
        }

        protected void OnCloseView()
        {
            this.goContainer.SetActive(false);
        }

        protected void OnAddCurrencyClick()
        {
            var ddCurrencyOption = this.ddCurrency.options[this.ddCurrency.value].text;
            this.inventoryDataController.AddCurrency(int.Parse(this.inputCurrencyValue.text), ddCurrencyOption).Forget();
        }

        protected void OnOnOffUIClick()
        {
            this.onOffUI.SetOnOff(!this.onOffUI.isOn);
            this.SetOnOffUI(this.onOffUI.isOn);
        }

        protected virtual void SetOnOffUI(bool isOn)
        {
            var rootUICanvas = FindObjectOfType<RootUICanvas>();
            if (rootUICanvas == null) return;
            var canvas = rootUICanvas.GetComponentInChildren<Canvas>();
            if (canvas == null) return;
            canvas.enabled = isOn;
        }

        protected void Update()
        {
            if (!this.cheatDetector.Check()) return;
            this.goContainer.SetActive(true);
            this.SetupDropdown();
        }

        protected void SetupDropdown()
        {
            this.ddCurrency.ClearOptions();
            this.ddCurrency.AddOptions(this.currencyBlueprint.Keys.ToList());
        }

        protected void OnSliderValueChanged(float value)
        {
            var color = new Color(this.listSlider[0].value, this.listSlider[1].value, this.listSlider[2].value, this.listSlider[3].value);
            this.backgroundTest.color = color;
        }

        protected void ChangeHexcimal(string text)
        {
            if (ColorUtility.TryParseHtmlString(text, out Color color))
            {
                if (this.backgroundTest != null)
                {
                    this.backgroundTest.color = color;
                }
                else
                {
                    Debug.LogError("Target image is not assigned.");
                }
            }
        }

        protected void ChangeGameplayBGColor()
            => this.signalBus.Fire(new ChangeBGColorSignal(this.txtHexCode.text, this.listSlider[0].value, this.listSlider[1].value, this.listSlider[2].value, this.listSlider[3].value));
    }
}