#if THEONE_LOCALIZATION
namespace TheOneStudio.UITemplate.UITemplate
{
    using Sirenix.OdinInspector;
    using TMPro;
    using UnityEngine;
    using UnityEngine.Localization.Components;
    using UnityEngine.Localization.Events;

    [RequireComponent(typeof(LocalizeStringEvent))] [DisallowMultipleComponent] public class UITemplateAutoLocalization : MonoBehaviour
    {
        [Button] public void SetUpStringEvent(string tableName)
        {
            var localizeStringEvent = this.gameObject.GetComponent<LocalizeStringEvent>();
            if (localizeStringEvent == null)
            {
                return;
            }

            localizeStringEvent.SetTable(tableName);
            var textValue = this.gameObject.GetComponent<TextMeshProUGUI>()?.text;

            localizeStringEvent.SetEntry(textValue);
            if (localizeStringEvent.OnUpdateString == null)
            {
                localizeStringEvent.OnUpdateString = new UnityEventString();
            }

            // Clear any existing listeners and add the text component's text property
            localizeStringEvent.OnUpdateString.RemoveAllListeners();
            localizeStringEvent.OnUpdateString.AddListener(localizedText =>
            {
                var textComponent = this.gameObject.GetComponent<TMP_Text>();
                if (textComponent != null)
                {
                    textComponent.text = localizedText;
                }
            });
        }
    }
}
#endif