#if THEONE_LOCALIZATION
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

public class LocalizationToolWindow : OdinEditorWindow
{
    // Create a menu item in Unity's toolbar to open the window
    [MenuItem("Tools/Localization Tool")]
    private static void OpenWindow() { GetWindow<LocalizationToolWindow>().Show(); }

    // Button to trigger loading all prefabs
    [Button("Load All Prefabs")]
    public void LoadAllPrefabs()
    {
        AllTMPTexts.Clear();

        // Load all prefab locations from Addressables without filtering by label
        var locations = Addressables.LoadResourceLocationsAsync(typeof(GameObject));
        locations.Completed += OnLocationsLoaded;
    }

    private void OnLocationsLoaded(AsyncOperationHandle<IList<IResourceLocation>> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            foreach (var location in handle.Result)
            {
                var prefabInstance = Addressables.InstantiateAsync(location);
                prefabInstance.Completed += OnPrefabInstantiated;
            }
        }
    }

    private void OnPrefabInstantiated(AsyncOperationHandle<GameObject> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            var prefab = handle.Result;
            var texts  = prefab.GetComponentsInChildren<TMP_Text>(true);
            foreach (var text in texts)
            {
                AllTMPTexts.Add(new LocalizedTextData { gameObject = text.gameObject, tmpText = text, localizationType = LocalizationType.NoLocalized });
            }

            DestroyImmediate(prefab);
        }
    }

    // Display all the TMP_Text components in the editor window
    [Title("All TMP Texts")] [ShowInInspector, ListDrawerSettings(Expanded = true)]
    public List<LocalizedTextData> AllTMPTexts = new List<LocalizedTextData>();
}

public enum LocalizationType
{
    StaticLocalized,
    DynamicLocalized,
    NoLocalized
}

[Serializable]
public class LocalizedTextData
{
    public GameObject       gameObject;
    public TMP_Text         tmpText;
    public LocalizationType localizationType;
}
#endif