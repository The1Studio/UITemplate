namespace TheOneStudio.UITemplate.UITemplate.Creative.CheatLevel
{
    using GameFoundation.DI;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class CreativeSelectLevelView : MonoBehaviour
    {
        public TMP_InputField inputField;
        public Button         btnSubmit;

        private void Awake() { this.btnSubmit.onClick.AddListener(this.OnSubmit); }

        private void OnEnable()
        {
#if !CREATIVE
            this.gameObject.SetActive(false);
#endif
        }

        private void OnSubmit()
        {
            if (!int.TryParse(this.inputField.text, out var level)) return;

            this.GetCurrentContainer().Resolve<UITemplateLevelDataController>().SelectLevel(level);
            this.GetCurrentContainer().Resolve<SignalBus>().Fire(new ChangeLevelCreativeSignal(level));
        }
    }
}