#if THEONE_LOCALIZATION
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using TMPro;
using UnityEditor.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Metadata;
using UnityEngine.Localization.Tables;

public class TMPOdinInspector : OdinEditorWindow
{
    // Toggle to filter results
    [HorizontalGroup("Search Group")]
    [LabelText("Filter by LocalizeStringEvent")]
    public bool filterByLocalizeStringEvent = false;

    [HorizontalGroup("Search Group")]
    [Button("Search TMP in Addressables")]
    private void SearchTMPInAddressables()
    {
        foundTMPTextInfos = new List<TMPTextInfo>();

        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
            return;

        foreach (AddressableAssetGroup group in settings.groups)
        {
            foreach (AddressableAssetEntry entry in group.entries)
            {
                string     assetPath = AssetDatabase.GUIDToAssetPath(entry.guid);
                GameObject obj       = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

                if (obj)
                {
                    TextMeshProUGUI[] tmpComponents = obj.GetComponentsInChildren<TextMeshProUGUI>(true);
                    foreach (var tmp in tmpComponents)
                    {
                        // Apply filter here
                        if (!filterByLocalizeStringEvent || tmp.GetComponent<LocalizeStringEvent>())
                        {
                            foundTMPTextInfos.Add(new TMPTextInfo(tmp));
                        }
                    }
                }
            }
        }
    }

    [ShowInInspector]
    [TableList]
    private List<TMPTextInfo> foundTMPTextInfos;

    [MenuItem("Window/TMP Odin Inspector")]
    private static void OpenWindow()
    {
        GetWindow<TMPOdinInspector>().Show();
    }
}

[System.Serializable]
public class TMPTextInfo
{
    [ShowInInspector, ReadOnly]
    private TextMeshProUGUI tmpText;
    
    [ShowInInspector, ReadOnly]
    public LocalizeStringEvent LocalizeEvent { get; private set; }

    [ShowInInspector, ReadOnly]
    public string DisplayText => tmpText.text;

    [ShowInInspector, ReadOnly]
    public string PrefabAssetPath => tmpText ? PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(tmpText.gameObject) : "";

    [ShowInInspector, ReadOnly]
    public string RelativePathInsidePrefab
    {
        get
        {
            if (tmpText)
            {
                Transform currentTransform = tmpText.transform;
                string path = currentTransform.name;
                while (currentTransform.parent != null)
                {
                    currentTransform = currentTransform.parent;
                    path = currentTransform.name + "/" + path;
                }
                return path;
            }
            return "";
        }
    }

    [ShowInInspector, ReadOnly]
    private IList<IMetadata> allTableEntries = GetAllTableEntries();

    private static IList<IMetadata> GetAllTableEntries()
    {
        // TODO: Retrieve your list of available TableEntryReference values from wherever they're stored
        // This is just a placeholder for demonstration.
        return LocalizationEditorSettings.ActiveLocalizationSettings.GetAssetDatabase().GetTable("StringLocalizationAssets").MetadataEntries;
    }

    [ShowInInspector, ValueDropdown("allTableEntries")]
    public TableEntryReference TableEntry
    {
        get => LocalizeEvent ? LocalizeEvent.StringReference.TableEntryReference : null;
        set
        {
            if (LocalizeEvent)
                LocalizeEvent.StringReference.TableEntryReference = value;
        }
    }
    
    [ShowInInspector]
    public string TableEntryValue
    {
        get => LocalizeEvent ? LocalizeEvent.StringReference.GetLocalizedString() : null;
    }

    public TMPTextInfo(TextMeshProUGUI text)
    {
        tmpText = text;
        LocalizeEvent = text.GetComponent<LocalizeStringEvent>();
    }

    [Button("Navigate")]
    private void NavigateToText()
    {
        if (tmpText)
        {
            Selection.activeGameObject = tmpText.gameObject;
            EditorGUIUtility.PingObject(tmpText.gameObject);

            if (PrefabUtility.GetPrefabAssetType(tmpText.gameObject) != PrefabAssetType.NotAPrefab)
            {
                string assetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(tmpText.gameObject);
                PrefabUtility.LoadPrefabContents(assetPath);
            }
        }
    }
}
#endif