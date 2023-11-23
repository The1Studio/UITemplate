namespace TheOneStudio.UITemplate.UITemplate.Creative.CheatLevel
{
    using GameFoundation.Scripts.Utilities.Extension;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;

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