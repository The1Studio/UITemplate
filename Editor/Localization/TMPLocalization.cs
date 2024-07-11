#if THEONE_LOCALIZATION
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using TMPro;
using UITemplate.Editor;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
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

    public string      StringTableName = "StringLocalizationAssets";
    public StringTable stringTable;
    
    [HorizontalGroup("Action Group")]
    [Button("Search TMP in Addressables")]
    [GUIColor(0.3f, 0.8f, 0.3f)]
    private void SearchTMPInAddressables()
    {
        AssetDatabase.Refresh();
        this.noLocalizedTextInfos.Clear();
        this.staticLocalizedTextInfos.Clear();
        this.dynamicLocalizedTextInfos.Clear();
        this.stringTable = LocalizationSettings.StringDatabase.GetTableAsync(Instance.StringTableName).Result;

        var gameObjects = AssetSearcher.GetAllAssetInAddressable<GameObject>().Keys;
        foreach (var obj in gameObjects)
        {
            var tmpComponents = obj.GetComponentsInChildren<TextMeshProUGUI>(true);
            foreach (var tmp in tmpComponents)
            {
                var info = new TMPTextInfo(tmp, obj);

                // if (tmp.TryGetComponent<LocalizeStringEvent>(out var component))
                // {
                //     info.TextType = this.stringTable.Values.Any(value => value.Key.Equals(tmp.text)) ? TextMeshType.StaticLocalized : TextMeshType.DynamicLocalized;
                // }
                info.TextType = gameObjects.Any(objr =>
                {
                    if (this.IsTMPReferencedInGameObject(tmp, objr, out var refMono))
                    {
                        info.refMono = refMono;
                        return true;
                    }
                    return false;
                }) ? TextMeshType.DynamicLocalized : TextMeshType.StaticLocalized;
                
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
    }
    
    private bool IsTMPReferencedInGameObject(TextMeshProUGUI tmp, GameObject obj, out MonoBehaviour refMono)
    {
        var monoBehaviours = obj.GetComponentsInChildren<MonoBehaviour>(true);
        foreach (var monoBehaviour in monoBehaviours)
        {
            if (monoBehaviour == null)
            {
                Debug.LogWarning($"{obj.name} has null monoBehaviour");
                continue;
            }
            var fields = monoBehaviour.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields)
            {
                if (field.FieldType == typeof(TextMeshProUGUI))
                {
                    var value = field.GetValue(monoBehaviour) as TextMeshProUGUI;
                    if (value == tmp)
                    {
                        refMono = monoBehaviour;
                        return true;
                    }
                }
                else if (typeof(IEnumerable<TextMeshProUGUI>).IsAssignableFrom(field.FieldType))
                {
                    if (field.GetValue(monoBehaviour) is IEnumerable<TextMeshProUGUI> collection && collection.Contains(tmp))
                    {
                        refMono = monoBehaviour;
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
        foreach (var textInfo in this.noLocalizedTextInfos.ToList())
        {
            if (textInfo.DisplayText.Any(char.IsLetter))
            {
                textInfo.Localized();
            }
        }
        
        foreach (var textInfo in this.DynamicLocalizedTextInfos.ToList())
        {
            if (!textInfo.DisplayText.Any(char.IsLetter))
            {
                textInfo.Localized();
            }
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

    [MenuItem("TheOne/Localization")]
    private static void OpenWindow() { GetWindow<TMPLocalization>().Show(); }
}

[Serializable]
public class TMPTextInfo
{
    [ShowInInspector, ReadOnly] private TextMeshProUGUI tmpText;
    [ShowInInspector, ReadOnly] private GameObject      prefab;
    [ShowIf("IsDynamicLocalized")]
    [ShowInInspector, ReadOnly] public  MonoBehaviour   refMono;
    [ShowInInspector, ReadOnly] public  string          DisplayText => this.tmpText.text;

    [ShowInInspector, ReadOnly] public TextMeshType TextType;

    public TMPTextInfo(TextMeshProUGUI text, GameObject gameObject)
    {
        this.tmpText = text;
        this.prefab  = gameObject;
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
        if (this.IsNoLocalized())
        {
            this.AddLocalizationLogic();
        }
        else 
        {
            this.RemoveLocalizationLogic();
        }
    }
    
    [ShowIf("IsDynamicLocalized")]
    [Button(ButtonSizes.Large)]
    [GUIColor(1.0f, 0.5f, 0.0f)]
    [ButtonGroup("Action")]
    public void MakeThisStatic()
    {
        this.UpdatePrefab((tmpText) =>
        {
            var localizeStringEvent = tmpText.GetComponent<LocalizeStringEvent>();
            localizeStringEvent.SetTable(TMPLocalization.Instance.StringTableName);
            var textValue = tmpText.GetComponent<TMP_Text>().text;
            TMPLocalization.Instance.stringTable.AddEntry(textValue, textValue);
            localizeStringEvent.SetEntry(textValue);
        }, TMPLocalization.Instance.StaticLocalizedTextInfos);
    }
    
    [ShowIf("IsStaticLocalized")]
    [Button(ButtonSizes.Large)]
    [GUIColor(1.0f, 0.5f, 0.0f)]
    [ButtonGroup("Action")]
    public void MakeThisDynamic()
    {
        this.UpdatePrefab(_ =>
        {
        }, TMPLocalization.Instance.DynamicLocalizedTextInfos);
    }

    private bool IsDynamicLocalized()
    {
        return this.TextType == TextMeshType.DynamicLocalized;
    }
    
    private bool IsStaticLocalized()
    {
        return this.TextType == TextMeshType.StaticLocalized;
    }

    private string GetLocalizationButtonName()
    {
        return this.IsNoLocalized() ? "Add Localization" : "Remove Localization";
    }

    private bool IsNoLocalized()
    {
        return this.TextType == TextMeshType.NoLocalized;
    }


    private void AddLocalizationLogic()
    {
        this.UpdatePrefab((tmpTextInInstance) =>
        {
            if (!tmpTextInInstance.TryGetComponent<LocalizeStringEvent>(out var staticComponent))
            {
                tmpTextInInstance.AddComponent<LocalizeStringEvent>();
            }
        }, TMPLocalization.Instance.DynamicLocalizedTextInfos);
    }

    private void RemoveLocalizationLogic()
    {
        this.UpdatePrefab((tmpTextInInstance) => Object.DestroyImmediate(tmpTextInInstance.GetComponent<LocalizeStringEvent>()), TMPLocalization.Instance.NoLocalizedTextInfos);
    }


    private void UpdatePrefab(Action<GameObject> updatePrefabAction, List<TMPTextInfo> newList)
    {
        GameObject prefabInstance    = PrefabUtility.InstantiatePrefab(this.prefab) as GameObject;
        string     pathInPrefab      = GetPathInPrefab(this.tmpText.transform);
        GameObject tmpTextInInstance = GetGameObjectInPrefabInstance(prefabInstance, pathInPrefab);

        updatePrefabAction.Invoke(tmpTextInInstance);

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

        string path = t.name;
        while (t.parent.parent != null) // We check the parent's parent to stop one level before the root
        {
            t    = t.parent;
            path = t.name + "/" + path;
        }

        return path;
    }

    public static GameObject GetGameObjectInPrefabInstance(GameObject prefabInstance, string pathInPrefab)
    {
        Transform foundTransform = prefabInstance.transform.Find(pathInPrefab);
        return foundTransform ? foundTransform.gameObject : null;
    }
}
#endif