#if THEONE_LOCALIZATION
namespace TheOneStudio.UITemplate.UITemplate
{
    using System;
    using Sirenix.OdinInspector;
    using TMPro;
    using UnityEditor.Events;
    using UnityEngine;
    using UnityEngine.Localization.Components;
    using UnityEngine.Localization.Events;
    using UnityEngine.Events;

    [RequireComponent(typeof(LocalizeStringEvent))] [DisallowMultipleComponent] public class UITemplateAutoLocalization : MonoBehaviour
    {
        [Button] public void SetUpStringEvent(string tableName)
        {
            var localizeStringEvents = this.gameObject.GetComponents<LocalizeStringEvent>();

            if (localizeStringEvents.Length > 1)
            {
                for (var i = 1; i < localizeStringEvents.Length; i++)
                {
                    DestroyImmediate(localizeStringEvents[i]);
                }
            }


            var localizeStringEvent = this.gameObject.GetComponent<LocalizeStringEvent>();
            if (localizeStringEvent == null)
            {
                return;
            }

            localizeStringEvent.SetTable(tableName);
            var textValue = this.gameObject.GetComponent<TextMeshProUGUI>()?.text;

            localizeStringEvent.SetEntry(textValue);

            var textComponent = this.gameObject.GetComponent<TextMeshProUGUI>();
            if (textComponent != null)
            {
                if (textComponent.textWrappingMode != TextWrappingModes.NoWrap)
                {
                    textComponent.enableAutoSizing = true;
                }
                if (localizeStringEvent.OnUpdateString == null)
                {
                    localizeStringEvent.OnUpdateString = new UnityEventString();
                }

                // Clear any existing persistent listeners
                for (int i = localizeStringEvent.OnUpdateString.GetPersistentEventCount() - 1; i >= 0; i--)
                {
                    UnityEventTools.RemovePersistentListener(localizeStringEvent.OnUpdateString, i);
                }

                // Add persistent listener that will show up in Inspector
                UnityEventTools.AddPersistentListener(localizeStringEvent.OnUpdateString,
                    this.SetText);

                // Mark the object as dirty so Unity knows to save the changes
                #if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(localizeStringEvent);
                #endif
            }
        }

        public void SetText(string text)
        {
            var textComponent = this.gameObject.GetComponent<TMP_Text>();
            if (textComponent != null)
            {
                textComponent.textInfo.ClearAllMeshInfo();
                textComponent.text = text;
            }
        }
    }
}
#endif