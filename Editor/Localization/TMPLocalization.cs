#if THEONE_LOCALIZATION
namespace TheOne.Tool.Localization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Castle.DynamicProxy.Internal;
    using GameFoundation.Scripts.Utilities.Extension;
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using TheOne.Tool.Core;
    using TheOneStudio.UITemplate.UITemplate;
    using TMPro;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Localization.Components;
    using UnityEngine.Localization.Settings;
    using UnityEngine.Localization.Tables;
    using UnityEditor.Localization;
    using UnityEngine.Localization.Events;
    using Object = UnityEngine.Object;

    public enum TextMeshType
    {
        NoLocalized,
        StaticLocalized,
        DynamicLocalized
    }

    public class TMPLocalization : OdinEditorWindow
    {
        public static TMPLocalization Instance;

        private void OnEnable() { Instance = this; }

        [ValueDropdown("GetStringTableNames")]
        public string      StringTableName = "StringLocalizationAssets";

        public StringTable stringTable;
        public StringTableCollection selectedCollection;

        private IEnumerable<string> GetStringTableNames()
        {
            var stringTableCollections = LocalizationEditorSettings.GetStringTableCollections();
            return stringTableCollections.Select(collection => collection.TableCollectionName);
        }

        [HorizontalGroup("Action Group")]
        [Button("Search TMP in Addressable")]
        [GUIColor(0.3f, 0.8f, 0.3f)]
        private void SearchTMPInAddressable()
        {
            AssetDatabase.Refresh();
            this.noLocalizedTextInfos.Clear();
            this.staticLocalizedTextInfos.Clear();
            this.dynamicLocalizedTextInfos.Clear();

            // Get string table collection and table
            this.selectedCollection = LocalizationEditorSettings.GetStringTableCollection(Instance.StringTableName);
            if (this.selectedCollection == null)
            {
                Debug.LogError($"String table collection '{Instance.StringTableName}' not found!");
                return;
            }

            this.stringTable = LocalizationSettings.StringDatabase.GetTableAsync(Instance.StringTableName).Result;

            var gameObjects = AssetSearcher.GetAllAssetInAddressable<GameObject>().Keys;
            foreach (var obj in gameObjects)
            {
                var tmpComponents = obj.GetComponentsInChildren<TMP_Text>(true);
                foreach (var tmp in tmpComponents)
                {
                    var info = new TMPTextInfo(tmp, obj)
                    {
                        TextType = TextMeshType.NoLocalized
                    };

                    if (tmp.TryGetComponent<UITemplateAutoLocalization>(out var component))
                    {
                        var isDynamic = IsTMPReferencedInGameObject(tmp, obj, out var refMono, out var fieldName);
                        info.TextType = isDynamic ? TextMeshType.DynamicLocalized : TextMeshType.StaticLocalized;
                        
                        // Set reference for dynamic localized
                        if (isDynamic)
                        {
                            info.refMono = refMono;
                            info.fieldName = fieldName;
                        }
                    }

                    switch (info.TextType)
                    {
                        case TextMeshType.NoLocalized:
                            this.noLocalizedTextInfos.Add(info);
                            break;
                        case TextMeshType.StaticLocalized:
                            this.staticLocalizedTextInfos.Add(info);
                            break;
                        case TextMeshType.DynamicLocalized:
                            this.dynamicLocalizedTextInfos.Add(info);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            //Refesh the editor to show the updated lists
            this.Repaint();
        }

        public static bool IsTMPReferencedInGameObject(TMP_Text tmp, GameObject prefab, out MonoBehaviour refMono, out string fieldName)
        {
            fieldName = string.Empty;
            var monoBehaviours = prefab.GetComponentsInChildren<MonoBehaviour>(true);
            foreach (var monoBehaviour in monoBehaviours)
            {
                if (monoBehaviour == null)
                {
                    Debug.LogWarning($"{prefab.name} has null monoBehaviour");
                    continue;
                }

                var fields = monoBehaviour.GetType().GetAllFields();

                foreach (var field in fields)
                {
                    if (field.FieldType == typeof(TMP_Text))
                    {
                        var value = field.GetValue(monoBehaviour) as TMP_Text;
                        if (value == tmp)
                        {
                            refMono = monoBehaviour;
                            fieldName = field.Name;
                            return true;
                        }
                    }
                    else if (typeof(IEnumerable<TMP_Text>).IsAssignableFrom(field.FieldType))
                    {
                        if (field.GetValue(monoBehaviour) is IEnumerable<TMP_Text> collection && collection.Contains(tmp))
                        {
                            refMono = monoBehaviour;
                            fieldName = field.Name;
                            return true;
                        }
                    }
                }
            }

            refMono = null;
            return false;
        }

        [HorizontalGroup("Action Group")]
        [Button("AutoLocalized")]
        [GUIColor(1.0f, 0.5f, 0.0f)]
        private void DoAutoLocalized()
        {
            var localizedTextInfos = this.noLocalizedTextInfos.ToList();
            foreach (var textInfo in localizedTextInfos)
            {
                if (textInfo.DisplayText.Any(char.IsLetter))
                {
                    textInfo.Localized();
                    textInfo.SetupLocalizeStringEvent();
                }
            }
        }

        [HorizontalGroup("Action Group")]
        [Button("Add All Entry Keys")]
        [GUIColor(0.0f, 0.7f, 1.0f)]
        private void AddAllStaticEntryKeys()
        {
            // Ensure we have the string table collection
            if (this.selectedCollection == null)
            {
                this.selectedCollection = LocalizationEditorSettings.GetStringTableCollection(Instance.StringTableName);
            }

            if (this.selectedCollection == null)
            {
                Debug.LogError($"Could not find string table collection: {Instance.StringTableName}");
                return;
            }

            var staticLocalizedInfos = this.staticLocalizedTextInfos.ToList();
            int addedCount = 0;

            foreach (var textInfo in staticLocalizedInfos)
            {
                if (textInfo.DisplayText.Any(char.IsLetter))
                {
                    var textValue = textInfo.DisplayText;

                    // Check if entry already exists
                    var existingEntry = this.selectedCollection.SharedData.GetEntry(textValue);
                    if (existingEntry == null)
                    {
                        // Add new entry
                        var entry = this.selectedCollection.SharedData.AddKey(textValue);
                        if (entry != null)
                        {
                            // Add the entry to all locale tables in the collection
                            foreach (var table in this.selectedCollection.StringTables)
                            {
                                if (table != null)
                                {
                                    var tableEntry = table.AddEntry(entry.Key, textValue);
                                    if (tableEntry != null)
                                    {
                                        EditorUtility.SetDirty(table);
                                    }
                                }
                            }
                            addedCount++;
                        }
                    }
                }
            }

            if (addedCount > 0)
            {
                EditorUtility.SetDirty(this.selectedCollection.SharedData);
                AssetDatabase.SaveAssets();
                Debug.Log($"Added {addedCount} entry keys to string table collection.");
            }
            else
            {
                Debug.Log("No new entry keys were added. All entries may already exist.");
            }
        }
        public List<TMPTextInfo> NoLocalizedTextInfos      => this.noLocalizedTextInfos;
        public List<TMPTextInfo> StaticLocalizedTextInfos  => this.staticLocalizedTextInfos;
        public List<TMPTextInfo> DynamicLocalizedTextInfos => this.dynamicLocalizedTextInfos;

        [ShowInInspector, TabGroup("NoLocalized"), TableList]
        private List<TMPTextInfo> noLocalizedTextInfos = new();

        [ShowInInspector, TabGroup("StaticLocalized"), TableList]
        private List<TMPTextInfo> staticLocalizedTextInfos = new();

        [ShowInInspector, TabGroup("DynamicLocalized"), TableList]
        private List<TMPTextInfo> dynamicLocalizedTextInfos = new();
    }

    [Serializable]
    public class TMPTextInfo
    {
        [ShowInInspector, ReadOnly] private TMP_Text   tmpText;
        [ShowInInspector, ReadOnly] private GameObject prefab;

        [ShowIf("IsDynamicLocalized")] [ShowInInspector, ReadOnly]
        public MonoBehaviour refMono;
        [ShowIf("IsDynamicLocalized")] [ShowInInspector, ReadOnly]
        public string fieldName;

        [ShowInInspector, ReadOnly] public string DisplayText => this.tmpText.text;

        [ShowInInspector, ReadOnly] public TextMeshType TextType;

        public TMPTextInfo(TMP_Text text, GameObject gameObject)
        {
            this.tmpText = text;
            this.prefab = gameObject;
        }

        private void RemoveElementFromList()
        {
            if (TMPLocalization.Instance.StaticLocalizedTextInfos.Contains(this))
            {
                TMPLocalization.Instance.StaticLocalizedTextInfos.Remove(this);
            }

            if (TMPLocalization.Instance.NoLocalizedTextInfos.Contains(this))
            {
                TMPLocalization.Instance.NoLocalizedTextInfos.Remove(this);
            }

            if (TMPLocalization.Instance.DynamicLocalizedTextInfos.Contains(this))
            {
                TMPLocalization.Instance.DynamicLocalizedTextInfos.Remove(this);
            }
        }

        // This attribute specifies the condition for when to show the button
        [Button("$GetLocalizationButtonName")]
        [GUIColor(0.3f, 0.8f, 0.3f)]
        [ButtonGroup("Action")]
        public void Localized()
        {
            var isTMPReferencedInGameObject = TMPLocalization.IsTMPReferencedInGameObject(this.tmpText, this.prefab, out var refMn, out var fn);
            this.refMono = refMn;
            this.fieldName = fn;
            this.UpdatePrefab((tmpTextInInstance) =>
            {
                // Check on both instance and original prefab to handle variants
                var hasComponentOnInstance = tmpTextInInstance.TryGetComponent<UITemplateAutoLocalization>(out _);
                var hasComponentOnOriginal = this.tmpText.TryGetComponent<UITemplateAutoLocalization>(out _);
                
                if (!hasComponentOnInstance && !hasComponentOnOriginal)
                {
                    tmpTextInInstance.AddComponent<UITemplateAutoLocalization>();
                }
            }, isTMPReferencedInGameObject ? TMPLocalization.Instance.DynamicLocalizedTextInfos : TMPLocalization.Instance.StaticLocalizedTextInfos);
        }

        [ShowIf("IsDynamicLocalized")]
        [Button(ButtonSizes.Large)]
        [GUIColor(1.0f, 0.5f, 0.0f)]
        [ButtonGroup("Action")]
        public void MakeThisStatic()
        {
            this.UpdatePrefab((tmpGo) =>
            {
                var localizeStringEvent = tmpGo.GetComponent<LocalizeStringEvent>();
                if (localizeStringEvent == null)
                {
                    localizeStringEvent = tmpGo.AddComponent<LocalizeStringEvent>();
                }

                localizeStringEvent.SetTable(TMPLocalization.Instance.StringTableName);
                var textValue = this.tmpText.text;

                // Ensure we have the string table collection
                if (TMPLocalization.Instance.selectedCollection == null)
                {
                    TMPLocalization.Instance.selectedCollection = LocalizationEditorSettings.GetStringTableCollection(TMPLocalization.Instance.StringTableName);
                }

                if (TMPLocalization.Instance.selectedCollection != null)
                {
                    // Check if entry already exists, if not add it
                    var existingEntry = TMPLocalization.Instance.selectedCollection.SharedData.GetEntry(textValue);
                    string entryKey;

                    if (existingEntry != null)
                    {
                        entryKey = existingEntry.Key;
                    }
                    else
                    {
                        // Add new entry
                        var entry = TMPLocalization.Instance.selectedCollection.SharedData.AddKey(textValue);
                        if (entry != null)
                        {
                            entryKey = entry.Key;
                            // Add the entry to all locale tables in the collection
                            foreach (var table in TMPLocalization.Instance.selectedCollection.StringTables)
                            {
                                if (table != null)
                                {
                                    var tableEntry = table.AddEntry(entry.Key, textValue);
                                    if (tableEntry != null)
                                    {
                                        EditorUtility.SetDirty(table);
                                    }
                                }
                            }
                            EditorUtility.SetDirty(TMPLocalization.Instance.selectedCollection.SharedData);
                            AssetDatabase.SaveAssets();
                        }
                        else
                        {
                            Debug.LogError($"Failed to add entry key for text: {textValue}");
                            return;
                        }
                    }

                    localizeStringEvent.SetEntry(entryKey);
                }
                else
                {
                    Debug.LogError($"Could not find string table collection: {TMPLocalization.Instance.StringTableName}");
                }
            }, TMPLocalization.Instance.StaticLocalizedTextInfos);
        }

        [ShowIf("IsStaticLocalized")]
        [Button(ButtonSizes.Large)]
        [GUIColor(1.0f, 0.5f, 0.0f)]
        [ButtonGroup("Action")]
        public void MakeThisDynamic() { this.UpdatePrefab(_ => { }, TMPLocalization.Instance.DynamicLocalizedTextInfos); }

        public void SetupLocalizeStringEvent()
        {
            this.UpdatePrefab((tmpGo) =>
                {
                    var localizeStringEvent = tmpGo.GetComponent<LocalizeStringEvent>();
                    if (localizeStringEvent == null)
                    {
                        localizeStringEvent = tmpGo.AddComponent<LocalizeStringEvent>();
                    }

                    localizeStringEvent.SetTable(TMPLocalization.Instance.StringTableName);
                    var textValue = this.tmpText.text;

                    // Ensure we have the string table collection
                    if (TMPLocalization.Instance.selectedCollection == null)
                    {
                        TMPLocalization.Instance.selectedCollection = LocalizationEditorSettings.GetStringTableCollection(TMPLocalization.Instance.StringTableName);
                    }

                    if (TMPLocalization.Instance.selectedCollection != null)
                    {
                        // Find existing entry by text value
                        var existingEntry = TMPLocalization.Instance.selectedCollection.SharedData.GetEntry(textValue);
                        if (existingEntry != null)
                        {
                            localizeStringEvent.SetEntry(existingEntry.Key);

                            // Set the Update String to update at runtime
                            if (localizeStringEvent.OnUpdateString == null)
                            {
                                localizeStringEvent.OnUpdateString = new UnityEventString();
                            }

                            // Clear any existing listeners and add the text component's text property
                            localizeStringEvent.OnUpdateString.RemoveAllListeners();
                            localizeStringEvent.OnUpdateString.AddListener((string localizedText) =>
                            {
                                var textComponent = tmpGo.GetComponent<TMP_Text>();
                                if (textComponent != null)
                                {
                                    textComponent.text = localizedText;
                                }
                            });

                            // Set the string reference to target the 'text' property
                            localizeStringEvent.StringReference.SetReference(TMPLocalization.Instance.StringTableName, existingEntry.Key);

                            Debug.Log($"Set entry key '{existingEntry.Key}' for text: {textValue}");
                        }
                        else
                        {
                            Debug.LogWarning($"Entry not found for text: {textValue}. Please add entry key first.");
                        }
                    }
                    else
                    {
                        Debug.LogError($"Could not find string table collection: {TMPLocalization.Instance.StringTableName}");
                    }
                },
                TMPLocalization.Instance.StaticLocalizedTextInfos);
        }

        private bool IsDynamicLocalized() { return this.TextType == TextMeshType.DynamicLocalized; }

        private bool IsStaticLocalized() { return this.TextType == TextMeshType.StaticLocalized; }

        private string GetLocalizationButtonName() { return this.IsNoLocalized() ? "Add Localization" : "Remove Localization"; }

        private bool IsNoLocalized() { return this.TextType == TextMeshType.NoLocalized; }

        private void UpdatePrefab(Action<GameObject> updatePrefabAction, List<TMPTextInfo> newList)
        {
            var prefabInstance = PrefabUtility.InstantiatePrefab(this.prefab) as GameObject;
            var pathInPrefab = GetPathInPrefab(this.tmpText.transform);
            var tmpTextInInstance = GetGameObjectInPrefabInstance(prefabInstance, pathInPrefab);

            updatePrefabAction.Invoke(tmpTextInInstance);

            // Only apply changes to this specific prefab, not all variants
            PrefabUtility.ApplyPrefabInstance(prefabInstance, InteractionMode.UserAction);
            
            this.RemoveElementFromList();
            newList.Add(this);

            Object.DestroyImmediate(prefabInstance);

            if (newList == TMPLocalization.Instance.StaticLocalizedTextInfos)
            {
                this.TextType = TextMeshType.StaticLocalized;
            }
            else if (newList == TMPLocalization.Instance.DynamicLocalizedTextInfos)
            {
                this.TextType = TextMeshType.DynamicLocalized;
            }
            else
            {
                this.TextType = TextMeshType.NoLocalized;
            }
        }

        public static string GetPathInPrefab(Transform t)
        {
            if (t.parent == null)
                return ""; // Or return t.name if you want just the name of the root

            var path = t.name;
            while (t.parent.parent != null) // We check the parent's parent to stop one level before the root
            {
                t = t.parent;
                path = t.name + "/" + path;
            }

            return path;
        }

        public static GameObject GetGameObjectInPrefabInstance(GameObject prefabInstance, string pathInPrefab)
        {
            var foundTransform = prefabInstance.transform.Find(pathInPrefab);
            return foundTransform ? foundTransform.gameObject : null;
        }
    }
}
#endif