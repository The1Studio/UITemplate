#if THEONE_LOCALIZATION
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Components;

namespace TheOne.Tool.Localization
{
    public static class LocalizationContextMenu
    {
        [MenuItem("CONTEXT/Text/Add to Localization")]
        private static void AddTextToLocalization(MenuCommand command)
        {
            var text = command.context as Text;
            if (text != null)
            {
                AddComponentToLocalization(text.gameObject, text.text, typeof(LocalizeStringEvent));
            }
        }
        
        [MenuItem("CONTEXT/TextMeshPro/Add to Localization")]
        private static void AddTMPToLocalization(MenuCommand command)
        {
            var tmp = command.context as TextMeshProUGUI;
            if (tmp != null)
            {
                AddComponentToLocalization(tmp.gameObject, tmp.text, typeof(LocalizeStringEvent));
            }
        }
        
        [MenuItem("CONTEXT/TextMeshProUGUI/Add to Localization")]
        private static void AddTMPUGUIToLocalization(MenuCommand command)
        {
            var tmp = command.context as TextMeshProUGUI;
            if (tmp != null)
            {
                AddComponentToLocalization(tmp.gameObject, tmp.text, typeof(LocalizeStringEvent));
            }
        }
        
        private static void AddComponentToLocalization(GameObject gameObject, string text, System.Type localizeComponentType)
        {
            if (string.IsNullOrEmpty(text))
            {
                EditorUtility.DisplayDialog("Empty Text", "Text component is empty. Cannot create localization key.", "OK");
                return;
            }
            
            // Generate key suggestion
            var suggestedKey = GenerateKeyFromText(text);
            
            // Show dialog to confirm key
            var key = EditorInputDialog.Show("Add to Localization", 
                $"Object: {gameObject.name}\nText: {text}\n\nEnter localization key:", 
                suggestedKey);
                
            if (string.IsNullOrEmpty(key))
                return; // User cancelled
            
            // Add to localization tables
            AutoLocalizationManager.AddLocalizationEntry(key, text);
            
            // Add LocalizeStringEvent component
            var localizeComponent = gameObject.GetComponent<LocalizeStringEvent>();
            if (localizeComponent == null)
            {
                localizeComponent = gameObject.AddComponent<LocalizeStringEvent>();
            }
            
            // Set the table reference (use first available collection)
            var collections = UnityEditor.Localization.LocalizationEditorSettings.GetStringTableCollections();
            if (collections.Count > 0)
            {
                localizeComponent.StringReference.TableReference = collections[0].TableCollectionNameReference;
                localizeComponent.StringReference.TableEntryReference = key;
            }
            
            // Mark objects as dirty
            EditorUtility.SetDirty(gameObject);
            EditorUtility.SetDirty(localizeComponent);
            
            Debug.Log($"✅ Added '{key}' to localization and connected to {gameObject.name}");
        }
        
        private static string GenerateKeyFromText(string text)
        {
            // Clean and format text to create a key
            var key = text.ToLower()
                .Replace(" ", "_")
                .Replace("!", "")
                .Replace("?", "")
                .Replace(".", "")
                .Replace(",", "")
                .Replace("'", "")
                .Replace("\"", "")
                .Replace("(", "")
                .Replace(")", "")
                .Replace("-", "_");
                
            // Limit length
            if (key.Length > 30)
            {
                key = key.Substring(0, 30);
            }
            
            // Add prefix
            return $"ui_{key}";
        }
    }
    
    public static class EditorInputDialog
    {
        public static string Show(string title, string message, string defaultValue = "")
        {
            var dialog = ScriptableObject.CreateInstance<InputDialog>();
            dialog.titleContent = new GUIContent(title);
            dialog.message = message;
            dialog.inputText = defaultValue;
            
            dialog.ShowModal();
            
            return dialog.confirmed ? dialog.inputText : null;
        }
    }
    
    public class InputDialog : EditorWindow
    {
        public string message = "";
        public string inputText = "";
        public bool confirmed = false;
        
        private void OnGUI()
        {
            EditorGUILayout.Space(10);
            
            EditorGUILayout.LabelField(this.message, EditorStyles.wordWrappedLabel);
            
            EditorGUILayout.Space(10);
            
            GUI.SetNextControlName("InputField");
            this.inputText = EditorGUILayout.TextField("Key:", this.inputText);
            
            // Focus on input field
            if (Event.current.type == EventType.Repaint)
            {
                GUI.FocusControl("InputField");
            }
            
            // Handle Enter key
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
            {
                this.confirmed = true;
                this.Close();
            }
            
            EditorGUILayout.Space(10);
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("OK"))
            {
                this.confirmed = true;
                this.Close();
            }
            
            if (GUILayout.Button("Cancel"))
            {
                this.confirmed = false;
                this.Close();
            }
            
            EditorGUILayout.EndHorizontal();
        }
    }
}
#endif
